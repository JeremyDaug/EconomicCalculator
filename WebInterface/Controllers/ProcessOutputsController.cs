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
    public class ProcessOutputsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: ProcessOutputs
        public ActionResult Index()
        {
            var processOutputs = db.ProcessOutputs.Include(p => p.Output).Include(p => p.Process);
            return View(processOutputs.ToList());
        }

        // GET: ProcessOutputs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessOutput processOutput = db.ProcessOutputs.Find(id);
            if (processOutput == null)
            {
                return HttpNotFound();
            }
            return View(processOutput);
        }

        // GET: ProcessOutputs/Create
        public ActionResult Create()
        {
            ViewBag.OutputId = new SelectList(db.Products, "Id", "Name");
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name");
            return View();
        }

        // POST: ProcessOutputs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProcessId,OutputId,Amount,Tag")] ProcessOutput processOutput)
        {
            if (ModelState.IsValid)
            {
                db.ProcessOutputs.Add(processOutput);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OutputId = new SelectList(db.Products, "Id", "Name", processOutput.OutputId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processOutput.ProcessId);
            return View(processOutput);
        }

        // GET: ProcessOutputs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessOutput processOutput = db.ProcessOutputs.Find(id);
            if (processOutput == null)
            {
                return HttpNotFound();
            }
            ViewBag.OutputId = new SelectList(db.Products, "Id", "Name", processOutput.OutputId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processOutput.ProcessId);
            return View(processOutput);
        }

        // POST: ProcessOutputs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProcessId,OutputId,Amount,Tag")] ProcessOutput processOutput)
        {
            if (ModelState.IsValid)
            {
                db.Entry(processOutput).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OutputId = new SelectList(db.Products, "Id", "Name", processOutput.OutputId);
            ViewBag.ProcessId = new SelectList(db.Processes, "Id", "Name", processOutput.ProcessId);
            return View(processOutput);
        }

        // GET: ProcessOutputs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessOutput processOutput = db.ProcessOutputs.Find(id);
            if (processOutput == null)
            {
                return HttpNotFound();
            }
            return View(processOutput);
        }

        // POST: ProcessOutputs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProcessOutput processOutput = db.ProcessOutputs.Find(id);
            db.ProcessOutputs.Remove(processOutput);
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
