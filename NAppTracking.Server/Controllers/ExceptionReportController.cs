namespace NAppTracking.Server.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web.Mvc;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Services;
    using PagedList;

    [Authorize]
    public class ExceptionReportController : Controller
    {
        private readonly EntitiesContext db;
        private readonly IFileStorageService fileStorageService;

        public ExceptionReportController(EntitiesContext db, IFileStorageService fileStorageService)
        {
            this.db = db;
            this.fileStorageService = fileStorageService;
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

            var pageSize = 25;

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

            ExceptionReport exceptionreport = await db.ExceptionReports.FindAsync(exceptionReportId);

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

            ExceptionReport exceptionreport = await db.ExceptionReports.FindAsync(exceptionReportId);

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