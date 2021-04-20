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
    public class CultureTagsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: CultureTags
        public ActionResult Index()
        {
            var cultureTags = db.CultureTags.Include(c => c.Culture);
            return View(cultureTags.ToList());
        }

        // GET: CultureTags/Details/5
        public ActionResult Details(int? cultureId, string tag)
        {
            if (cultureId == null || tag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureTag cultureTag = db.CultureTags.Single(x => x.CultureId == cultureId && x.Tag == tag);
            if (cultureTag == null)
            {
                return HttpNotFound();
            }
            return View(cultureTag);
        }

        // GET: CultureTags/Create
        public ActionResult Create()
        {
            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name");
            return View();
        }

        // POST: CultureTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CultureId,Tag")] CultureTag cultureTag)
        {
            if (ModelState.IsValid)
            {
                db.CultureTags.Add(cultureTag);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CultureId = new SelectList(db.Cultures, "Id", "Name", cultureTag.CultureId);
            return View(cultureTag);
        }

        // GET: CultureTags/Delete/5
        public ActionResult Delete(int? cultureId, string tag)
        {
            if (cultureId == null || tag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureTag cultureTag = db.CultureTags
                .SingleOrDefault(x => x.CultureId == cultureId && x.Tag == tag);
            if (cultureTag == null)
            {
                return HttpNotFound();
            }
            return View(cultureTag);
        }

        // POST: CultureTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int cultureId, string tag)
        {
            CultureTag cultureTag = db.CultureTags
                .SingleOrDefault(x => x.CultureId == cultureId && x.Tag == tag);

            db.CultureTags.Remove(cultureTag);
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
