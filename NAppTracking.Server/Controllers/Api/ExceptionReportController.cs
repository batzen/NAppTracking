namespace NAppTracking.Server.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using AutoMapper;
    using NAppTracking.Dto;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Filters;
    using NAppTracking.Server.Helpers;
    using NAppTracking.Server.Services;

    [ApiAuthorize]
    public class ExceptionReportController : ApiController
    {
        private readonly EntitiesContext db;
        private readonly IFileSystemService fileSystemService;
        private readonly IExceptionReportFileStorageService exceptionReportFileStorageService;
        private readonly IMailSenderService mailSenderService;

        public ExceptionReportController(EntitiesContext db, IFileSystemService fileSystemService, IExceptionReportFileStorageService exceptionReportFileStorageService, IMailSenderService mailSenderService)
        {
            this.db = db;
            this.fileSystemService = fileSystemService;
            this.exceptionReportFileStorageService = exceptionReportFileStorageService;
            this.mailSenderService = mailSenderService;
        }

        // POST api/ExceptionReport
        [HttpPost]
        [ResponseType(typeof(ExceptionReport))]
        public async Task<IHttpActionResult> PostExceptionReport(ExceptionReportDto exceptionReportDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            
            if (exceptionReportDto == null)
            {
                return this.BadRequest("No data specified for exception report.");
            }

            var apiKey = ApiKeyHelper.GetApiKey(this.Request);

            var application = this.db.TrackingApplications.FirstOrDefault(x => x.ApiKey == apiKey);

            if (application == null)
            {
                return this.BadRequest("No application found for supplied Api-Key.");
            }

            var exceptionReport = Mapper.Map<ExceptionReportDto, ExceptionReport>(exceptionReportDto);
            exceptionReport.Application = application;

            this.db.ExceptionReports.Add(exceptionReport);
            await this.db.SaveChangesAsync();

            await this.SendExceptionReportMails(exceptionReport);

            return this.CreatedAtRoute("DefaultApi", new { exceptionReport.Id }, exceptionReport);
        }

        private async Task SendExceptionReportMails(ExceptionReport exceptionReport)
        {
            var message = new StringBuilder();
            message.AppendLine(this.Url.Link("Default", new {controller = "ExceptionReport", action = "Details", id = exceptionReport.Id}));
            message.AppendLine();
            message.AppendFormat("Message: {0}", exceptionReport.Message);
            message.AppendLine();
            message.AppendLine();
            message.AppendLine("Details:");
            message.AppendLine(exceptionReport.Details);

            await this.mailSenderService.SendAsync(exceptionReport.Application.Owners, exceptionReport.Message, message.ToString());
        }

        // PUT api/ExceptionReport/5
        [HttpPut]
        public async Task<IHttpActionResult> PutExceptionReport(int id)
        {
            // Check if the request contains multipart/form-data.
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var apiKey = ApiKeyHelper.GetApiKey(this.Request);

            var application = this.db.TrackingApplications.FirstOrDefault(x => x.ApiKey == apiKey);

            if (application == null)
            {
                return this.BadRequest("No application found for supplied Api-Key.");
            }

            var exceptionReport = await this.db.ExceptionReports.FindAsync(id);

            if (exceptionReport == null)
            {
                return this.NotFound();
            }

            var tempPath = this.fileSystemService.BuildPath("Temp");

            if (!this.fileSystemService.DirectoryExists(tempPath))
            {
                this.fileSystemService.CreateDirectory(tempPath);
            }

            try
            {
                var provider = new MultipartFormDataStreamProvider(tempPath);

                await this.Request.Content.ReadAsMultipartAsync(provider);
                
                foreach (var fileData in provider.FileData)
                {
                    var exceptionReportFile = this.db.ExceptionReportFiles.Create();
                    exceptionReportFile.ExceptionReport = exceptionReport;
                    
                    await exceptionReportFileStorageService.Store(exceptionReportFile, fileData);

                    this.db.ExceptionReportFiles.Add(exceptionReportFile);
                }

                await this.db.SaveChangesAsync();

                return this.Ok();
            }
            catch (Exception e)
            {
                return this.InternalServerError(e);
            }
        }
    }
}