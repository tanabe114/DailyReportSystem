using DailyReportSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace DailyReportSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //ログインしている人のID取得
            string UserId = User.Identity.GetUserId();

            //フォロー先従業員IDリスト(myFollows)作成
            List<string> myFollows = db.Follows
                .Where(f => f.EmployeeId == UserId)
                .Select(f => f.FollowId)
                .ToList();

            //myFollowsに自分のID追加
            myFollows.Add(UserId);

            //フォローしている日報取得　日付順に並べ替え
            List<Report> myReports = db.Reports
                .Where(r => myFollows.Contains(r.EmployeeId))
                .OrderByDescending(r => r.ReportDate)
                .ToList();

            //日報表示
            List<ReportsIndexViewModel> indexViewModels = new List<ReportsIndexViewModel>();
            foreach (Report report in myReports)
            {
                ReportsIndexViewModel indexViewModel = new ReportsIndexViewModel
                {
                    Id = report.Id,
                    EmployeeName = db.Users.Find(report.EmployeeId).EmployeeName,
                    ReportDate = report.ReportDate,
                    Title = report.Title,
                    Content = report.Content,
                    NegotiationStatus = report.NegotiationStatus
                };
                indexViewModel.ApprovalStatus = report.ApprovalStatus == 1 ? "承認済み" : "未承認";
                indexViewModels.Add(indexViewModel);
            }
            return View(indexViewModels);
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