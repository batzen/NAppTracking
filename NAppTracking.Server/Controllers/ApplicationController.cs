namespace NAppTracking.Server.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using NAppTracking.Server.Entities;

    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly EntitiesContext db;
        private readonly ApplicationUserManager userManager;

        public ApplicationController(EntitiesContext db, ApplicationUserManager userManager)
        {
            this.db = db;
            this.userManager = userManager;
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
                var currentUser = await this.userManager.FindByIdAsync(User.Identity.GetUserId());
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

        [ActionName("ManageOwners")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOwner(int id, string username)
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

            var owner = await this.userManager.FindByNameAsync(username);
            if (owner == null)
            {
                this.ModelState.AddModelError(string.Empty, "User not found");
                this.ViewBag.Username = username;
                return this.View(trackingapplication);
            }

            // Prevent duplicate owner addition
            if (trackingapplication.Owners.All(x => x.Id != owner.Id))
            {
                trackingapplication.Owners.Add(owner);
                await this.db.SaveChangesAsync();
            }

            return this.View(trackingapplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveOwner(int id, string ownerId)
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

            var owner = await this.userManager.FindByIdAsync(ownerId);
            if (owner == null)
            {
                // todo: implement proper user notification for error
                return this.HttpNotFound();
            }

            trackingapplication.Owners.Remove(owner);
            await this.db.SaveChangesAsync();

            return this.RedirectToAction("ManageOwners", new { trackingapplication.Id });
        }
    }
}