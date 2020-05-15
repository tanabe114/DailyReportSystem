using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DailyReportSystem.Models
{
    public enum FollowStatusEnum
    {
        Unfollowed = 0,
        Following = 1,
        LoginUser = 2
    }

    public class FollowsIndexViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("フォロー先従業員名")]
        public string FollowName { get; set; }
        public string FollowId { get; set; }
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

    public class FollowsDeleteViewModel
    {
        public int Id { get; set; }

        [DisplayName("フォロー先従業員名")]
        public string FollowName { get; set; }
    }
}