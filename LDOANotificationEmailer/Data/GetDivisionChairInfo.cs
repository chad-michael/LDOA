using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LDOANotificationEmailer.Data.ERPDataSetTableAdapters;

namespace LDOANotificationEmailer.Data
{
    public static class GetDivisionChairInfo
    {
        public static string GetDivisionChairEmail(string courseSection)
        {
            string email = "";
            using (GetDivisionChairEmailTableAdapter db = new GetDivisionChairEmailTableAdapter())
            {
                var result = db.GetData(courseSection.Split("-".ToCharArray())[0]).ToList();
                if (result.Any())
                {
                    email = result.First().DeltaEmail ?? "";
                }
            }

            return email;
        }
    }
}