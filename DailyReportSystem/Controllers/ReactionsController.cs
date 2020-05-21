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
                if (reactions != null) //リアクション先あるなら
                {
                    //旧リアクションインスタンス生成＆検索
                    Reaction oldReaction = reactions.Find(r => r.ReportId == reaction.ReportId);
                    //旧リアクションあるなら
                    if (oldReaction != null)
                    {
                        //旧リアクション削除
                        DeleteReaction(oldReaction);

                        //リアクション済みの種類と同種類のボタンがおされた
                        if (oldReaction.Category == reaction.Category)
                        {
                            //ビュー更新
                            ReportsDetailsViewModel plainDetailsViewModel = UpdateDetailsViewModel(reaction);
                            //旧リアクション消すだけで処理終了(ビューに返す)
                            return PartialView("_ReactionForm", plainDetailsViewModel);
                        }
                    }
                }

                //新リアクション追加準備
                reaction.EmployeeId = UserId;
                reaction.CreatedAt = DateTime.Now;

                //Contextに新しいオブジェクト追加
                db.Reactions.Add(reaction);
                //実際のDBに反映
                db.SaveChanges();

                //ビュー更新
                ReportsDetailsViewModel detailsViewModel = UpdateDetailsViewModel(reaction);

                //ビューに返す
                return PartialView("_ReactionForm", detailsViewModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        private void DeleteReaction(Reaction reaction)
        {
            // 行削除
            db.Reactions.Remove(reaction);
            db.SaveChanges();
        }

        private ReportsDetailsViewModel UpdateDetailsViewModel(Reaction reaction)
        {
            //detailsビューモデル生成
            Report report = db.Reports.Find(reaction.ReportId);
            ReportsDetailsViewModel detailsViewModel = new ReportsDetailsViewModel
            {
                Id = reaction.ReportId
            };
            string loginUserId = User.Identity.GetUserId();

            //リアクション
            //リアクション種類別GROUP分け
            var reactionsCategory = db.Reactions
                .Where(r => r.ReportId == report.Id)
                .GroupBy(r => r.Category);

            var reactionQuantity = new Dictionary<string, int>();
            var reactionFlag = new Dictionary<string, bool>();
            var reactionString = new Dictionary<string, string>();

            //Dictionary初期値設定(null防止)
            foreach (ReactionCategoryEnum c in Enum.GetValues(typeof(ReactionCategoryEnum)))
            {
                reactionQuantity.Add(c.ToString(), 0);
                reactionFlag.Add(c.ToString(), false);
            }

            reactionString.Add("Like", "いいね");
            reactionString.Add("Love", "超いいね");
            reactionString.Add("Haha", "笑い");
            reactionString.Add("Wow", "びっくり");

            //Dictionaryに値設定
            foreach (var rc in reactionsCategory)
            {
                reactionQuantity[rc.Key] = rc.Count();
                foreach (var r in rc)
                {
                    if (r.EmployeeId == loginUserId)
                    {
                        reactionFlag[rc.Key] = true;
                    }
                }
            }

            detailsViewModel.ReactionQuantity = reactionQuantity;
            detailsViewModel.ReactionFlag = reactionFlag;
            detailsViewModel.ReactionString = reactionString;

            //ビューに返す
            return (detailsViewModel);
        }
    }
}