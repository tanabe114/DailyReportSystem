using DailyReportSystem.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

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

            //フォロー先ユーザーList作成
            List<Follows> myFollows = db.Follows
                .Where(r => r.EmployeeId == UserId)
                .ToList();

            // フォロー一覧リストを作成。
            List<FollowsIndexViewModel> indexViewModels = new List<FollowsIndexViewModel>();
            foreach (Follows follow in myFollows)
            {
                FollowsIndexViewModel indexViewModel = new FollowsIndexViewModel
                {
                    Id = follow.Id,
                    FollowName = db.Users.Find(follow.FollowId).EmployeeName,
                    FollowId = follow.FollowId
                };
                indexViewModels.Add(indexViewModel);
            }

            return View(indexViewModels);
        }

        // POST: Follows/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeId")] string EmployeeId)
        {
            //GET受信できてるか
            if (EmployeeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //フォロー先IDが有効か
            if (db.Users.Find(EmployeeId) == null)
            {
                return HttpNotFound();
            }

            //ログインユーザー取得
            string UserId = User.Identity.GetUserId();

            Follows follow = new Follows()
            {
                EmployeeId = UserId,
                FollowId = EmployeeId
            };

            //Contextに新しいオブジェクト追加
            db.Follows.Add(follow);
            //実際のDBに反映
            db.SaveChanges();
            // TempDataにフラッシュメッセージを入れておく。
            TempData["flush"] = String.Format("{0}さんをフォローしました。", db.Users.Find(EmployeeId).EmployeeName);
            //indexにRedirect（ページ遷移）
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "EmployeeId")] string EmployeeId)
        {
            if (EmployeeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ログインしているユーザーID取得
            string UserId = User.Identity.GetUserId();

            //フォロ－先一覧取得
            List<Follows> follows = db.Follows
                .Where(r => r.EmployeeId == UserId)
                .ToList();
            if (follows == null)
            {
                return HttpNotFound();
            }

            //解除したいフォロー先従業員の行取得
            Follows follow = follows.Find(f => f.FollowId == EmployeeId);
            if (follow == null)
            {
                return HttpNotFound();
            }

            // 行削除
            db.Follows.Remove(follow);
            db.SaveChanges();

            //解除先従業員名取得
            string deleteName = db.Users.Find(follow.FollowId).EmployeeName;
            TempData["flush"] = String.Format("{0}さんのフォローを解除しました。", deleteName);

            return RedirectToAction("Index", "Follows");
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