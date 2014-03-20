using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NAppTracking.Server.Models;

namespace NAppTracking.Server.Controllers
{
    using NAppTracking.Server.Entities;

    public class ApplicationsController : Controller
    {
        private EntitiesContext db = new EntitiesContext();

        // GET: /Applications/
        public async Task<ActionResult> Index()
        {
            return View(await db.TrackingApplications.ToListAsync());
        }

        // GET: /Applications/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrackingApplication trackingapplication = await db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
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
        public async Task<ActionResult> Create([Bind(Include="Key,Name,Description")] TrackingApplication trackingapplication)
        {
            if (ModelState.IsValid)
            {
                db.TrackingApplications.Add(trackingapplication);
                trackingapplication.ApiKey = Guid.NewGuid().ToString().ToLowerInvariant();
                await db.SaveChangesAsync();
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
            TrackingApplication trackingapplication = await db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }
            return View(trackingapplication);
        }

        // POST: /Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Key,Name,Description")] TrackingApplication trackingapplication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trackingapplication).State = EntityState.Modified;
                await db.SaveChangesAsync();
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
            TrackingApplication trackingapplication = await db.TrackingApplications.FindAsync(id);
            if (trackingapplication == null)
            {
                return HttpNotFound();
            }
            return View(trackingapplication);
        }

        // POST: /Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TrackingApplication trackingapplication = await db.TrackingApplications.FindAsync(id);
            db.TrackingApplications.Remove(trackingapplication);
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