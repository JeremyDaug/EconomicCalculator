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
    public class PoliticalBreakdownsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: PoliticalBreakdowns
        public ActionResult Index()
        {
            var popPoliticalBreakdowns = db.PopPoliticalBreakdowns
                .Include(p => p.Parent)
                .Include("Parent.Territory")
                .Include("Parent.PrimaryJob")
                .Include(p => p.PoliticalGroup);
            return View(popPoliticalBreakdowns.ToList());
        }

        // GET: PoliticalBreakdowns/Details/5
        public ActionResult Details(int? parentId, int? partyId)
        {
            if (parentId == null || partyId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalBreakdown politicalBreakdown = db.PopPoliticalBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.PoliticalGroupId == partyId);
            if (politicalBreakdown == null)
            {
                return HttpNotFound();
            }
            return View(politicalBreakdown);
        }

        // GET: PoliticalBreakdowns/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name");
            ViewBag.PoliticalGroupId = new SelectList(db.PoliticalGroups, "Id", "Name");
            return View();
        }

        // POST: PoliticalBreakdowns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentId,PoliticalGroupId,Percent")] PoliticalBreakdown politicalBreakdown)
        {
            if (ModelState.IsValid)
            {
                // get target percent
                var target = politicalBreakdown.Percent;

                // get parent
                var parent = db.PopulationGroups
                    .Single(x => x.Id == politicalBreakdown.ParentId);

                // get party
                var party = db.PoliticalGroups
                    .Single(x => x.Id == politicalBreakdown.PoliticalGroupId);

                // add to list
                parent.SetPartyPercent(party, target);

                // update others
                foreach (var part in parent.PoliticalBreakdown.Where(x => x.PoliticalGroupId != party.Id))
                {
                    db.Entry(part).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", politicalBreakdown.ParentId);
            ViewBag.PoliticalGroupId = new SelectList(db.PoliticalGroups, "Id", "Name", politicalBreakdown.PoliticalGroupId);
            return View(politicalBreakdown);
        }

        // GET: PoliticalBreakdowns/Edit/5
        public ActionResult Edit(int? parentId, int? partyId)
        {
            if (parentId == null || partyId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalBreakdown politicalBreakdown = db.PopPoliticalBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.PoliticalGroupId == partyId);
            if (politicalBreakdown == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", politicalBreakdown.ParentId);
            ViewBag.PoliticalGroupId = new SelectList(db.PoliticalGroups, "Id", "Name", politicalBreakdown.PoliticalGroupId);
            return View(politicalBreakdown);
        }

        // POST: PoliticalBreakdowns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParentId,PoliticalGroupId,Percent")] PoliticalBreakdown politicalBreakdown)
        {
            if (ModelState.IsValid)
            {
                // get target percent
                var target = politicalBreakdown.Percent;

                // parent
                var old = db.PopulationGroups
                    .Single(x => x.Id == politicalBreakdown.ParentId);

                // get party
                var party = db.PoliticalGroups
                    .Single(x => x.Id == politicalBreakdown.PoliticalGroupId);

                // change percents
                old.SetPartyPercent(party, target);

                // update all other groups
                foreach (var par in old.PoliticalBreakdown)
                {
                    db.Entry(par).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", politicalBreakdown.ParentId);
            ViewBag.PoliticalGroupId = new SelectList(db.PoliticalGroups, "Id", "Name", politicalBreakdown.PoliticalGroupId);
            return View(politicalBreakdown);
        }

        // GET: PoliticalBreakdowns/Delete/5
        public ActionResult Delete(int? parentId, int? partyId)
        {
            if (parentId == null || partyId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalBreakdown politicalBreakdown = db.PopPoliticalBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.PoliticalGroupId == partyId);
            if (politicalBreakdown == null)
            {
                return HttpNotFound();
            }
            return View(politicalBreakdown);
        }

        // POST: PoliticalBreakdowns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int parentId, int partyId)
        {
            PoliticalBreakdown politicalBreakdown = db.PopPoliticalBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.PoliticalGroupId == partyId);
            // Get parent before getting to far.
            var pop = db.PopulationGroups.Single(x => x.Id == parentId);

            db.PopPoliticalBreakdowns.Remove(politicalBreakdown);

            // after deletion normalize percent
            pop.NormalizePolitics();

            // mark remaining parts as modified
            foreach (var pol in pop.PoliticalBreakdown)
            {
                db.Entry(pol).State = EntityState.Modified;
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
