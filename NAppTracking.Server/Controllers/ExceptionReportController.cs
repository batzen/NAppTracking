namespace NAppTracking.Server.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web.Mvc;
    using Microsoft.Ajax.Utilities;
    using NAppTracking.Server.Configuration;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Services;
    using PagedList;

    [Authorize]
    public class ExceptionReportController : Controller
    {
        private readonly EntitiesContext db;
        private readonly IAppConfiguration configuration;
        private readonly IFileStorageService fileStorageService;
        private readonly IFileSystemService fileSystemService;
        private readonly IExceptionReportFileStorageService exceptionReportFileStorageService;

        public ExceptionReportController(EntitiesContext db, IAppConfiguration configuration, IFileStorageService fileStorageService, IFileSystemService fileSystemService, IExceptionReportFileStorageService exceptionReportFileStorageService)
        {
            this.db = db;
            this.configuration = configuration;
            this.fileStorageService = fileStorageService;
            this.fileSystemService = fileSystemService;
            this.exceptionReportFileStorageService = exceptionReportFileStorageService;
        }

        // GET: /ExceptionReport/
        // id is used as page and just called id here so we are able to use the default routing
        public ActionResult Index(int? id, int? applicationId, string q)
        {
            this.ViewBag.SearchQuery = q;

            var currentPageIndex = id.HasValue
                ? id.Value
                : 1;

            var pageSize = configuration.DefaultPageSize;

            var reports = db.ExceptionReports
                .Where(x => (applicationId.HasValue == false || x.Application.Id == applicationId) && x.Application.Owners.Any(u => u.UserName == this.User.Identity.Name));

            if (q.IsNullOrWhiteSpace() == false)
            {
                reports = reports.Where(report => report.Message.Contains(q));
            }

            var pagedAndOrderedReports = reports.OrderByDescending(x => x.CreatedUtc)
                .ToPagedList(currentPageIndex, pageSize);

            return View(pagedAndOrderedReports);
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

            this.exceptionReportFileStorageService.Delete(exceptionreport);

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

            if (this.fileSystemService.FileExists(path) == false)
            {
                return this.HttpNotFound();
            }

            return this.File(path, exceptionReportFile.MIMEType, exceptionReportFile.FileName);
        }
    }
}