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
    public class SpeciesBreakdownsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: SpeciesBreakdowns
        public ActionResult Index()
        {
            var popSpeciesBreakdowns = db.PopSpeciesBreakdowns
                .Include(s => s.Parent)
                .Include("Parent.Territory")
                .Include("Parent.PrimaryJob")
                .Include(s => s.Species);
            return View(popSpeciesBreakdowns.ToList());
        }

        // GET: SpeciesBreakdowns/Details/5
        public ActionResult Details(int? parentId, int? speciesId)
        {
            if (parentId == null || speciesId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesBreakdown speciesBreakdown = db.PopSpeciesBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.SpeciesId == speciesId);
            if (speciesBreakdown == null)
            {
                return HttpNotFound();
            }
            return View(speciesBreakdown);
        }

        // GET: SpeciesBreakdowns/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Id");
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name");
            return View();
        }

        // GET: SpeciesBreakdowns/Create/3
        public ActionResult CreateForParent(int parentId)
        {
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Id");
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name");
            return View();
        }

        // POST: SpeciesBreakdowns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentId,SpeciesId,Percent")] SpeciesBreakdown speciesBreakdown)
        {
            if (ModelState.IsValid)
            {
                // update ratios
                var target = speciesBreakdown.Percent;

                // ensure the old is gotten.
                var old = db.PopulationGroups
                    .Single(x => x.Id == speciesBreakdown.ParentId);

                var spe = db.Species.Single(x => x.Id == speciesBreakdown.SpeciesId);
                // set percents
                old.SetSpeciesPercent(spe, target);

                foreach (var conn in old.SpeciesBreakdown.Where(x => x.SpeciesId != spe.Id))
                {
                    db.Entry(conn).State = EntityState.Modified;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", speciesBreakdown.ParentId);
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesBreakdown.SpeciesId);
            return View(speciesBreakdown);
        }

        // GET: SpeciesBreakdowns/Edit/5
        public ActionResult Edit(int? parentId, int? speciesId)
        {
            if (parentId == null || speciesId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesBreakdown speciesBreakdown = db.PopSpeciesBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.SpeciesId == speciesId);
            if (speciesBreakdown == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", speciesBreakdown.ParentId);
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesBreakdown.SpeciesId);
            return View(speciesBreakdown);
        }

        // POST: SpeciesBreakdowns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParentId,SpeciesId,Percent")] SpeciesBreakdown speciesBreakdown)
        {
            if (ModelState.IsValid)
            {
                // update ratios
                var target = speciesBreakdown.Percent;

                // ensure the old is gotten.
                var old = db.PopulationGroups
                    .Single(x => x.Id == speciesBreakdown.ParentId);

                var spe = db.Species.Single(x => x.Id == speciesBreakdown.SpeciesId);

                old.SetSpeciesPercent(speciesBreakdown.Species, target);

                foreach (var conn in old.SpeciesBreakdown)
                {
                    db.Entry(conn).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", speciesBreakdown.ParentId);
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesBreakdown.SpeciesId);
            return View(speciesBreakdown);
        }

        // GET: SpeciesBreakdowns/Delete/5
        public ActionResult Delete(int? parentId, int? speciesId)
        {
            if (parentId == null && speciesId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesBreakdown speciesBreakdown = db.PopSpeciesBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.SpeciesId == speciesId);
            if (speciesBreakdown == null)
            {
                return HttpNotFound();
            }
            return View(speciesBreakdown);
        }

        // POST: SpeciesBreakdowns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int parentId, int speciesId)
        {
            SpeciesBreakdown speciesBreakdown = db.PopSpeciesBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.SpeciesId == speciesId);
            db.PopSpeciesBreakdowns.Remove(speciesBreakdown);

            var parent = db.PopulationGroups
                .Single(x => x.Id == speciesBreakdown.ParentId);

            // after deletion normalize percents to 100 percent splits.
            parent.NormalizeSpecies();

            // Mark all remaining sections as modified so they are updated in the DB.
            foreach (var spe in parent.SpeciesBreakdown)
            {
                db.Entry(spe).State = EntityState.Modified;
            }

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
