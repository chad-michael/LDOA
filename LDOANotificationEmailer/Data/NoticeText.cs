using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LDOANotificationEmailer.Net;

namespace LDOANotificationEmailer.Data
{
    public static class NoticeText
    {
        public static string GetNoticeText(SendEmail.NoticeType noticeType)
        {
            string textToReturn = "";

            switch (noticeType)
            {
                case SendEmail.NoticeType.FirstNotice:
                    textToReturn = File.ReadAllText("Data/FirstNotice.htm");
                    break;
                case SendEmail.NoticeType.SecondNotice:
                    textToReturn = File.ReadAllText("Data/SecondNotice.htm");
                    break;
                case SendEmail.NoticeType.ThirdNotice:
                    textToReturn = File.ReadAllText("Data/ThirdNotice.htm");
                    break;
                case SendEmail.NoticeType.FourthNotice:
                    textToReturn = File.ReadAllText("Data/FourthNotice.htm");
                    break;
                case SendEmail.NoticeType.FifthNotice:
                    textToReturn = File.ReadAllText("Data/FifthNotice.htm");
                    break;
            }

            return textToReturn;
        }

        public static string GetSubjectText(SendEmail.NoticeType noticeType)
        {
            string textToReturn = "";

            switch (noticeType)
            {
                case SendEmail.NoticeType.FirstNotice:
                    textToReturn = File.ReadAllText("Data/FirstNoticeSubject.txt");
                    break;
                case SendEmail.NoticeType.SecondNotice:
                    textToReturn = File.ReadAllText("Data/SecondNoticeSubject.txt");
                    break;
                case SendEmail.NoticeType.ThirdNotice:
                    textToReturn = File.ReadAllText("Data/ThirdNoticeSubject.txt");
                    break;
                case SendEmail.NoticeType.FourthNotice:
                    textToReturn = File.ReadAllText("Data/FourthNoticeSubject.txt");
                    break;
                case SendEmail.NoticeType.FifthNotice:
                    textToReturn = File.ReadAllText("Data/FifthNoticeSubject.txt");
                    break;
            }

            return textToReturn;
        }
    }
}