using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DailyReportSystem.Models;

namespace DailyReportSystem.Utils
{
    public static class RolesEnumUtil
    {
        public static int GetRoleNum(string role)
        {
            foreach(RolesEnum r in Enum.GetValues(typeof(RolesEnum)))
            {
                if(role == r.ToString())
                {
                    return (int)r;
                }
                
            }

            return (int)RolesEnum.Normal;
        }
    }
}