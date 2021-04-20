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
    public class SpeciesWantsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: SpeciesWants
        public ActionResult Index()
        {
            var speciesWants = db.SpeciesWants.Include(s => s.Species);
            return View(speciesWants.ToList());
        }

        // GET: SpeciesWants/Details/5
        public ActionResult Details(int? id, string want)
        {
            if (id == null || want == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesWant speciesWant = db.SpeciesWants
                .SingleOrDefault(x => x.SpeciesId == id && x.Want == want);
            if (speciesWant == null)
            {
                return HttpNotFound();
            }
            return View(speciesWant);
        }

        // GET: SpeciesWants/Create
        public ActionResult Create()
        {
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name");
            return View();
        }

        // POST: SpeciesWants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SpeciesId,Want,Amount,Tag")] SpeciesWant speciesWant)
        {
            if (ModelState.IsValid)
            {
                db.SpeciesWants.Add(speciesWant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesWant.SpeciesId);
            return View(speciesWant);
        }

        // GET: SpeciesWants/Delete/5
        public ActionResult Delete(int? id, string want)
        {
            if (id == null || want == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesWant speciesWant = db.SpeciesWants
                .SingleOrDefault(x => x.SpeciesId == id && x.Want == want);
            if (speciesWant == null)
            {
                return HttpNotFound();
            }
            return View(speciesWant);
        }

        // POST: SpeciesWants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string want)
        {
            SpeciesWant speciesWant = db.SpeciesWants
                .SingleOrDefault(x => x.SpeciesId == id && x.Want == want);
            db.SpeciesWants.Remove(speciesWant);
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
