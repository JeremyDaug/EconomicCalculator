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
    public class SpeciesNeedsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: SpeciesNeeds
        public ActionResult Index()
        {
            var speciesNeeds = db.SpeciesNeeds.Include(s => s.Need).Include(s => s.Species);
            return View(speciesNeeds.ToList());
        }

        // GET: SpeciesNeeds/Details/5
        public ActionResult Details(int? id, int? need)
        {
            if (id == null || need == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesNeed speciesNeed = db.SpeciesNeeds
                .SingleOrDefault(x => x.SpeciesId == id && x.NeedId == need);
            if (speciesNeed == null)
            {
                return HttpNotFound();
            }
            return View(speciesNeed);
        }

        // GET: SpeciesNeeds/Create
        public ActionResult Create()
        {
            ViewBag.NeedId = new SelectList(db.Products, "Id", "Name");
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name");
            return View();
        }

        // POST: SpeciesNeeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SpeciesId,NeedId,Amount,Tag")] SpeciesNeed speciesNeed)
        {
            if (ModelState.IsValid)
            {
                db.SpeciesNeeds.Add(speciesNeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NeedId = new SelectList(db.Products, "Id", "Name", speciesNeed.NeedId);
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesNeed.SpeciesId);
            return View(speciesNeed);
        }
        
        // GET: SpeciesNeeds/Delete/5
        public ActionResult Delete(int? id, int? need)
        {
            if (id == null || need == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesNeed speciesNeed = db.SpeciesNeeds
                .SingleOrDefault(x => x.SpeciesId == id && x.NeedId == need);
            if (speciesNeed == null)
            {
                return HttpNotFound();
            }
            return View(speciesNeed);
        }

        // POST: SpeciesNeeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int need)
        {
            SpeciesNeed speciesNeed = db.SpeciesNeeds
                .SingleOrDefault(x => x.SpeciesId == id && x.NeedId == need);
            db.SpeciesNeeds.Remove(speciesNeed);
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
