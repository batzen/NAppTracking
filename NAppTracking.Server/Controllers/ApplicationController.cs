namespace NAppTracking.Server.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using NAppTracking.Server.Entities;

    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IEntitiesContext db;
        private ApplicationUserManager userManager;

        public ApplicationController(IEntitiesContext db)
        {
            this.db = db;
        }

        public ApplicationController(IEntitiesContext db, ApplicationUserManager userManager)
        {
            this.db = db;
            this.UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this.userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                this.userManager = value;
            }
        }

        // GET: /Application/
        public async Task<ActionResult> Index()
        {
            return View(await this.db.TrackingApplications.ToListAsync());
        }

        // GET: /Application/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }

            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(trackingapplication);
        }

        // GET: /Application/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Application/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Name,Description")] TrackingApplication trackingapplication)
        {
            if (ModelState.IsValid)
            {                
                this.db.TrackingApplications.Add(trackingapplication);
                var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                trackingapplication.Owners.Add(currentUser);
                await this.db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(trackingapplication);
        }

        // GET: /Application/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }

            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(trackingapplication);
        }

        // POST: /Application/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Name,Description")] TrackingApplication trackingapplication)
        {
            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                this.db.Entry(trackingapplication).State = EntityState.Modified;
                await this.db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(trackingapplication);
        }

        // GET: /Application/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }

            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(trackingapplication);
        }

        // POST: /Application/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var trackingapplication = await this.db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }

            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            this.db.TrackingApplications.Remove(trackingapplication);
            await this.db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ManageOwners(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }

            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return this.View(trackingapplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOwnerConfirmed(int id, string username)
        {
            var trackingapplication = await this.db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }

            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var owner = await this.UserManager.FindByNameAsync(username);
            if (owner == null)
            {
                // todo: implement proper user notification for error
                return this.HttpNotFound();
            }

            // Prevent duplicate owner addition
            if (trackingapplication.Owners.All(x => x.Id != owner.Id))
            {
                trackingapplication.Owners.Add(owner);
                await this.db.SaveChangesAsync();
            }

            return this.RedirectToAction("ManageOwners", new { id });
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