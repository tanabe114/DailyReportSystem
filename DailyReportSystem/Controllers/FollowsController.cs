using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DailyReportSystem.Models;
using Microsoft.AspNet.Identity;

namespace DailyReportSystem.Controllers
{
    [Authorize]
    public class FollowsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Follows
        public ActionResult Index()
        {
            //ログインしてるユーザーID取得
            string UserId = User.Identity.GetUserId();

            //フォロー先ユーザー探す
            var myFollows = db.Follows
                .Where(r => r.EmployeeId == UserId)
                .ToList();

            // フォロー一覧リストを作成。
            List<FollowsIndexViewModel> indexViewModels = new List<FollowsIndexViewModel>();
            foreach (Follows follow in myFollows)
            {
                FollowsIndexViewModel indexViewModel = new FollowsIndexViewModel
                {
                    Id = follow.Id,
                    FollowName = db.Users.Find(follow.FollowId).EmployeeName
                };
                indexViewModels.Add(indexViewModel);
            }

            return View(indexViewModels);
        }

        // GET: Follows/Create
        public ActionResult Create(string followId)
        {
            //GET受信できてるか
            if (followId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //フォロー先IDが有効か
            ApplicationUser applicationUser = db.Users.Find(followId);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            //ビューモデル生成
            FollowsCreateViewModel followsCreateViewModel = new FollowsCreateViewModel {
                UserId = User.Identity.GetUserId(),
                FollowId = followId,
                FollowName = db.Users.Find(followId).EmployeeName
            };
            return View(followsCreateViewModel);
        }

        // POST: Follows/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FollowId")] Follows follows)
        {
            if (ModelState.IsValid)
            {
                db.Follows.Add(follows);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(follows);
        }

        // GET: Follows/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Follows follows = db.Follows.Find(id);
            if (follows == null)
            {
                return HttpNotFound();
            }
            return View(follows);
        }

        // POST: Follows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Follows follows = db.Follows.Find(id);
            db.Follows.Remove(follows);
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
