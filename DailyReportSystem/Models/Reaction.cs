using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DailyReportSystem.Models
{
    public class Reaction
    {
        [Key]
        public int Id { get; set; }

        public string EmployeeId { get; set; }

        public int ReportId { get; set; }

        [DisplayName("リアクションの種類")]
        public int Category { get; set; }

        [DisplayName("リアクションした日付")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }
    }
}