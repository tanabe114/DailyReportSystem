﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DailyReportSystem.Models
{
    public class ReportsIndexViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        public string EmployeeId { get; set; }

        [DisplayName("日付")]
        [DataType(DataType.Date)]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        public string Title { get; set; }

        [DisplayName("内容")]
        public string Content { get; set; }

        [DisplayName("商談状況")]
        public string NegotiationStatus { get; set; }

        [DisplayName("リアクション数")]
        public int ReactionQuantity { get; set; }

        [DisplayName("承認状況")]
        public string ApprovalStatus { get; internal set; }

        public FollowStatusEnum? FollowStatusFlag { get; set; }
    }

    public class ReportsCreateViewModel
    {
        [DisplayName("日付")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "日報の日付を入力してください。")]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        [Required(ErrorMessage = "タイトルを入力してください。")]
        [StringLength(100, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string Title { get; set; }

        [DisplayName("内容")]
        [Required(ErrorMessage = "内容を入力してください。")]
        public string Content { get; set; }

        [DisplayName("商談状況")]
        [Required(ErrorMessage = "内容を入力してください。")]
        public string NegotiationStatus { get; set; }

        [DisplayName("出勤時刻")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? AttendanceTime { get; set; }

        [DisplayName("退勤時刻")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? LeavingTime { get; set; }
    }

    public class ReportsDetailsViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        [DisplayName("日付")]
        [DataType(DataType.Date)]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        public string Title { get; set; }

        [DisplayName("内容")]
        public string Content { get; set; }

        [DisplayName("商談状況")]
        public string NegotiationStatus { get; set; }

        [DisplayName("出勤時刻")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? AttendanceTime { get; set; }

        [DisplayName("退勤時刻")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? LeavingTime { get; set; }

        [DisplayName("作成日付")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }

        [DisplayName("更新日付")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [DisplayName("承認状況")]
        public string ApprovalStatus { get; set; }

        public bool Approvable { get; set; }

        [DisplayName("リアクション数")]
        public Dictionary<string, int> ReactionQuantity { get; set; }

        public Dictionary<string, bool> ReactionFlag { get; set; }

        public Dictionary<string, string> ReactionString { get; set; }

        public bool IsReportCreater { get; set; }
    }

    public class ReportsEditViewModel
    {

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("日付")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "日報の日付を入力してください。")]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        [Required(ErrorMessage = "タイトルを入力してください。")]
        [StringLength(100, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string Title { get; set; }

        [DisplayName("内容")]
        [Required(ErrorMessage = "内容を入力してください。")]
        public string Content { get; set; }

        [DisplayName("商談状況")]
        [Required(ErrorMessage = "内容を入力してください。")]
        public string NegotiationStatus { get; set; }

        [DisplayName("出勤時刻")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? AttendanceTime { get; set; }

        [DisplayName("退勤時刻")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? LeavingTime { get; set; }
    }
}