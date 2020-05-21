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
    public class ReactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reactions
        public ActionResult Index()
        {
            //ログインユーザーID取得
            string UserId = User.Identity.GetUserId();
            //リアクション先日報IDList作成
            List<int> myReportIds = db.Reactions
                .Where(r => r.EmployeeId == UserId)
                .Select(r => r.ReportId)
                .ToList();

            // 日報のリストから、表示用のビューモデルのリストを作成
            List<ReportsIndexViewModel> indexViewModels = new List<ReportsIndexViewModel>();
            foreach (int reportId in myReportIds)
            {
                Report report = db.Reports.Find(reportId);
                ReportsIndexViewModel indexViewModel = new ReportsIndexViewModel
                {
                    Id = report.Id,
                    EmployeeName = db.Users.Find(report.EmployeeId).EmployeeName,
                    EmployeeId = report.EmployeeId,
                    ReportDate = report.ReportDate,
                    Title = report.Title,
                    Content = report.Content,
                    NegotiationStatus = report.NegotiationStatus,
                };

                indexViewModel.ApprovalStatus = report.ApprovalStatus == 1 ? "承認済み" : "未承認";

                List<string> reactions = db.Reactions
                    .Where(r => r.ReportId == reportId)
                    .Select(r => r.EmployeeId)
                    .ToList();

                indexViewModel.ReactionQuantity = reactions.Count();

                indexViewModels.Add(indexViewModel);
            }

            return View(indexViewModels);
        }

        // POST: Reactions/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportId,Category")] Reaction reaction)
        {
            //受信できてるか
            if (ModelState.IsValid)
            {
                string UserId = User.Identity.GetUserId();

                //旧リアクションあるか判定
                //リアクション先一覧取得
                List<Reaction> reactions = db.Reactions
                    .Where(r => r.EmployeeId == UserId)
                    .ToList();
                if(reactions != null)
                {
                    Reaction oldReaction = reactions.Find(r => r.ReportId == reaction.ReportId);
                    if(oldReaction != null)
                    {
                        //旧リアクション削除
                        DeleteReaction(oldReaction);
                    }
                }

                reaction.EmployeeId = UserId;
                reaction.CreatedAt = DateTime.Now;

                //Contextに新しいオブジェクト追加
                db.Reactions.Add(reaction);
                //実際のDBに反映
                db.SaveChanges();
                //indexにRedirect（ページ遷移）
                return RedirectToAction("Index");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        private void DeleteReaction(Reaction reaction)
        {
            // 行削除
            db.Reactions.Remove(reaction);
            db.SaveChanges();
        }

        // POST: Reactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include = "ReportId,Category")] Reaction reaction)
        {
            if(ModelState.IsValid)
            {
                //ログインしているユーザーID取得
                string UserId = User.Identity.GetUserId();

                //リアクション先一覧取得
                List<Reaction> reactions = db.Reactions
                    .Where(r => r.EmployeeId == UserId)
                    .ToList();
                if (reactions == null)
                {
                    return HttpNotFound();
                }

                //解除したい行取得
                Reaction reactionRecord = reactions.Find(r => r.ReportId == reaction.ReportId && r.Category == reaction.Category);
                if (reactionRecord == null)
                {
                    return HttpNotFound();
                }

                // 行削除
                db.Reactions.Remove(reactionRecord);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
