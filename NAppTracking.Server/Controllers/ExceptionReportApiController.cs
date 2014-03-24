namespace NAppTracking.Server.Controllers
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Filters;

    [ApiAuthorize]
    //[Authorize]
    public class ExceptionReportApiController : ApiController
    {
        private EntitiesContext db = new EntitiesContext();

        // GET api/ExceptionReportApi
        public IQueryable<ExceptionReport> GetExceptionReports()
        {
            return db.ExceptionReports;
        }

        // GET api/ExceptionReportApi/5
        [ResponseType(typeof(ExceptionReport))]
        public async Task<IHttpActionResult> GetExceptionReport(int id)
        {
            ExceptionReport exceptionreport = await db.ExceptionReports.FindAsync(id);
            if (exceptionreport == null)
            {
                return NotFound();
            }

            return Ok(exceptionreport);
        }

        // PUT api/ExceptionReportApi/5
        public async Task<IHttpActionResult> PutExceptionReport(int id, ExceptionReport exceptionreport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != exceptionreport.Id)
            {
                return BadRequest();
            }

            db.Entry(exceptionreport).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExceptionReportExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/ExceptionReportApi
        [ResponseType(typeof(ExceptionReport))]
        public async Task<IHttpActionResult> PostExceptionReport(ExceptionReport exceptionreport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ExceptionReports.Add(exceptionreport);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = exceptionreport.Id }, exceptionreport);
        }

        // DELETE api/ExceptionReportApi/5
        [ResponseType(typeof(ExceptionReport))]
        public async Task<IHttpActionResult> DeleteExceptionReport(int id)
        {
            ExceptionReport exceptionreport = await db.ExceptionReports.FindAsync(id);
            if (exceptionreport == null)
            {
                return NotFound();
            }

            db.ExceptionReports.Remove(exceptionreport);
            await db.SaveChangesAsync();

            return Ok(exceptionreport);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ExceptionReportExists(int id)
        {
            return db.ExceptionReports.Count(e => e.Id == id) > 0;
        }
    }
}