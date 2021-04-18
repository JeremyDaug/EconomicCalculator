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
    public class CultureNeedsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: CultureNeeds
        public ActionResult Index()
        {
            var cultureNeeds = db.CultureNeeds.Include(c => c.Culture).Include(c => c.Need);
            return View(cultureNeeds.ToList());
        }

        // GET: CultureNeeds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureNeed cultureNeed = db.CultureNeeds.Find(id);
            if (cultureNeed == null)
            {
                return HttpNotFound();
            }
            return View(cultureNeed);
        }

        // GET: CultureNeeds/Create
        public ActionResult Create()
        {
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name");
            ViewBag.NeedId = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: CultureNeeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CultureId,NeedId,NeedType,Amount")] CultureNeed cultureNeed)
        {
            if (ModelState.IsValid)
            {
                db.CultureNeeds.Add(cultureNeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureNeed.CultureId);
            ViewBag.NeedId = new SelectList(db.Products, "Id", "Name", cultureNeed.NeedId);
            return View(cultureNeed);
        }

        // GET: CultureNeeds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureNeed cultureNeed = db.CultureNeeds.Find(id);
            if (cultureNeed == null)
            {
                return HttpNotFound();
            }
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureNeed.CultureId);
            ViewBag.NeedId = new SelectList(db.Products, "Id", "Name", cultureNeed.NeedId);
            return View(cultureNeed);
        }

        // POST: CultureNeeds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CultureId,NeedId,NeedType,Amount")] CultureNeed cultureNeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cultureNeed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureNeed.CultureId);
            ViewBag.NeedId = new SelectList(db.Products, "Id", "Name", cultureNeed.NeedId);
            return View(cultureNeed);
        }

        // GET: CultureNeeds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureNeed cultureNeed = db.CultureNeeds.Find(id);
            if (cultureNeed == null)
            {
                return HttpNotFound();
            }
            return View(cultureNeed);
        }

        // POST: CultureNeeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CultureNeed cultureNeed = db.CultureNeeds.Find(id);
            db.CultureNeeds.Remove(cultureNeed);
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
