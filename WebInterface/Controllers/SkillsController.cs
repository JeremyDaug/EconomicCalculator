using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EconModels;
using EconModels.Enums;
using EconModels.SkillsModel;
using WebInterface.Models;

namespace WebInterface.Controllers
{
    public class SkillsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: Skills
        public ActionResult Index()
        {
            return View(db.Skills.ToList());
        }

        // GET: Skills/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        // GET: Skills/Create
        public ActionResult Create()
        {
            var skillModel = new SkillModel();

            skillModel.Id = 0;
            skillModel.Name = "";
            skillModel.Desc = "";
            skillModel.Min = 0;
            skillModel.Max = 0;

            // Skills
            var skillList = new List<SelectListItem>();

            foreach (var skill in db.Skills)
            {
                skillList.Add(new SelectListItem
                {
                    Text = skill.Name,
                    Value = skill.Id.ToString()
                });
            }

            skillModel.SelectedSkillIds = new int[] { };
            skillModel.RelatedSkills = skillList;

            // Labors
            var laborList = new List<SelectListItem>();

            // only grab labors, not all products.
            foreach (var labor in db.Products.Where(x => x.ProductType == ProductTypes.Service))
            {
                laborList.Add(new SelectListItem
                {
                    Text = labor.Name,
                    Value = labor.Id.ToString()
                });
            }

            skillModel.LaborList = laborList;
            skillModel.SelectedLaborIds = new int[] { };

            return View(skillModel);
        }

        // POST: Skills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SkillModel skill)
        {
            if (ModelState.IsValid)
            {
                var newSkill = new Skill
                {
                    Name = skill.Name,
                    Desc = skill.Desc,
                    Min = skill.Min,
                    Max = skill.Max
                };
                // related skills
                foreach (var id in skill.SelectedSkillIds)
                {
                    var relSkill = db.Skills.Single(x => x.Id == id);
                    newSkill.AddSkillRelation(relSkill);
                }
                // related Labors
                foreach (var id in skill.SelectedLaborIds)
                {
                    var relLab = db.Products.Single(x => x.Id == id);
                    newSkill.ValidLabors.Add(relLab);
                    relLab.Skills.Add(newSkill);
                }
                // do not connect to jobs here. Jobs should decide who they relate to.

                db.Skills.Add(newSkill);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(skill);
        }

        // GET: Skills/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            // Convert to Skill Model
            var skillModel = new SkillModel
            {
                Id = skill.Id,
                Name = skill.Name,
                Desc = skill.Desc,
                Min = skill.Min,
                Max = skill.Max
            };
            // get all skills for possible relations
            var skillList = new List<SelectListItem>();

            foreach (var relSkill in db.Skills)
            {
                skillList.Add(new SelectListItem
                {
                    Text = relSkill.Name,
                    Value = relSkill.Id.ToString()
                });
            }

            skillModel.RelatedSkills = skillList;
            // get selected related skills
            var selectedIds = new List<int>();
            foreach (var relSkill in skill.RelationChild)
            {
                selectedIds.Add(relSkill.Id);
            }
            skillModel.SelectedSkillIds = selectedIds.ToArray();

            // get labors

            var laborList = new List<SelectListItem>();

            // only grab labors, not all products.
            foreach (var labor in db.Products.Where(x => x.ProductType == ProductTypes.Service))
            {
                laborList.Add(new SelectListItem
                {
                    Text = labor.Name,
                    Value = labor.Id.ToString()
                });
            }

            // get already selected labors
            var laborIds = new List<int>();
            foreach (var relLabor in skill.ValidLabors)
            {
                laborIds.Add(relLabor.Id);
            }
            skillModel.SelectedLaborIds = laborIds.ToArray();
            skillModel.LaborList = laborList;

            return View(skillModel);
        }

        // POST: Skills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SkillModel skillModel)
        {
            if (ModelState.IsValid)
            {
                // get skill being edited.
                var skill = db.Skills.Single(x => x.Id == skillModel.Id);

                // update normal stats.
                skill.Name = skillModel.Name;
                skill.Desc = skillModel.Desc;
                skill.Min = skillModel.Min;
                skill.Max = skillModel.Max;

                // update related skills, remove old stuff, get new stuff.
                skill.ClearSkillRelations();
                foreach (var id in skillModel.SelectedSkillIds)
                {
                    skill.AddSkillRelation(db.Skills.Single(x => x.Id == id));
                }

                // update related labors, remove old stuff, get new stuff.
                foreach (var prod in skill.ValidLabors)
                {
                    prod.Skills.Remove(skill);
                }
                skill.ValidLabors.Clear();

                foreach (var id in skillModel.SelectedLaborIds)
                {
                    var labor = db.Products.Single(x => x.Id == id);
                    skill.ValidLabors.Add(labor);
                    labor.Skills.Add(skill);
                }

                db.Entry(skill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skillModel);
        }

        // GET: Skills/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Skill skill = db.Skills.Find(id);

            skill.ClearSkillRelations();
            skill.ValidLabors.Clear();

            db.Skills.Remove(skill);
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
