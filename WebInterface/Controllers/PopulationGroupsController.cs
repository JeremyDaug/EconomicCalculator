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
    public class PopulationGroupsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: PopulationGroups
        public ActionResult Index()
        {
            var populationGroups = db.PopulationGroups
                .Include(p => p.CultureBreakdown)
                .Include(p => p.SpeciesBreakdown)
                .Include(p => p.PoliticalBreakdown)
                .Include(p => p.PrimaryJob)
                .Include(p => p.Territory);
            return View(populationGroups.ToList());
        }

        // GET: PopulationGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PopulationGroup populationGroup = db.PopulationGroups.Find(id);
            if (populationGroup == null)
            {
                return HttpNotFound();
            }
            return View(populationGroup);
        }

        // GET: PopulationGroups/Create
        public ActionResult Create()
        {
            ViewBag.PrimaryJobId = new SelectList(db.Jobs, "Id", "Name");
            ViewBag.TerritoryId = new SelectList(db.Territories, "Id", "Name");
            return View();
        }

        // POST: PopulationGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TerritoryId,Infants,Children,Adults,Seniors,SkillLevel,PrimaryJobId,Priority")] PopulationGroup populationGroup)
        {
            if (ModelState.IsValid)
            {
                db.PopulationGroups.Add(populationGroup);
                db.SaveChanges();

                // Population Groups default to 100% of some (usually the first)
                // Party, Species, and Culture by default to ensure they have
                // something.

                // get the new pop group.
                var pop = db.PopulationGroups
                    .Single(x => x.TerritoryId == populationGroup.TerritoryId &&
                                 x.PrimaryJobId == populationGroup.PrimaryJobId);

                // Set Default Species
                var species = new SpeciesBreakdown
                {
                    ParentId = pop.Id,
                    SpeciesId = db.Species.First().Id,
                    Percent = 1
                };

                db.PopSpeciesBreakdowns.Add(species);

                // set default Culture
                var culture = new CultureBreakdown
                {
                    ParentId = pop.Id,
                    CultureId = db.Cultures.First().Id,
                    Percent = 1
                };

                db.PopCultureBreakdowns.Add(culture);

                // set Default Political Group
                var party = new PoliticalBreakdown
                {
                    ParentId = pop.Id,
                    PoliticalGroupId = db.PoliticalGroups.First().Id,
                    Percent = 1
                };

                db.PopPoliticalBreakdowns.Add(party);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PrimaryJobId = new SelectList(db.Jobs, "Id", "Name", populationGroup.PrimaryJobId);
            ViewBag.TerritoryId = new SelectList(db.Territories, "Id", "Name", populationGroup.TerritoryId);
            return View(populationGroup);
        }

        // GET: PopulationGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PopulationGroup populationGroup = db.PopulationGroups.Find(id);
            if (populationGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.PrimaryJobId = new SelectList(db.Jobs, "Id", "Name", populationGroup.PrimaryJobId);
            ViewBag.TerritoryId = new SelectList(db.Territories, "Id", "Name", populationGroup.TerritoryId);
            return View(populationGroup);
        }

        // POST: PopulationGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TerritoryId,Infants,Children,Adults,Seniors,SkillLevel,PrimaryJobId,Priority")] PopulationGroup populationGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(populationGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PrimaryJobId = new SelectList(db.Jobs, "Id", "Name", populationGroup.PrimaryJobId);
            ViewBag.TerritoryId = new SelectList(db.Territories, "Id", "Name", populationGroup.TerritoryId);
            return View(populationGroup);
        }

        // GET: PopulationGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PopulationGroup populationGroup = db.PopulationGroups.Find(id);
            if (populationGroup == null)
            {
                return HttpNotFound();
            }
            return View(populationGroup);
        }

        // POST: PopulationGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PopulationGroup populationGroup = db.PopulationGroups.Find(id);
            db.PopulationGroups.Remove(populationGroup);
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
