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
    public class SpeciesController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: Species
        public ActionResult Index()
        {
            var species = db.Species
                .Include(j => j.Aversions)
                .Include(j => j.Anathemas)
                .Include("Anathemas.Anathema")
                .Include(j => j.LifeNeeds)
                .Include("LifeNeeds.Need")
                .Include(j => j.LifeWants)
                .Include(j => j.Tags);
            return View(species.ToList());
        }

        // GET: Species/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Species species = db.Species
                .Include(x => x.LifeNeeds)
                .Include("LifeNeeds.Need")
                .Include(x => x.Anathemas)
                .Include("Anathemas.Anathema")
                .SingleOrDefault(x => x.Id == id);
            if (species == null)
            {
                return HttpNotFound();
            }
            
            return View(species);
        }

        // GET: Species/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Species/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,VariantName,SpeciesGrowthRate,TempuraturePreference,GravityPreference,InfantPhaseLength,ChildPhaseLength,AdultPhaseLength,AverageLifeSpan")] Species species)
        {
            if (ModelState.IsValid)
            {
                db.Species.Add(species);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(species);
        }

        // GET: Species/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Species species = db.Species
                .SingleOrDefault(x => x.Id == id);
            if (species == null)
            {
                return HttpNotFound();
            }
            return View(species);
        }

        // POST: Species/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,VariantName,SpeciesGrowthRate,TempuraturePreference,GravityPreference,InfantPhaseLength,ChildPhaseLength,AdultPhaseLength,AverageLifeSpan")] Species species)
        {
            if (ModelState.IsValid)
            {
                db.Entry(species).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(species);
        }

        // GET: Species/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Species species = db.Species
                .SingleOrDefault(x => x.Id == id);
            if (species == null)
            {
                return HttpNotFound();
            }
            return View(species);
        }

        // POST: Species/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Species species = db.Species
                .Include(x => x.LifeNeeds)
                .Include("LifeNeeds.Need")
                .Include(x => x.Anathemas)
                .Include("Anathemas.Anathema")
                .SingleOrDefault(x => x.Id == id);
            db.Species.Remove(species);
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
