using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DailyReportSystem.Models;

namespace DailyReportSystem.Controllers
{
    public class ReactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reactions
        public ActionResult Index()
        {
            return View(db.Reactions.ToList());
        }

        // GET: Reactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reaction reaction = db.Reactions.Find(id);
            if (reaction == null)
            {
                return HttpNotFound();
            }
            return View(reaction);
        }

        // GET: Reactions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reactions/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmployeeId,ReportId,Category,CreatedAt")] Reaction reaction)
        {
            if (ModelState.IsValid)
            {
                db.Reactions.Add(reaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reaction);
        }

        // GET: Reactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reaction reaction = db.Reactions.Find(id);
            if (reaction == null)
            {
                return HttpNotFound();
            }
            return View(reaction);
        }

        // POST: Reactions/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmployeeId,ReportId,Category,CreatedAt")] Reaction reaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reaction);
        }

        // GET: Reactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reaction reaction = db.Reactions.Find(id);
            if (reaction == null)
            {
                return HttpNotFound();
            }
            return View(reaction);
        }

        // POST: Reactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reaction reaction = db.Reactions.Find(id);
            db.Reactions.Remove(reaction);
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
