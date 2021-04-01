using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EconModels;
using EconModels.JobModels;
using WebInterface.Models;

namespace WebInterface.Controllers
{
    public class JobsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: Jobs
        public ActionResult Index()
        {
            var jobs = db.Jobs.Include(j => j.Skill);
            return View(jobs.ToList());
        }

        // GET: Jobs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // GET: Jobs/Create
        public ActionResult Create()
        {
            var jobModel = new JobModel
            {
                Name = "",
                JobCategory = EconModels.Enums.JobCategory.Bureaucrat,
                JobType = EconModels.Enums.JobTypes.Bureaucrat,
                SkillLevel = 0,
                Skill = null
            };

            // processes
            jobModel.SelectedProcessIds = new int[] { };
            var procList = new List<SelectListItem>();
            foreach (var proc in db.Processes)
            {
                procList.Add(new SelectListItem
                {
                    Text = proc.Name,
                    Value = proc.Id.ToString()
                });
            }
            jobModel.ProcessList = procList;

            // Labor
            jobModel.SelectedLaborIds = new int[] { };
            var laborList = new List<SelectListItem>();
            foreach (var labor in db.Products.Where(x => x.ProductType == EconModels.Enums.ProductTypes.Service))
            {
                laborList.Add(new SelectListItem
                {
                    Text = labor.Name,
                    Value = labor.Id.ToString()
                });
            }
            jobModel.LaborList = laborList;

            // Other Jobs
            jobModel.SelectedRelatedJobIds = new int[] { };
            var jobs = new List<SelectListItem>();
            foreach (var job in db.Jobs)
            {
                jobs.Add(new SelectListItem
                {
                    Text = job.Name,
                    Value = job.Id.ToString()
                });
            }
            jobModel.Jobs = jobs;

            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name");

            return View(jobModel);
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobModel jobModel)
        {
            if (ModelState.IsValid)
            {
                var newJob = new Job
                {
                    Name = jobModel.Name,
                    JobCategory = jobModel.JobCategory,
                    JobType = jobModel.JobType,
                    SkillId = jobModel.SkillId,
                    SkillLevel = jobModel.SkillLevel
                };

                // get skill
                newJob.Skill = db.Skills.Single(x => x.Id == jobModel.SkillId);

                // get process(es)
                foreach (var id in jobModel.SelectedProcessIds)
                {
                    var proc = db.Processes.Single(x => x.Id == id);
                    newJob.Processes.Add(proc);
                    proc.Jobs.Add(newJob);
                }

                // get labor(s)
                foreach (var id in jobModel.SelectedLaborIds)
                {
                    var labor = db.Products.Single(x => x.Id == id);
                    newJob.Labor.Add(labor);
                    labor.Jobs.Add(newJob);
                }

                // get related job(s)
                foreach (var id in jobModel.SelectedRelatedJobIds)
                {
                    var otherJob = db.Jobs.Single(x => x.Id == id);
                    newJob.AddRelatedJob(otherJob);
                }

                db.Jobs.Add(newJob);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name", jobModel.SkillId);
            return View(jobModel);
        }

        // GET: Jobs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Single(x => x.Id == id);
            if (job == null)
            {
                return HttpNotFound();
            }
            var jobModel = new JobModel
            {
                Id = job.Id,
                Name = job.Name,
                JobCategory = job.JobCategory,
                JobType = job.JobType,
                SkillId = job.SkillId,
                Skill = db.Skills.Single(x => x.Id == job.SkillId),
                SkillLevel = job.SkillLevel
            };

            // processes
            jobModel.SelectedProcessIds = new int[] { };
            var procList = new List<SelectListItem>();
            foreach (var proc in db.Processes)
            {
                procList.Add(new SelectListItem
                {
                    Text = proc.Name,
                    Value = proc.Id.ToString()
                });
            }
            jobModel.ProcessList = procList;

            // Labor
            jobModel.SelectedLaborIds = new int[] { };
            var laborList = new List<SelectListItem>();
            foreach (var labor in db.Products.Where(x => x.ProductType == EconModels.Enums.ProductTypes.Service))
            {
                laborList.Add(new SelectListItem
                {
                    Text = labor.Name,
                    Value = labor.Id.ToString()
                });
            }
            jobModel.LaborList = laborList;

            // Other Jobs
            jobModel.SelectedRelatedJobIds = new int[] { };
            var jobs = new List<SelectListItem>();
            foreach (var otherJobs in db.Jobs)
            {
                jobs.Add(new SelectListItem
                {
                    Text = otherJobs.Name,
                    Value = otherJobs.Id.ToString()
                });
            }
            jobModel.Jobs = jobs;

            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name", jobModel.SkillId);

            return View(jobModel);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobModel job)
        {
            if (ModelState.IsValid)
            {
                var update = db.Jobs.Single(x => x.Id == job.Id);

                // Update normal Stats.
                update.Name = job.Name;
                update.JobCategory = job.JobCategory;
                update.JobType = job.JobType;
                update.SkillId = job.SkillId;
                update.SkillLevel = job.SkillLevel;

                // processes
                update.ClearProcessRelations();
                foreach (var id in job.SelectedProcessIds)
                {
                    update.AddProcess(db.Processes.Single(x => x.Id == id));
                }

                // labor
                update.ClearLaborRelations();
                foreach (var id in job.SelectedLaborIds)
                {
                    update.AddLabor(db.Products.Single(x => x.Id == id));
                }
                
                // jobs
                update.ClearJobRelations();
                foreach (var id in job.SelectedRelatedJobIds)
                {
                    update.AddRelatedJob(db.Jobs.Single(x => x.Id == id));
                }

                db.Entry(update).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name", job.SkillId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Job job = db.Jobs.Find(id);

            job.ClearJobRelations();
            job.ClearLaborRelations();
            job.ClearProcessRelations();

            db.Jobs.Remove(job);
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
