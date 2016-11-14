using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using LDOANotificationEmailer.Data.odsDataSetTableAdapters;
using LDOANotificationEmailer.Data.ERPDataSetTableAdapters;

namespace LDOANotificationEmailer.Data
{
    public static class DataAccess
    {
        public class UserInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public static ERPDataSet.GetUnfinishedLDOADataTable GetOutstandingLDOA(string term)
        {
            using (GetUnfinishedLDOATableAdapter db = new GetUnfinishedLDOATableAdapter())
            {
                var outstandingLDOAs = db.GetData(term);
                return outstandingLDOAs;
            }
        }

        public static string GetCurrentTerm()
        {
            using (View_TERMSTableAdapter db = new View_TERMSTableAdapter())
            {
                var possibleTerms = db.GetData().Where(x => x.TERMS_START_DATE < DateTime.Now && x.TERMS_END_DATE > DateTime.Now).OrderBy(X => X.TERMS_START_DATE);

                return possibleTerms.Any() ? possibleTerms.First().TERMS_ID : "";
            }
        }

    }
}