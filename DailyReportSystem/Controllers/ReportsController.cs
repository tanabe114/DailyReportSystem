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
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reports
        public ActionResult Index()
        {
            // 日報のリストから、表示用のビューモデルのリストを作成
            List<ReportsIndexViewModel> indexViewModels = new List<ReportsIndexViewModel>();
            var reports = db.Reports.OrderByDescending(r => r.ReportDate).ToList();
            foreach (Report report in reports)
            {
                ReportsIndexViewModel indexViewModel = new ReportsIndexViewModel
                {
                    Id = report.Id,
                    // 従業員のリストからこの日報のEmployeeIdで検索をかけて取得した従業員の名前を設定
                    EmployeeName = db.Users.Find(report.EmployeeId).EmployeeName,
                    ReportDate = report.ReportDate,
                    Title = report.Title,
                    Content = report.Content,
                    NegotiationStatus = report.NegotiationStatus
                };
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
                UpdatedAt = report.UpdatedAt,
            };
            detailsViewModel.EmployeeName = db.Users.Find(report.EmployeeId).EmployeeName;
            detailsViewModel.isReportCreater = User.Identity.GetUserId() == report.EmployeeId;

            return View(detailsViewModel);
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
                    CreatedAt = DateTime.Now
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
