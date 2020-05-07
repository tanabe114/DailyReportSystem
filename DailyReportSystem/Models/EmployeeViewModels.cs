using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace DailyReportSystem.Models
{
    public class EmployeesIndexViewModel
    {
        public string Id { get; set; }

        [DisplayName("メールアドレス")]
        public string Email { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        public int DeleteFlg { get; set; }
    }

}