using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DailyReportSystem.Models
{
    public class FollowsIndexViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("フォロー先従業員名")]
        public string FollowName { get; set; }
    }

    public class FollowsCreateViewModel
    {
        [DisplayName("ユーザーID")]
        public string UserId { get; set; }

        [DisplayName("フォロー先従業員ID")]
        public string FollowId { get; set; }

        [DisplayName("フォロー先従業員名")]
        public string FollowName { get; set; }
    }
}