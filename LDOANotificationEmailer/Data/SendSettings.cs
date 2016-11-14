using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using LDOANotificationEmailer.Net;

namespace LDOANotificationEmailer.Data
{
    public static class SendSettings
    {
        private static List<int> _FacultyDays;
        private static List<int> _DivisionChairDays;
        private static List<int> _FirstNoticeDays;
        private static List<int> _SecondNoticeDays;
        private static List<int> _ThirdNoticeDays;
        private static List<int> _FourthNoticeDays;
        private static List<int> _FifthNoticeDays;

        private static List<int> FirstNoticeDays
        {
            get
            {
                if (_FirstNoticeDays == null)
                {
                    _FirstNoticeDays = GetListOfValuesFromElement("firstNotice");
                }

                return _FirstNoticeDays;
            }
        }

        private static List<int> SecondNoticeDays
        {
            get
            {
                if (_SecondNoticeDays == null)
                {
                    _SecondNoticeDays = GetListOfValuesFromElement("secondNotice");
                }

                return _SecondNoticeDays;
            }
        }

        private static List<int> ThirdNoticeDays
        {
            get
            {
                if (_ThirdNoticeDays == null)
                {
                    _ThirdNoticeDays = GetListOfValuesFromElement("thirdNotice");
                }

                return _ThirdNoticeDays;
            }
        }

        private static List<int> FourthNoticeDays
        {
            get
            {
                if (_FourthNoticeDays == null)
                {
                    _FourthNoticeDays = GetListOfValuesFromElement("fourthNotice");
                }

                return _FourthNoticeDays;
            }
        }

        private static List<int> FifthNoticeDays
        {
            get
            {
                if (_FifthNoticeDays == null)
                {
                    _FifthNoticeDays = GetListOfValuesFromElement("fifthNotice");
                }

                return _FifthNoticeDays;
            }
        }

        private static List<int> FacultyDays
        {
            get
            {
                if (_FacultyDays == null)
                {
                    _FacultyDays = GetListOfValuesFromElement("sendInstructorEmail");
                }

                return _FacultyDays;
            }
        }

        private static List<int> DivisionChairDays
        {
            get
            {
                if (_DivisionChairDays == null)
                {
                    _DivisionChairDays = GetListOfValuesFromElement("sendDivisionChairEmail");
                }

                return _DivisionChairDays;
            }
        }

        public static bool SendFacultyEmailOnDay(int day)
        {
            if (FacultyDays.Contains(day))
            {
                return true;
            }
            return false;
        }

        public static bool SendDivisionChairEmailOnday(int day)
        {
            if (DivisionChairDays.Contains(day))
            {
                return true;
            }
            return false;
        }

        public static SendEmail.NoticeType GetNoticeType(int day)
        {
            if (FirstNoticeDays.Contains(day))
            {
                return SendEmail.NoticeType.FirstNotice;
            }
            else if (SecondNoticeDays.Contains(day))
            {
                return SendEmail.NoticeType.SecondNotice;
            }
            else if (ThirdNoticeDays.Contains(day))
            {
                return SendEmail.NoticeType.ThirdNotice;
            }
            else if (FourthNoticeDays.Contains(day))
            {
                return SendEmail.NoticeType.FourthNotice;
            }
            else if (FifthNoticeDays.Contains(day))
            {
                return SendEmail.NoticeType.FifthNotice;
            }
            else
            {
                return SendEmail.NoticeType.NoNotice;
            }
        }

        private static List<int> GetListOfValuesFromElement(string sElement)
        {
            List<int> returnValues = new List<int>();
            XDocument settingsDoc = XDocument.Load("Data\\MailSettings.xml");
            XElement divisionChairElement = settingsDoc.Root.Elements(sElement).First();
            string[] values = divisionChairElement.Attributes("days").First().Value.Split(",".ToCharArray());

            foreach (string value in values)
            {
                returnValues.Add(Int32.Parse(value));
            }

            return returnValues;
        }
    }
}