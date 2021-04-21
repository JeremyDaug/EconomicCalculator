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
    public class SpeciesAnathemasController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: SpeciesAnathemas
        public ActionResult Index()
        {
            var speciesAnathemas = db.SpeciesAnathemas.Include(s => s.Anathema).Include(s => s.Species);
            return View(speciesAnathemas.ToList());
        }

        // GET: SpeciesAnathemas/Details/5
        public ActionResult Details(int? id, int? anathema)
        {
            if (id == null || anathema == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesAnathema speciesAnathema = db.SpeciesAnathemas
                .SingleOrDefault(x => x.SpeciesId == id && x.AnathemaId == anathema);
            if (speciesAnathema == null)
            {
                return HttpNotFound();
            }
            return View(speciesAnathema);
        }

        // GET: SpeciesAnathemas/Create
        public ActionResult Create()
        {
            ViewBag.AnathemaId = new SelectList(db.Products, "Id", "Name");
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name");
            return View();
        }

        // POST: SpeciesAnathemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SpeciesId,AnathemaId,Amount,Tag")] SpeciesAnathema speciesAnathema)
        {
            if (ModelState.IsValid)
            {
                db.SpeciesAnathemas.Add(speciesAnathema);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnathemaId = new SelectList(db.Products, "Id", "Name", speciesAnathema.AnathemaId);
            ViewBag.SpeciesId = new SelectList(db.Species, "Id", "Name", speciesAnathema.SpeciesId);
            return View(speciesAnathema);
        }

        // GET: SpeciesAnathemas/Delete/5
        public ActionResult Delete(int? id, int? anathema)
        {
            if (id == null || anathema == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpeciesAnathema speciesAnathema = db.SpeciesAnathemas
                .SingleOrDefault(x => x.SpeciesId == id && x.AnathemaId == anathema);
            if (speciesAnathema == null)
            {
                return HttpNotFound();
            }
            return View(speciesAnathema);
        }

        // POST: SpeciesAnathemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int anathema)
        {
            SpeciesAnathema speciesAnathema = db.SpeciesAnathemas
                .SingleOrDefault(x => x.SpeciesId == id && x.AnathemaId == anathema);
            db.SpeciesAnathemas.Remove(speciesAnathema);
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
