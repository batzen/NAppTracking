namespace NAppTracking.Server.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web.Mvc;
    using NAppTracking.Server.Configuration;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Services;
    using PagedList;

    [Authorize]
    public class ExceptionReportController : Controller
    {
        private readonly IEntitiesContext db;
        private readonly IAppConfiguration configuration;
        private readonly IFileStorageService fileStorageService;
        private readonly IFileSystemService fileSystemService;

        public ExceptionReportController(IEntitiesContext db, IAppConfiguration configuration, IFileStorageService fileStorageService, IFileSystemService fileSystemService)
        {
            this.db = db;
            this.configuration = configuration;
            this.fileStorageService = fileStorageService;
            this.fileSystemService = fileSystemService;
        }

        // GET: /ExceptionReport/
        public ActionResult Index(int? applicationId, int? page)
        {
            if (applicationId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var currentPageIndex = page.HasValue
                ? page.Value
                : 1;

            var pageSize = configuration.DefaultPageSize;

            var reports = db.ExceptionReports
                .Where(x => x.Application.Id == applicationId)
                .OrderByDescending(x => x.CreatedUtc)
                .ToPagedList(currentPageIndex, pageSize);

            return View(reports);
        }

        // GET: /ExceptionReport/Details/5
        public async Task<ActionResult> Details(int? exceptionReportId)
        {
            if (exceptionReportId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var exceptionreport = await db.ExceptionReports.FindAsync(exceptionReportId);

            if (exceptionreport == null)
            {
                return HttpNotFound();
            }

            return View(exceptionreport);
        }

        // GET: /ExceptionReport/Delete/5
        public async Task<ActionResult> Delete(int? exceptionReportId)
        {
            if (exceptionReportId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var exceptionreport = await db.ExceptionReports.FindAsync(exceptionReportId);

            if (exceptionreport == null)
            {
                return HttpNotFound();
            }

            return View(exceptionreport);
        }

        // POST: /ExceptionReport/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int exceptionReportId)
        {
            var exceptionreport = await db.ExceptionReports.FindAsync(exceptionReportId);
            db.ExceptionReports.Remove(exceptionreport);

            foreach (var file in exceptionreport.ExceptionReportFiles)
            {
                await this.fileStorageService.DeleteFileAsync(file.FileName);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult File(Guid storageId)
        {
            var result = this.db.ExceptionReportFiles
                .Where(x => x.StorageId == storageId)
                .Select(x => new {ApplicationId = x.ExceptionReport.Application.Id, ExceptionId = x.ExceptionReport.Id, FileName = x.FileName})
                .FirstOrDefault();

            if (result == null)
            {
                return this.HttpNotFound();
            }

            var path = this.fileSystemService.BuildPath(string.Format("Application_{0}/Exception_{1}/{2}", result.ApplicationId, result.ExceptionId, storageId));

            return this.File(path, "application/force-download", result.FileName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}