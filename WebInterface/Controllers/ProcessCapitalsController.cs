using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EconModels;
using EconModels.ProcessModel;

namespace WebInterface.Controllers
{
    public class ProcessCapitalsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: ProcessCapitals
        public ActionResult Index()
        {
            var processCapitals = db.ProcessCapitals.Include(p => p.Capital).Include(p => p.Process);
            return View(processCapitals.ToList());
        }

        // GET: ProcessCapitals/Details/5
        public ActionResult Details(int? processId, int? capitalId)
        {
            if (processId == null || capitalId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessCapital processCapital = db.ProcessCapitals.SingleOrDefault(x => x.ProcessId == processId
                                                                      && x.CapitalId == capitalId);
            if (processCapital == null)
            {
                return HttpNotFound();
            }
            return View(processCapital);
        }

        // GET: ProcessCapitals/Create
        public ActionResult Create()
        {
            ViewBag.CapitalId = new SelectList(db.Products, "Id", "Name");
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name");
            return View();
        }

        // POST: ProcessCapitals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProcessId,CapitalId,Amount,Tag")] ProcessCapital processCapital)
        {
            if (ModelState.IsValid)
            {
                db.ProcessCapitals.Add(processCapital);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CapitalId = new SelectList(db.Products, "Id", "Name", processCapital.CapitalId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processCapital.ProcessId);
            return View(processCapital);
        }

        // GET: ProcessCapitals/Edit/5
        public ActionResult Edit(int? processId, int? capitalId)
        {
            if (processId == null || capitalId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessCapital processCapital = db.ProcessCapitals.SingleOrDefault(x => x.ProcessId == processId
                                                                      && x.CapitalId == capitalId);
            if (processCapital == null)
            {
                return HttpNotFound();
            }
            ViewBag.CapitalId = new SelectList(db.Products, "Id", "Name", processCapital.CapitalId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processCapital.ProcessId);
            return View(processCapital);
        }

        // POST: ProcessCapitals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProcessId,CapitalId,Amount,Tag")] ProcessCapital processCapital)
        {
            if (ModelState.IsValid)
            {
                db.Entry(processCapital).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CapitalId = new SelectList(db.Products, "Id", "Name", processCapital.CapitalId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processCapital.ProcessId);
            return View(processCapital);
        }

        // GET: ProcessCapitals/Delete/5
        public ActionResult Delete(int? processId, int? capitalId)
        {
            if (processId == null || capitalId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessCapital processCapital = db.ProcessCapitals.SingleOrDefault(x => x.ProcessId == processId
                                                                      && x.CapitalId == capitalId);
            if (processCapital == null)
            {
                return HttpNotFound();
            }
            return View(processCapital);
        }

        // POST: ProcessCapitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int processId, int capitalId)
        {
            ProcessCapital processCapital = db.ProcessCapitals.SingleOrDefault(x => x.ProcessId == processId
                                                                      && x.CapitalId == capitalId);
            db.ProcessCapitals.Remove(processCapital);
            db.SaveChanges();
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
