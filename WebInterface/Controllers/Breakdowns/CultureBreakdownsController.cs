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
    public class CultureBreakdownsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: CultureBreakdowns
        public ActionResult Index()
        {
            var popCultureBreakdowns = db.PopCultureBreakdowns
                .Include(c => c.Culture)
                .Include(c => c.Parent)
                .Include("Parent.Territory");
            return View(popCultureBreakdowns.ToList());
        }

        // GET: CultureBreakdowns/Details/5
        public ActionResult Details(int? parentId, int? cultureId)
        {
            if (parentId == null || cultureId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureBreakdown cultureBreakdown = db.PopCultureBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.CultureId == cultureId);
            if (cultureBreakdown == null)
            {
                return HttpNotFound();
            }
            return View(cultureBreakdown);
        }

        // GET: CultureBreakdowns/Create
        public ActionResult Create()
        {
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name");
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name");
            return View();
        }

        // POST: CultureBreakdowns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentId,CultureId,Percent")] CultureBreakdown cultureBreakdown)
        {
            if (ModelState.IsValid)
            {
                // get target
                var target = cultureBreakdown.Percent;

                // get parent
                var parent = db.PopulationGroups
                    .Single(x => x.Id == cultureBreakdown.ParentId);

                // get culture target
                var culture = db.Cultures
                    .Single(x => x.Id == cultureBreakdown.CultureId);

                // add new culture
                parent.SetCulturePercent(culture, target);

                // update other culture percents
                foreach (var cult in parent.CultureBreakdown.Where(x => x.CultureId != culture.Id))
                {
                    db.Entry(cult).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureBreakdown.CultureId);
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", cultureBreakdown.ParentId);
            return View(cultureBreakdown);
        }

        // GET: CultureBreakdowns/Edit/5
        public ActionResult Edit(int? parentId, int? cultureId)
        {
            if (parentId == null || cultureId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureBreakdown cultureBreakdown = db.PopCultureBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.CultureId == cultureId);
            if (cultureBreakdown == null)
            {
                return HttpNotFound();
            }
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureBreakdown.CultureId);
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", cultureBreakdown.ParentId);
            return View(cultureBreakdown);
        }

        // POST: CultureBreakdowns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParentId,CultureId,Percent")] CultureBreakdown cultureBreakdown)
        {
            if (ModelState.IsValid)
            {
                // update ratios
                var target = cultureBreakdown.Percent;

                // get parent
                var parent = db.PopulationGroups
                    .Single(x => x.Id == cultureBreakdown.ParentId);

                // get culture
                var culture = db.Cultures
                    .Single(x => x.Id == cultureBreakdown.CultureId);

                parent.SetCulturePercent(culture, target);

                foreach (var cult in parent.CultureBreakdown)
                {
                    db.Entry(cult).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureBreakdown.CultureId);
            ViewBag.ParentId = new SelectList(db.PopulationGroups, "Id", "Name", cultureBreakdown.ParentId);
            return View(cultureBreakdown);
        }

        // GET: CultureBreakdowns/Delete/5
        public ActionResult Delete(int? parentId, int? cultureId)
        {
            if (parentId == null || cultureId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureBreakdown cultureBreakdown = db.PopCultureBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.CultureId == cultureId);
            if (cultureBreakdown == null)
            {
                return HttpNotFound();
            }
            return View(cultureBreakdown);
        }

        // POST: CultureBreakdowns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int parentId, int cultureId)
        {
            CultureBreakdown cultureBreakdown = db.PopCultureBreakdowns
                .SingleOrDefault(x => x.ParentId == parentId && x.CultureId == cultureId);
            db.PopCultureBreakdowns.Remove(cultureBreakdown);

            // get parent
            var parent = db.PopulationGroups
                .Single(x => x.Id == parentId);

            // normalize remaining cultures
            parent.NormalizeCultures();

            // mark remaining as updated
            foreach (var cult in parent.CultureBreakdown)
            {
                db.Entry(cult).State = EntityState.Modified;
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
