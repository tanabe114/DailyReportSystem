using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DailyReportSystem.Models
{
    public enum ReactionCategoryEnum
    {
        Like = 0,
        Love = 1,
        Haha = 2,
        Wow = 3
    }

    public class Reaction
    {
        [Key]
        public int Id { get; set; }

        public string EmployeeId { get; set; }

        public int ReportId { get; set; }

        [DisplayName("リアクションの種類")]
        public string Category { get; set; }

        [DisplayName("リアクションした日付")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }
    }
}