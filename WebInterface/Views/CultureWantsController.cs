using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EconModels;
using EconModels.PopulationModel;

namespace WebInterface.Views
{
    public class CultureWantsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: CultureWants
        public ActionResult Index()
        {
            var cultureWants = db.CultureWants.Include(c => c.Culture);
            return View(cultureWants.ToList());
        }

        // GET: CultureWants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureWant cultureWant = db.CultureWants.Find(id);
            if (cultureWant == null)
            {
                return HttpNotFound();
            }
            return View(cultureWant);
        }

        // GET: CultureWants/Create
        public ActionResult Create()
        {
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name");
            return View();
        }

        // POST: CultureWants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CultureId,Want,NeedType,Amount")] CultureWant cultureWant)
        {
            if (ModelState.IsValid)
            {
                db.CultureWants.Add(cultureWant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureWant.CultureId);
            return View(cultureWant);
        }

        // GET: CultureWants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureWant cultureWant = db.CultureWants.Find(id);
            if (cultureWant == null)
            {
                return HttpNotFound();
            }
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureWant.CultureId);
            return View(cultureWant);
        }

        // POST: CultureWants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CultureId,Want,NeedType,Amount")] CultureWant cultureWant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cultureWant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureWant.CultureId);
            return View(cultureWant);
        }

        // GET: CultureWants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureWant cultureWant = db.CultureWants.Find(id);
            if (cultureWant == null)
            {
                return HttpNotFound();
            }
            return View(cultureWant);
        }

        // POST: CultureWants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CultureWant cultureWant = db.CultureWants.Find(id);
            db.CultureWants.Remove(cultureWant);
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
