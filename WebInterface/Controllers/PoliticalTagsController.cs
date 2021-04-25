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
    public class PoliticalTagsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: PoliticalTags
        public ActionResult Index()
        {
            var politicalTags = db.PoliticalTags.Include(p => p.Group);
            return View(politicalTags.ToList());
        }

        // GET: PoliticalTags/Details/5
        public ActionResult Details(int? id, string tag)
        {
            if (id == null || tag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalTag politicalTag = db.PoliticalTags
                .SingleOrDefault(x => x.GroupId == id && x.Tag == tag);
            if (politicalTag == null)
            {
                return HttpNotFound();
            }
            return View(politicalTag);
        }

        // GET: PoliticalTags/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(db.PoliticalGroups, "Id", "Name");
            return View();
        }

        // POST: PoliticalTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,Tag")] PoliticalTag politicalTag)
        {
            if (ModelState.IsValid)
            {
                db.PoliticalTags.Add(politicalTag);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(db.PoliticalGroups, "Id", "Name", politicalTag.GroupId);
            return View(politicalTag);
        }

        // GET: PoliticalTags/Delete/5
        public ActionResult Delete(int? id, string tag)
        {
            if (id == null || tag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalTag politicalTag = db.PoliticalTags
                .SingleOrDefault(x => x.GroupId == id && x.Tag == tag);
            if (politicalTag == null)
            {
                return HttpNotFound();
            }
            return View(politicalTag);
        }

        // POST: PoliticalTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string tag)
        {
            PoliticalTag politicalTag = db.PoliticalTags
                .SingleOrDefault(x => x.GroupId == id && x.Tag == tag);
            db.PoliticalTags.Remove(politicalTag);
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
