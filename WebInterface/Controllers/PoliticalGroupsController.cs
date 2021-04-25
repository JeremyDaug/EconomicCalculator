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
using WebInterface.Models;

namespace WebInterface.Controllers
{
    public class PoliticalGroupsController : Controller
    {
        private EconSimContext db = new EconSimContext();

        // GET: PoliticalGroups
        public ActionResult Index()
        {
            var politicalGroups = db.PoliticalGroups
                .Include(x => x.Tags)
                .Include(x => x.Allies)
                .Include(x => x.Enemies);

            return View(politicalGroups.ToList());
        }

        // GET: PoliticalGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalGroup politicalGroup = db.PoliticalGroups
                .Include(x => x.Tags)
                .Include(x => x.Allies)
                .Include(x => x.Enemies)
                .SingleOrDefault(x => x.Id == id);

            if (politicalGroup == null)
            {
                return HttpNotFound();
            }
            return View(politicalGroup);
        }

        // GET: PoliticalGroups/Create
        public ActionResult Create()
        {
            var groupModel = new PoliticalGroupModel();

            // allies
            var allyList = new List<SelectListItem>();
            foreach (var party in db.PoliticalGroups)
            {
                allyList.Add(new SelectListItem
                {
                    Text = party.Name + " : " + party.VariantName,
                    Value = party.Id.ToString()
                });
            }

            groupModel.SelectedAllyIds = new int[] { };
            groupModel.AllyList = allyList;

            // enemies
            var enemyList = new List<SelectListItem>();
            foreach (var party in db.PoliticalGroups)
            {
                enemyList.Add(new SelectListItem
                {
                    Text = party.Name + " : " + party.VariantName,
                    Value = party.Id.ToString()
                });
            }

            groupModel.SelectedEnemyIds = new int[] { };
            groupModel.EnemyList = enemyList;

            return View(groupModel);
        }

        // POST: PoliticalGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PoliticalGroupModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.PoliticalGroups.Any(x => x.Name == model.Name && x.VariantName == model.VariantName))
                    return View(model);

                var newGroup = new PoliticalGroup
                {
                    Name = model.Name,
                    VariantName = model.VariantName,
                    Authority = model.Authority,
                    Centralization = model.Centralization,
                    Militarism = model.Militarism,
                    Nationalism = model.Nationalism,
                    Planning = model.Planning,
                    Radicalism = model.Radicalism
                };

                // preemptively save
                db.PoliticalGroups.Add(newGroup);

                db.SaveChanges();

                newGroup = db.PoliticalGroups.Single(x => x.Name == model.Name);

                // allies
                foreach (var id in model.SelectedAllyIds)
                {
                    var relGroup = db.PoliticalGroups.Single(x => x.Id == id);
                    newGroup.AddAlly(relGroup);
                }
                // enemies
                foreach (var id in model.SelectedEnemyIds)
                {
                    var relGroup = db.PoliticalGroups.Single(x => x.Id == id);
                    newGroup.AddEnemy(relGroup);
                }

                db.Entry(newGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: PoliticalGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalGroup pol = db.PoliticalGroups
                .Include(x => x.Tags)
                .Include(x => x.Allies)
                .Include(x => x.Enemies)
                .SingleOrDefault(x => x.Id == id);
            if (pol == null)
            {
                return HttpNotFound();
            }

            var groupModel = new PoliticalGroupModel
            {
                Id = pol.Id,
                Authority = pol.Authority,
                Centralization = pol.Centralization,
                Militarism = pol.Militarism,
                Nationalism = pol.Nationalism,
                Planning = pol.Planning,
                Radicalism = pol.Radicalism,
                Name = pol.Name,
                VariantName = pol.VariantName
            };

            // allies
            var allyList = new List<SelectListItem>();
            foreach (var party in db.PoliticalGroups)
            {
                if (party.Id != id)
                {
                    allyList.Add(new SelectListItem
                    {
                        Text = party.Name + " : " + party.VariantName,
                        Value = party.Id.ToString()
                    });
                }
            }

            var allies = new List<int>();

            foreach (var party in pol.Allies)
            {
                if (party.Id != id)
                    allies.Add(party.Id);
            }

            groupModel.SelectedAllyIds = allies.ToArray();
            groupModel.AllyList = allyList;

            // enemies
            var enemyList = new List<SelectListItem>();
            foreach (var party in db.PoliticalGroups)
            {
                if (party.Id != id)
                {
                    enemyList.Add(new SelectListItem
                    {
                        Text = party.Name + " : " + party.VariantName,
                        Value = party.Id.ToString()
                    });
                }
            }

            var enemies = new List<int>();

            foreach (var party in pol.Enemies)
            {
                if (party.Id != id)
                    enemies.Add(party.Id);
            }

            groupModel.SelectedEnemyIds = enemies.ToArray();
            groupModel.EnemyList = enemyList;

            return View(groupModel);
        }

        // POST: PoliticalGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,VariantName,Radicalism,Nationalism,Centralization,Authority,Planning,Militarism")] PoliticalGroup politicalGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(politicalGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(politicalGroup);
        }

        // GET: PoliticalGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PoliticalGroup politicalGroup = db.PoliticalGroups
                .Include(x => x.Tags)
                .Include(x => x.Allies)
                .Include(x => x.Enemies)
                .SingleOrDefault(x => x.Id == id);

            if (politicalGroup == null)
            {
                return HttpNotFound();
            }
            return View(politicalGroup);
        }

        // POST: PoliticalGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PoliticalGroup pol = db.PoliticalGroups
                .Include(x => x.Allies)
                .Include(x => x.AlliesRev)
                .Include(x => x.Enemies)
                .Include(x => x.EnemiesRev)
                .Single(x => x.Id == id);

            foreach (var ally in pol.Allies)
            {
                ally.Allies.Remove(pol);
                ally.AlliesRev.Remove(pol);
            }
            foreach (var enemy in pol.Enemies)
            {
                enemy.Enemies.Remove(pol);
                enemy.EnemiesRev.Remove(pol);
            }
            pol.Allies.Clear();
            pol.AlliesRev.Clear();
            pol.Enemies.Clear();
            pol.EnemiesRev.Clear();

            db.PoliticalGroups.Remove(pol);
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
