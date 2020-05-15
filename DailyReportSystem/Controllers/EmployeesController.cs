using DailyReportSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DailyReportSystem.Controllers
{
    [Authorize(Roles = "Admin,Chief,Manager,GeneralManager,ManagingDirector,President")]
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // このアプリケーション用のユーザーのサインインを管理するSignInManager
        private ApplicationSignInManager _signInManager;

        // このアプリケーション用のユーザー情報の管理をするUserManager
        private ApplicationUserManager _userManager;

        public EmployeesController()
        {
        }

        public EmployeesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        // GET: Employees
        public ActionResult Index()
        {
            //ログインユーザーID取得
            string UserId = User.Identity.GetUserId();
            //フォロー先ユーザーList作成
            List<string> myFollows = db.Follows
                .Where(r => r.EmployeeId == UserId)
                .Select(r => r.FollowId)
                .ToList();

            // ビューに送るためのEmployeesIndexViewModelのリストを作成
            List<EmployeesIndexViewModel> indexViewModel = new List<EmployeesIndexViewModel>();
            // ユーザー一覧を、作成日時が最近のものから順にしてリストとして取得
            List<ApplicationUser> users = db.Users.OrderByDescending(u => u.CreatedAt).ToList();
            // ユーザーのリストを、EmployeesIndexViewModelのリストに変換
            foreach (ApplicationUser applicationUser in users)
            {
                // EmployeesIndexViewModelをApplicationUserから必要なプロパティだけ抜き出して作成
                EmployeesIndexViewModel employee = new EmployeesIndexViewModel
                {
                    Email = applicationUser.Email,
                    EmployeeName = applicationUser.EmployeeName,
                    DeleteFlg = applicationUser.DeleteFlg,
                    Id = applicationUser.Id
                };

                //フォローボタン制御
                if (applicationUser.Id == UserId) //ログインユーザー自身
                {
                    employee.FollowStatusFlag = FollowStatusEnum.LoginUser;
                }
                else if (myFollows.Contains(employee.Id)) //フォロー済み
                {
                    employee.FollowStatusFlag = FollowStatusEnum.Following;
                }
                else //未フォロー
                {
                    employee.FollowStatusFlag = FollowStatusEnum.Unfollowed;
                }

                // 作成したEmployeesIndexViewModelをリストに追加
                indexViewModel.Add(employee);
            }
            // 作成したリストをIndexビューに送る
            return View(indexViewModel);
        }

        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            // idが無い場合、不正なリクエストとして処理
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            // ユーザーが取得できなければ、NotFoundエラーページへ
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            // ビューモデルにデータを詰め替える
            EmployeesDetailsViewModel employee = new EmployeesDetailsViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                EmployeeName = applicationUser.EmployeeName,
                CreatedAt = applicationUser.CreatedAt,
                UpdatedAt = applicationUser.UpdatedAt
            };

            employee.Role = UserManager.IsInRole(applicationUser.Id, "Admin") ? "管理者" : "一般";

            return View(employee);
        }

        // GET: Employees/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View(new EmployeesCreateViewModel());
        }

        // POST: Employees/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EmployeeName,Email,Password,AdminFlag")] EmployeesCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ビューから受け取ったEmployeesCreateViewModelからユーザー情報を作成
                ApplicationUser applicationUser =
                    new ApplicationUser
                    {
                        // IdentityアカウントのUserNameにはメールアドレスを入れる必要がある
                        UserName = model.Email,
                        Email = model.Email,
                        EmployeeName = model.EmployeeName,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        DeleteFlg = 0
                    };

                // ユーザー情報をDBに登録
                var result = await UserManager.CreateAsync(applicationUser, model.Password);
                // DB登録に成功した場合
                if (result.Succeeded)
                {
                    // Roleを追加する
                    var roleManager = new RoleManager<ApplicationRole>(
                        new RoleStore<ApplicationRole>(new ApplicationDbContext())
                        );

                    //            [Display(Name = "係長")]
                    //Chief = 2,
                    //[Display(Name = "部長")]
                    //Manager = 3,
                    //[Display(Name = "本部長")]
                    //GeneralManager = 4,
                    //[Display(Name = "専務")]
                    //ManagingDirector = 5,
                    //[Display(Name = "社長")]
                    //President = 6
                    if (!await roleManager.RoleExistsAsync("Chief"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "Chief" });
                    }
                    if (!await roleManager.RoleExistsAsync("Manager"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "Manager" });
                    }
                    if (!await roleManager.RoleExistsAsync("GeneralManager"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "GeneralManager" });
                    }
                    if (!await roleManager.RoleExistsAsync("ManagingDirector"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "ManagingDirector" });
                    }
                    if (!await roleManager.RoleExistsAsync("President"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "President" });
                    }

                    // mode.AdminFlagの内容によって、処理をswitchで変える。
                    switch (model.AdminFlag)
                    {
                        case RolesEnum.Chief:
                            await UserManager.AddToRoleAsync(applicationUser.Id, "Chief");
                            break;

                        case RolesEnum.Manager:
                            await UserManager.AddToRoleAsync(applicationUser.Id, "Manager");
                            break;

                        case RolesEnum.GeneralManager:
                            await UserManager.AddToRoleAsync(applicationUser.Id, "GeneralManager");
                            break;

                        case RolesEnum.ManagingDirector:
                            await UserManager.AddToRoleAsync(applicationUser.Id, "ManagingDirector");
                            break;

                        case RolesEnum.President:
                            await UserManager.AddToRoleAsync(applicationUser.Id, "President");
                            break;
                    }

                    // TempDataにフラッシュメッセージを入れておく。
                    TempData["flush"] = String.Format("{0}さんを登録しました。", applicationUser.EmployeeName);

                    return RedirectToAction("Index", "Employees");
                }
                //DB登録に失敗したらエラー登録
                AddErrors(result);
            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            // idが無い場合、不正なリクエストとして処理
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            // ビューモデルにデータを詰め替える
            EmployeesEditViewModel employee = new EmployeesEditViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                EmployeeName = applicationUser.EmployeeName
            };

            if (UserManager.IsInRole(applicationUser.Id, "Chief"))
            {
                employee.AdminFlag = RolesEnum.Chief;
            }
            else if (UserManager.IsInRole(applicationUser.Id, "Manager"))
            {
                employee.AdminFlag = RolesEnum.Manager;
            }
            else if (UserManager.IsInRole(applicationUser.Id, "GeneralManager"))
            {
                employee.AdminFlag = RolesEnum.GeneralManager;
            }
            else if (UserManager.IsInRole(applicationUser.Id, "ManagingDirector"))
            {
                employee.AdminFlag = RolesEnum.ManagingDirector;
            }
            else if (UserManager.IsInRole(applicationUser.Id, "President"))
            {
                employee.AdminFlag = RolesEnum.President;
            }
            else
            {
                employee.AdminFlag = RolesEnum.Normal;
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,EmployeeName,Password,AdminFlag")] EmployeesEditViewModel employee)
        {
            if (ModelState.IsValid)
            {
                // DBからidのユーザーを検索し取得。そのユーザーに対し変更をする
                ApplicationUser applicationUser = db.Users.Find(employee.Id);
                // IdentityアカウントのUserNameにはメールアドレスを入れる必要がある
                applicationUser.UserName = employee.Email;
                applicationUser.Email = employee.Email;
                applicationUser.EmployeeName = employee.EmployeeName;
                applicationUser.UpdatedAt = DateTime.Now;
                // passwordが空でなければパスワード変更する。
                if (!String.IsNullOrEmpty(employee.Password))
                {
                    // パスワードの入力検証
                    var result = await UserManager.PasswordValidator.ValidateAsync(employee.Password);
                    // パスワードの検証に失敗したら、エラーを追加しEditビューをもう一度描画
                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                        return View(employee);
                    }
                    // パスワードはハッシュ化したものをDBに登録する必要があるので、PasswordHasherでハッシュ化する
                    applicationUser.PasswordHash = UserManager.PasswordHasher.HashPassword(employee.Password);
                }
                // StateをModifiedにしてUPDATE文を行うように設定
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();

                // mode.AdminFlagの内容によって、処理をswitchで変える。
                switch (employee.AdminFlag)
                {
                    case RolesEnum.Chief:
                        if (UserManager.IsInRole(applicationUser.Id, "Chief"))
                            break;
                        UserManager.AddToRole(applicationUser.Id, "Chief");
                        break;

                    case RolesEnum.Manager:
                        if (UserManager.IsInRole(applicationUser.Id, "Manager"))
                            break;
                        UserManager.AddToRole(applicationUser.Id, "Manager");
                        break;

                    case RolesEnum.GeneralManager: 
                        if (UserManager.IsInRole(applicationUser.Id, "GeneralManager"))
                            break;
                        UserManager.AddToRole(applicationUser.Id, "GeneralManager");
                        break;

                    case RolesEnum.ManagingDirector:
                        if (UserManager.IsInRole(applicationUser.Id, "ManagingDirector"))
                            break;
                        UserManager.AddToRole(applicationUser.Id, "ManagingDirector");
                        break;

                    case RolesEnum.President:
                        if (UserManager.IsInRole(applicationUser.Id, "President"))
                            break;
                        UserManager.AddToRole(applicationUser.Id, "President");
                        break;

                    default:
                        if (UserManager.IsInRole(applicationUser.Id, "Chief"))
                        {
                            UserManager.RemoveFromRole(applicationUser.Id, "Chief");
                        }
                        if (UserManager.IsInRole(applicationUser.Id, "Manager"))
                        {
                            UserManager.RemoveFromRole(applicationUser.Id, "Manager");
                        }
                        if (UserManager.IsInRole(applicationUser.Id, "GeneralManager"))
                        {
                            UserManager.RemoveFromRole(applicationUser.Id, "GeneralManager");
                        }
                        if (UserManager.IsInRole(applicationUser.Id, "ManagingDirector"))
                        {
                            UserManager.RemoveFromRole(applicationUser.Id, "ManagingDirector");
                        }
                        if (UserManager.IsInRole(applicationUser.Id, "President"))
                        {
                            UserManager.RemoveFromRole(applicationUser.Id, "President");
                        }
                        break;
                }

                // TempDataにフラッシュメッセージを入れておく。TempDataは現在のリクエストと次のリクエストまで存在
                TempData["flush"] = String.Format("{0}さんの情報を更新しました。", applicationUser.EmployeeName);

                return RedirectToAction("Index", "Employees");
            }

            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            // idが無い場合、不正なリクエストとして処理
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            // ユーザーが取得できなければ、NotFoundエラーページへ
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            // ビューモデルにデータを詰め替える
            EmployeesDeleteViewModel employee = new EmployeesDeleteViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                EmployeeName = applicationUser.EmployeeName,
                CreatedAt = applicationUser.CreatedAt,
                UpdatedAt = applicationUser.UpdatedAt
            };

            employee.Role = UserManager.IsInRole(applicationUser.Id, "Admin") ? "管理者" : "一般";

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            // ユーザーを論理削除
            applicationUser.DeleteFlg = 1;
            // StateをModifiedにしてUPDATE文を行うように設定
            db.Entry(applicationUser).State = EntityState.Modified;
            db.SaveChanges();

            // TempDataにフラッシュメッセージを入れておく。TempDataは現在のリクエストと次のリクエストまで存在
            TempData["flush"] = String.Format("{0}さんの情報を削除しました。", applicationUser.EmployeeName);

            return RedirectToAction("Index", "Employees");
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