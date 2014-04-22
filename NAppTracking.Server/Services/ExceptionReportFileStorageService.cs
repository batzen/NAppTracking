namespace NAppTracking.Server.Services
{
    using System;
    using System.Web;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using NAppTracking.Server.Entities;

    public class ExceptionReportFileStorageService : IExceptionReportFileStorageService
    {
        private readonly IFileSystemService fileSystemService;
        private readonly IFileStorageService fileStorageService;

        public ExceptionReportFileStorageService(IFileSystemService fileSystemService, IFileStorageService fileStorageService)
        {
            this.fileSystemService = fileSystemService;
            this.fileStorageService = fileStorageService;
        }

        public async Task Store(ExceptionReportFile exceptionReportFile, MultipartFileData fileData)
        {
            var exceptionReportPath = this.fileSystemService.BuildPath(string.Format("Application_{0}/Exception_{1}", exceptionReportFile.ExceptionReport.Application.Id, exceptionReportFile.ExceptionReport.Id));

            if (!this.fileSystemService.DirectoryExists(exceptionReportPath))
            {
                this.fileSystemService.CreateDirectory(exceptionReportPath);
            }

            exceptionReportFile.StorageId = Guid.NewGuid();
            exceptionReportFile.FileName = fileData.Headers.ContentDisposition.FileName;
            exceptionReportFile.MIMEType = MimeMapping.GetMimeMapping(exceptionReportFile.FileName);
            exceptionReportFile.Size = new FileInfo(fileData.LocalFileName).Length;

            var fileName = string.Format("{0}_{1}", exceptionReportFile.StorageId, exceptionReportFile.FileName);
            await this.fileStorageService.MoveFileAsync(fileData.LocalFileName, Path.Combine(exceptionReportPath, fileName));
        }

        public void Delete(ExceptionReport exceptionReport)
        {
            var exceptionReportPath = this.fileSystemService.BuildPath(string.Format("Application_{0}/Exception_{1}", exceptionReport.Application.Id, exceptionReport.Id));

            if (!this.fileSystemService.DirectoryExists(exceptionReportPath))
            {
                return;
            }

            this.fileSystemService.DeleteDirectory(exceptionReportPath, true);
        }

        public void Delete(ExceptionReportFile exceptionReportFile)
        {
            var exceptionReportPath = this.fileSystemService.BuildPath(string.Format("Application_{0}/Exception_{1}", exceptionReportFile.ExceptionReport.Application.Id, exceptionReportFile.ExceptionReport.Id));

            if (!this.fileSystemService.DirectoryExists(exceptionReportPath))
            {
                return;
            }

            var fileName = string.Format("{0}_{1}", exceptionReportFile.StorageId, exceptionReportFile.FileName);
            this.fileSystemService.DeleteFile(Path.Combine(exceptionReportPath, fileName));
        }
    }
}