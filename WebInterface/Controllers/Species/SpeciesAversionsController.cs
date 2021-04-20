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

namespace WebInterface.Controllers
{
    public class SpeciesAversionsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: SpeciesAversions
        public ActionResult Index()
        {
            var speciesAversions = db.SpeciesAversions.Include(s => s.Species);
            return View(speciesAversions.ToList());
        }

        // GET: SpeciesAversions/Details/5
        public ActionResult Details(int? id, string aversion)
        {
            if (id == null || aversion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesAversion speciesAversion = db.SpeciesAversions
                .SingleOrDefault(x => x.SpeciesId == id && x.Aversion == aversion);
            if (speciesAversion == null)
            {
                return HttpNotFound();
            }
            return View(speciesAversion);
        }

        // GET: SpeciesAversions/Create
        public ActionResult Create()
        {
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name");
            return View();
        }

        // POST: SpeciesAversions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SpeciesId,Aversion,Amount,Tag")] SpeciesAversion speciesAversion)
        {
            if (ModelState.IsValid)
            {
                db.SpeciesAversions.Add(speciesAversion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesAversion.SpeciesId);
            return View(speciesAversion);
        }

        // GET: SpeciesAversions/Edit/5
        public ActionResult Edit(int? id, string aversion)
        {
            if (id == null || aversion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesAversion speciesAversion = db.SpeciesAversions
                .SingleOrDefault(x => x.SpeciesId == id && x.Aversion == aversion);
            if (speciesAversion == null)
            {
                return HttpNotFound();
            }
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesAversion.SpeciesId);
            return View(speciesAversion);
        }

        // POST: SpeciesAversions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SpeciesId,Aversion,Amount,Tag")] SpeciesAversion speciesAversion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(speciesAversion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesAversion.SpeciesId);
            return View(speciesAversion);
        }

        // GET: SpeciesAversions/Delete/5
        public ActionResult Delete(int? id, string aversion)
        {
            if (id == null || aversion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesAversion speciesAversion = db.SpeciesAversions
                .SingleOrDefault(x => x.SpeciesId == id && x.Aversion == aversion);
            if (speciesAversion == null)
            {
                return HttpNotFound();
            }
            return View(speciesAversion);
        }

        // POST: SpeciesAversions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string aversion)
        {
            SpeciesAversion speciesAversion = db.SpeciesAversions
                .SingleOrDefault(x => x.SpeciesId == id && x.Aversion == aversion);
            db.SpeciesAversions.Remove(speciesAversion);
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
