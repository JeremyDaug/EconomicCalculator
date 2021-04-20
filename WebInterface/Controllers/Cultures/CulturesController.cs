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
    public class CulturesController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: Cultures
        public ActionResult Index()
        {
            return View(db.Cultures.ToList());
        }

        // GET: Cultures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Culture culture = db.Cultures.Find(id);
            if (culture == null)
            {
                return HttpNotFound();
            }
            return View(culture);
        }

        // GET: Cultures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cultures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,VariantName,CultureGrowthRate")] Culture culture)
        {
            if (ModelState.IsValid)
            {
                db.Cultures.Add(culture);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(culture);
        }

        // GET: Cultures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Culture culture = db.Cultures.Find(id);
            if (culture == null)
            {
                return HttpNotFound();
            }
            return View(culture);
        }

        // POST: Cultures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,VariantName,CultureGrowthRate")] Culture culture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(culture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(culture);
        }

        // GET: Cultures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Culture culture = db.Cultures.Find(id);
            if (culture == null)
            {
                return HttpNotFound();
            }
            return View(culture);
        }

        // POST: Cultures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Culture culture = db.Cultures.Find(id);
            db.Cultures.Remove(culture);
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
