namespace NAppTracking.Server.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using NAppTracking.Server.Entities;

    public interface IExceptionReportFileStorageService
    {
        Task Store(ExceptionReportFile exceptionReportFile, MultipartFileData fileData);

        void Delete(ExceptionReport exceptionReport);

        void Delete(ExceptionReportFile exceptionReportFile);
    }
}