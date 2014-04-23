namespace NAppTracking.Server.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using AutoMapper;
    using NAppTracking.Client;
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

        public ExceptionReportController(EntitiesContext db, IFileSystemService fileSystemService, IExceptionReportFileStorageService exceptionReportFileStorageService)
        {
            this.db = db;
            this.fileSystemService = fileSystemService;
            this.exceptionReportFileStorageService = exceptionReportFileStorageService;
        }

        // POST api/ExceptionReport
        [HttpPost]
        [ResponseType(typeof(ExceptionReport))]
        public async Task<IHttpActionResult> PostExceptionReport(ExceptionReportDto exceptionreportDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            
            if (exceptionreportDto == null)
            {
                return this.BadRequest("No data specified for exception report.");
            }
           
            var exceptionreport = Mapper.Map<ExceptionReportDto, ExceptionReport>(exceptionreportDto);
            var apiKey = ApiKeyHelper.GetApiKey(this.Request);

            var application = this.db.TrackingApplications.FirstOrDefault(x => x.ApiKey == apiKey);

            if (application == null)
            {
                return this.BadRequest("No application found for supplied Api-Key.");
            }
            
            exceptionreport.Application = application;

            this.db.ExceptionReports.Add(exceptionreport);
            await this.db.SaveChangesAsync();

            return this.CreatedAtRoute("DefaultApi", new { exceptionreport.Id }, exceptionreport);
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