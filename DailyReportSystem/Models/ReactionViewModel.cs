using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DailyReportSystem.Models
{
    public class ReactionIndexViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("フォロー先従業員名")]
        public string FollowName { get; set; }
        public string FollowId { get; set; }
    }
}