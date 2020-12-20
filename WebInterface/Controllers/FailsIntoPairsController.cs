using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EconModels;
using EconModels.ProductModel;

namespace WebInterface.Controllers
{
    public class FailsIntoPairsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: FailsIntoPairs
        public ActionResult Index()
        {
            var failurePairs = db.FailurePairs.Include(f => f.Result).Include(f => f.Source);
            return View(failurePairs.ToList());
        }

        // GET: FailsIntoPairs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailsIntoPair failsIntoPair = db.FailurePairs.Find(id);
            if (failsIntoPair == null)
            {
                return HttpNotFound();
            }
            return View(failsIntoPair);
        }

        // GET: FailsIntoPairs/Create
        public ActionResult Create()
        {
            ViewBag.ResultId = new SelectList(db.Products, "Id", "Name");
            ViewBag.SourceId = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: FailsIntoPairs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SourceId,ResultId,Amount")] FailsIntoPair failsIntoPair)
        {
            if (ModelState.IsValid)
            {
                db.FailurePairs.Add(failsIntoPair);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ResultId = new SelectList(db.Products, "Id", "Name", failsIntoPair.ResultId);
            ViewBag.SourceId = new SelectList(db.Products, "Id", "Name", failsIntoPair.SourceId);
            return View(failsIntoPair);
        }

        // GET: FailsIntoPairs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailsIntoPair failsIntoPair = db.FailurePairs.Find(id);
            if (failsIntoPair == null)
            {
                return HttpNotFound();
            }
            ViewBag.ResultId = new SelectList(db.Products, "Id", "Name", failsIntoPair.ResultId);
            ViewBag.SourceId = new SelectList(db.Products, "Id", "Name", failsIntoPair.SourceId);
            return View(failsIntoPair);
        }

        // POST: FailsIntoPairs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SourceId,ResultId,Amount")] FailsIntoPair failsIntoPair)
        {
            if (ModelState.IsValid)
            {
                db.Entry(failsIntoPair).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ResultId = new SelectList(db.Products, "Id", "Name", failsIntoPair.ResultId);
            ViewBag.SourceId = new SelectList(db.Products, "Id", "Name", failsIntoPair.SourceId);
            return View(failsIntoPair);
        }

        // GET: FailsIntoPairs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailsIntoPair failsIntoPair = db.FailurePairs.Find(id);
            if (failsIntoPair == null)
            {
                return HttpNotFound();
            }
            return View(failsIntoPair);
        }

        // POST: FailsIntoPairs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FailsIntoPair failsIntoPair = db.FailurePairs.Find(id);
            db.FailurePairs.Remove(failsIntoPair);
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
