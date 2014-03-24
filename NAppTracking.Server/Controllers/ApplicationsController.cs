namespace NAppTracking.Server.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using NAppTracking.Server.Entities;

    [Authorize]
    public class ApplicationsController : Controller
    {
        public ApplicationsController()
            : this(new EntitiesContext()) 
        {
        }

        public ApplicationsController(EntitiesContext db)
        {
            this.Db = db;
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public EntitiesContext Db { get; private set; }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        // GET: /Applications/
        public async Task<ActionResult> Index()
        {
            return View(await this.Db.TrackingApplications.ToListAsync());
        }

        // GET: /Applications/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.Db.TrackingApplications.FindAsync(id);
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

        // GET: /Applications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Applications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,Name,Description")] TrackingApplication trackingapplication)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                trackingapplication.Owners.Add(currentUser);
                this.Db.TrackingApplications.Add(trackingapplication);
                await this.Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(trackingapplication);
        }

        // GET: /Applications/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.Db.TrackingApplications.FindAsync(id);
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

        // POST: /Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,Name,Description")] TrackingApplication trackingapplication)
        {
            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                this.Db.Entry(trackingapplication).State = EntityState.Modified;
                await this.Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(trackingapplication);
        }

        // GET: /Applications/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.Db.TrackingApplications.FindAsync(id);
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

        // POST: /Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var trackingapplication = await this.Db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }

            if (!trackingapplication.IsOwner(this.User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            this.Db.TrackingApplications.Remove(trackingapplication);
            await this.Db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ManageOwners(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var trackingapplication = await this.Db.TrackingApplications.FindAsync(id);
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
            var trackingapplication = await this.Db.TrackingApplications.FindAsync(id);
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
                await this.Db.SaveChangesAsync();
            }

            return this.RedirectToAction("ManageOwners", new {id = id});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}