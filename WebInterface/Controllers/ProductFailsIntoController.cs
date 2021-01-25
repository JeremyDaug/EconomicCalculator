using EconModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EconModels.ProductModel;
using System.Data.Entity;
using System.Net;

namespace WebInterface.Controllers
{
    public class ProductFailsIntoController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: ProductFailsInto/Index
        public ActionResult Index()
        {
            var failurePairs = db.FailurePairs.Include(f => f.Result)
                .Include(x => x.Source);
            return View(failurePairs.ToList());
        }

        // GET: SourceFailsInto
        public ActionResult SourceFailsInto(int? sourceId)
        {
            if (sourceId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var failurePairs = db.FailurePairs.Where(x => x.SourceId == sourceId);

            return View(failurePairs.ToList());
        }

        // GET: FailsIntoResult
        public ActionResult FailsIntoResult(int? resultId)
        {
            if (resultId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var failIntoResult = db.FailurePairs.Where(x => x.ResultId == resultId).ToList();

            return View(failIntoResult);
        }

        // GET: FailsIntoPairs/Details/5
        public ActionResult Details(int? sourceId, int? resultId)
        {
            if (sourceId == null || resultId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailsIntoPair failsIntoPair;
            try
            {
                failsIntoPair = db.FailurePairs
                    .Single(x => x.SourceId == sourceId && x.ResultId == resultId);
            }
            catch (InvalidOperationException)
            {
                return HttpNotFound();
            }
            return View(failsIntoPair);
        }

        // GET: AllFailureResults
        public ActionResult AllFailureResults()
        {
            var AllPairs = db.FailurePairs.Include(x => x.Result)
                .Include(x => x.Source);
            return View(AllPairs.ToList());
        }

        // GET: Add Failure Pair For Source
        public ActionResult CreateFailureWithSource(int? sourceId)
        {
            if (sourceId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currentPairs = db.FailurePairs.Where(x => x.SourceId == sourceId);

            ViewBag.CurrentPairs = currentPairs.ToList();
            ViewBag.DefaultSource = db.Products.Single(x => x.Id == sourceId);
            ViewBag.SourceId = new SelectList(db.Products, "Id", "Name");
            ViewBag.ResultId = new SelectList(db.Products, "Id", "Name");

            return View();
        }

        // POST: FailsIntoPairs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFailureWithSource([Bind(Include = "SourceId,ResultId,Amount")] FailsIntoPair failsIntoPair)
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
        public ActionResult Create([Bind(Include = "SourceId,ResultId,Amount")] FailsIntoPair failsIntoPair)
        {
            if (ModelState.IsValid)
            {
                db.FailurePairs.Add(failsIntoPair);

                // if it already exists
                if (db.FailurePairs.Any(x => x.SourceId == failsIntoPair.SourceId 
                    && x.ResultId == failsIntoPair.ResultId))
                {
                    // return a conflict from the DB
                    // TODO: Improve this so when it happens people know why.
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ResultId = new SelectList(db.Products, "Id", "Name", failsIntoPair.ResultId);
            ViewBag.SourceId = new SelectList(db.Products, "Id", "Name", failsIntoPair.SourceId);
            return View(failsIntoPair);
        }

        // GET: FailsIntoPairs/Edit/5
        public ActionResult Edit(int? sourceId, int? resultId)
        {
            if (sourceId == null || resultId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailsIntoPair failsIntoPair = db.FailurePairs.Single(x => x.SourceId == sourceId && x.ResultId == resultId);

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
        public ActionResult Edit([Bind(Include = "SourceId,ResultId,Amount")] FailsIntoPair failsIntoPair)
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
        public ActionResult Delete(int? sourceId, int? resultId)
        {
            if (sourceId == null || resultId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailsIntoPair failsIntoPair = db.FailurePairs
                .Single(x => x.SourceId == sourceId && x.ResultId == resultId);

            if (failsIntoPair == null)
            {
                return HttpNotFound();
            }
            return View(failsIntoPair);
        }

        // POST: FailsIntoPairs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? sourceId, int? resultId)
        {
            if (sourceId == null || resultId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FailsIntoPair failsIntoPair = db.FailurePairs
                .Single(x => x.SourceId == sourceId && x.ResultId == resultId);
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