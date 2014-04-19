﻿namespace NAppTracking.Server.Controllers.Api
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
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
        private readonly IEntitiesContext db;
        private readonly IFileStorageService fileStorageService;
        private readonly IFileSystemService fileSystemService;

        public ExceptionReportController(IEntitiesContext db, IFileSystemService fileSystemService, IFileStorageService fileStorageService)
        {
            this.db = db;
            this.fileSystemService = fileSystemService;
            this.fileStorageService = fileStorageService;
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

            var exceptionReportPath = this.fileSystemService.BuildPath(string.Format("Application_{0}/Exception_{1}", application.Id, id));

            if (!this.fileSystemService.DirectoryExists(exceptionReportPath))
            {
                this.fileSystemService.CreateDirectory(exceptionReportPath);
            }

            try
            {
                var provider = new MultipartFormDataStreamProvider(tempPath);

                // Read the form data.
                await this.Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (var file in provider.FileData)
                {
                    var exceptionReportFile = this.CreateExceptionReportFile(exceptionReport, file);
                    
                    var fileName = string.Format("{0}_{1}", exceptionReportFile.StorageId, exceptionReportFile.FileName);
                    await this.fileStorageService.MoveFileAsync(file.LocalFileName, Path.Combine(exceptionReportPath, fileName));

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

        private ExceptionReportFile CreateExceptionReportFile(ExceptionReport exceptionReport, MultipartFileData file)
        {
            var exceptionReportFile = this.db.ExceptionReportFiles.Create();
            exceptionReportFile.ExceptionReport = exceptionReport;
            exceptionReportFile.StorageId = Guid.NewGuid();
            exceptionReportFile.FileName = file.Headers.ContentDisposition.FileName;
            exceptionReportFile.MIMEType = MimeMapping.GetMimeMapping(exceptionReportFile.FileName);
            exceptionReportFile.Size = new FileInfo(file.LocalFileName).Length;

            return exceptionReportFile;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}