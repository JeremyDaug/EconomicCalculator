using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EconModels;
using EconModels.ProcessModel;

namespace WebInterface.Controllers
{
    public class ProcessInputsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: ProcessInputs
        public ActionResult Index()
        {
            var processInputs = db.ProcessInputs.Include(p => p.Input).Include(p => p.Process);
            return View(processInputs.ToList());
        }

        // GET: ProcessInputs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessInput processInput = db.ProcessInputs.Find(id);
            if (processInput == null)
            {
                return HttpNotFound();
            }
            return View(processInput);
        }

        // GET: ProcessInputs/Create
        public ActionResult Create()
        {
            ViewBag.InputId = new SelectList(db.Products, "Id", "Name");
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name");
            return View();
        }

        // POST: ProcessInputs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProcessId,InputId,Amount,Tag")] ProcessInput processInput)
        {
            if (ModelState.IsValid)
            {
                db.ProcessInputs.Add(processInput);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InputId = new SelectList(db.Products, "Id", "Name", processInput.InputId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processInput.ProcessId);
            return View(processInput);
        }

        // GET: ProcessInputs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessInput processInput = db.ProcessInputs.Find(id);
            if (processInput == null)
            {
                return HttpNotFound();
            }
            ViewBag.InputId = new SelectList(db.Products, "Id", "Name", processInput.InputId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processInput.ProcessId);
            return View(processInput);
        }

        // POST: ProcessInputs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProcessId,InputId,Amount,Tag")] ProcessInput processInput)
        {
            if (ModelState.IsValid)
            {
                db.Entry(processInput).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InputId = new SelectList(db.Products, "Id", "Name", processInput.InputId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processInput.ProcessId);
            return View(processInput);
        }

        // GET: ProcessInputs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessInput processInput = db.ProcessInputs.Find(id);
            if (processInput == null)
            {
                return HttpNotFound();
            }
            return View(processInput);
        }

        // POST: ProcessInputs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProcessInput processInput = db.ProcessInputs.Find(id);
            db.ProcessInputs.Remove(processInput);
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
