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
    public class SpeciesTagsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: SpeciesTags
        public ActionResult Index()
        {
            var speciesTags = db.SpeciesTags.Include(s => s.Species);
            return View(speciesTags.ToList());
        }

        // GET: SpeciesTags/Details/5
        public ActionResult Details(int? id, string tag)
        {
            if (id == null || tag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesTag speciesTag = db.SpeciesTags
                .SingleOrDefault(x => x.SpeciesId == id && x.Tag == tag);
            if (speciesTag == null)
            {
                return HttpNotFound();
            }
            return View(speciesTag);
        }

        // GET: SpeciesTags/Create
        public ActionResult Create()
        {
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name");
            return View();
        }

        // POST: SpeciesTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SpeciesId,Tag")] SpeciesTag speciesTag)
        {
            if (ModelState.IsValid)
            {
                db.SpeciesTags.Add(speciesTag);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesTag.SpeciesId);
            return View(speciesTag);
        }

        // GET: SpeciesTags/Delete/5
        public ActionResult Delete(int? id, string tag)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesTag speciesTag = db.SpeciesTags
                .SingleOrDefault(x => x.SpeciesId == id && x.Tag == tag);
            if (speciesTag == null)
            {
                return HttpNotFound();
            }
            return View(speciesTag);
        }

        // POST: SpeciesTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string tag)
        {
            SpeciesTag speciesTag = db.SpeciesTags
                .SingleOrDefault(x => x.SpeciesId == id && x.Tag == tag);
            db.SpeciesTags.Remove(speciesTag);
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
