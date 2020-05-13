using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DailyReportSystem.Models
{
    public class Follows
    {
        [Key]
        public int Id { get; set; }

        public string EmployeeId { get; set; }

        public string FollowId { get; set; }
    }
}