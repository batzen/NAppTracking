namespace NAppTracking.Server.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web;
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
        // id is used as page and just called id here so we are able to use the default routing
        public ActionResult Index(int? id, int? applicationId)
        {
            var currentPageIndex = id.HasValue
                ? id.Value
                : 1;

            var pageSize = configuration.DefaultPageSize;

            var reports = db.ExceptionReports
                .Where(x => (applicationId.HasValue == false || x.Application.Id == applicationId) && x.Application.Owners.Any(u => u.UserName == this.User.Identity.Name))
                .OrderByDescending(x => x.CreatedUtc)
                .ToPagedList(currentPageIndex, pageSize);

            return View(reports);
        }

        // GET: /ExceptionReport/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var exceptionreport = await db.ExceptionReports.FindAsync(id);

            if (exceptionreport == null)
            {
                return HttpNotFound();
            }

            if (exceptionreport.Application.IsOwner(this.User) == false)
            {
                return new HttpUnauthorizedResult();
            }

            return View(exceptionreport);
        }

        // GET: /ExceptionReport/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var exceptionreport = await db.ExceptionReports.FindAsync(id);

            if (exceptionreport == null)
            {
                return HttpNotFound();
            }

            if (exceptionreport.Application.IsOwner(this.User) == false)
            {
                return new HttpUnauthorizedResult();
            }

            return View(exceptionreport);
        }

        // POST: /ExceptionReport/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var exceptionreport = await db.ExceptionReports.FindAsync(id);

            if (exceptionreport == null)
            {
                return HttpNotFound();
            }

            if (exceptionreport.Application.IsOwner(this.User) == false)
            {
                return new HttpUnauthorizedResult();
            }

            db.ExceptionReports.Remove(exceptionreport);

            foreach (var file in exceptionreport.ExceptionReportFiles)
            {
                await this.fileStorageService.DeleteFileAsync(file.FileName);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult File(int id)
        {
            var exceptionReportFile = this.db.ExceptionReportFiles.FirstOrDefault(x => x.Id == id);

            if (exceptionReportFile == null)
            {
                return this.HttpNotFound();
            }

            if (exceptionReportFile.ExceptionReport.Application.IsOwner(this.User) == false)
            {
                return new HttpUnauthorizedResult();
            }

            var path = this.fileSystemService.BuildPath(string.Format("Application_{0}/Exception_{1}/{2}_{3}", exceptionReportFile.ExceptionReport.Application.Id, exceptionReportFile.ExceptionReport.Id, exceptionReportFile.StorageId, exceptionReportFile.FileName));

            return this.File(path, exceptionReportFile.MIMEType, exceptionReportFile.FileName);
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