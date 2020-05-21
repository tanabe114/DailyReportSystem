using DailyReportSystem.Models;
using DailyReportSystem.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

//using Utils.RolesEnumUtil;

namespace DailyReportSystem.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // このアプリケーション用のユーザー情報の管理をするUserManager
        private ApplicationUserManager _userManager;

        public ReportsController()
        {
        }

        public ReportsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Reports
        public ActionResult Index()
        {
            //ログインユーザーID取得
            string UserId = User.Identity.GetUserId();
            //フォロー先ユーザーList作成
            List<string> myFollows = db.Follows
                .Where(r => r.EmployeeId == UserId)
                .Select(r => r.FollowId)
                .ToList();

            // 日報のリストから、表示用のビューモデルのリストを作成
            List<ReportsIndexViewModel> indexViewModels = new List<ReportsIndexViewModel>();
            var reports = db.Reports.OrderByDescending(r => r.ReportDate).ToList();
            foreach (Report report in reports)
            {
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
                    .Where(r => r.ReportId == report.Id)
                    .Select(r => r.EmployeeId)
                    .ToList();

                indexViewModel.ReactionQuantity = reactions.Count();

                //フォローボタン制御
                if (report.EmployeeId == UserId) //ログインユーザー自身
                {
                    indexViewModel.FollowStatusFlag = FollowStatusEnum.LoginUser;
                }
                else if (myFollows.Contains(report.EmployeeId)) //フォロー済み
                {
                    indexViewModel.FollowStatusFlag = FollowStatusEnum.Following;
                }
                else //未フォロー
                {
                    indexViewModel.FollowStatusFlag = FollowStatusEnum.Unfollowed;
                }

                indexViewModels.Add(indexViewModel);
            }

            return View(indexViewModels);
        }

        // GET: Reports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            ReportsDetailsViewModel detailsViewModel = new ReportsDetailsViewModel
            {
                Id = report.Id,
                ReportDate = report.ReportDate,
                Title = report.Title,
                Content = report.Content,
                NegotiationStatus = report.NegotiationStatus,
                AttendanceTime = report.AttendanceTime,
                LeavingTime = report.LeavingTime,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt
            };

            ApplicationUser reportUser = db.Users.Find(report.EmployeeId);
            string loginUserId = User.Identity.GetUserId();

            detailsViewModel.EmployeeName = reportUser.EmployeeName;
            detailsViewModel.IsReportCreater = loginUserId == report.EmployeeId;
            detailsViewModel.ApprovalStatus = report.ApprovalStatus == 1 ? "承認済み" : "未承認";

            string reportUserRole = UserManager.GetRoles(reportUser.Id).Count() != 0 ? UserManager.GetRoles(reportUser.Id)[0] : "Normal";
            string loginUserRole = UserManager.GetRoles(loginUserId).Count() != 0 ? UserManager.GetRoles(loginUserId)[0] : "Normal";

            if (RolesEnumUtil.GetRoleNum(reportUserRole) < RolesEnumUtil.GetRoleNum(loginUserRole))
            {
                detailsViewModel.Approvable = true;
            }
            else
            {
                detailsViewModel.Approvable = false;
            }

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
            foreach (var reactions in reactionsCategory)
            {
                reactionQuantity[reactions.Key] = reactions.Count();
                foreach (var r in reactions)
                {
                    if (r.EmployeeId == loginUserId)
                    {
                        reactionFlag[reactions.Key] = true;
                    }
                } 
            }

            detailsViewModel.ReactionQuantity = reactionQuantity;
            detailsViewModel.ReactionFlag = reactionFlag;
            detailsViewModel.ReactionString = reactionString;

            return View(detailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "ReportId, EmployeeName")] int? ReportId, string EmployeeName)
        {
            if (ReportId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Report report = db.Reports.Find(ReportId);
            if (report == null)
            {
                return HttpNotFound();
            }
            if (report.ApprovalStatus == 1)
            {
                report.ApprovalStatus = 0;
            }
            else
            {
                report.ApprovalStatus = 1;
            }

            TempData["flush"] = report.ApprovalStatus == 1 ? $"{EmployeeName}さんの日報を承認しました。" : $"{EmployeeName}さんの日報の承認を解除しました。";

            db.Entry(report).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View(new ReportsCreateViewModel());
        }

        // POST: Reports/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportDate,Title,Content,NegotiationStatus,AttendanceTime,LeavingTime")] ReportsCreateViewModel createViewModel)
        {
            if (ModelState.IsValid)
            {
                Report report = new Report()
                {
                    ReportDate = createViewModel.ReportDate,
                    Title = createViewModel.Title,
                    Content = createViewModel.Content,
                    NegotiationStatus = createViewModel.NegotiationStatus,
                    EmployeeId = User.Identity.GetUserId(),
                    AttendanceTime = createViewModel.AttendanceTime,
                    LeavingTime = createViewModel.LeavingTime,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    ApprovalStatus = 0
                };

                //Contextに新しいオブジェクト追加
                db.Reports.Add(report);
                //実際のDBに反映
                db.SaveChanges();
                // TempDataにフラッシュメッセージを入れておく。
                TempData["flush"] = "日報を登録しました。";
                //indexにRedirect（ページ遷移）
                return RedirectToAction("Index");
            }

            return View(createViewModel);
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }

            //本人の日報でなければ表示しないように、それをEdit.cshtmlにTempDataで伝える。
            if (report.EmployeeId != User.Identity.GetUserId())
            {
                TempData["wrong_person"] = "true";
            }
            ReportsEditViewModel editViewModel = new ReportsEditViewModel
            {
                Id = report.Id,
                ReportDate = report.ReportDate,
                Title = report.Title,
                Content = report.Content,
                NegotiationStatus = report.NegotiationStatus,
                AttendanceTime = report.AttendanceTime,
                LeavingTime = report.LeavingTime
            };

            return View(editViewModel);
        }

        // POST: Reports/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ReportDate,Title,Content,NegotiationStatus,AttendanceTime,LeavingTime")] ReportsEditViewModel editViewModel)
        {
            if (ModelState.IsValid)
            {
                Report report = db.Reports.Find(editViewModel.Id);
                report.ReportDate = editViewModel.ReportDate;
                report.Title = editViewModel.Title;
                report.Content = editViewModel.Content;
                report.NegotiationStatus = editViewModel.NegotiationStatus;
                report.AttendanceTime = editViewModel.AttendanceTime;
                report.LeavingTime = editViewModel.LeavingTime;
                report.UpdatedAt = DateTime.Now;
                db.Entry(report).State = EntityState.Modified;
                db.SaveChanges();

                // TempDataにフラッシュメッセージを入れておく。
                TempData["flush"] = "日報を編集しました。";
                return RedirectToAction("Index");
            }
            return View(editViewModel);
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