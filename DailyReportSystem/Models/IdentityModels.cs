using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace DailyReportSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("従業員名")]
        [Required(ErrorMessage = "日報の日付を入力してください。")]
        [StringLength(20, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string EmployeeName { get; set; } // 名前

        [DisplayName("作成日付")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; } // データの作成された日時

        [DisplayName("更新日付")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; } // データの更新された日時

        public int DeleteFlg { get; set; } // Userの削除フラグ

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<DailyReportSystem.Models.Report> Reports { get; set; }

        public System.Data.Entity.DbSet<DailyReportSystem.Models.Follows> Follows { get; set; }
    }
}