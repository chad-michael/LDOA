using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LDOANotificationEmailer.Data;
using System.IO;
using System.Net.Mail;
using LDOANotificationEmailer.Logs;

namespace LDOANotificationEmailer.Net
{
    public static class SendEmail
    {
        public enum NoticeType
        {
            NoNotice, FirstNotice, SecondNotice, ThirdNotice, FourthNotice, FifthNotice
        };

        public static void SendNotice(NoticeType thisNotice, List<string> emailRecipients,
                                        string facultyName, string studentName, string studentID, string courseSection, string dropDate, string lastDate)
        {
            if (thisNotice != NoticeType.NoNotice)
            {
                string messageText = NoticeText.GetNoticeText(thisNotice);
                string subjectText = NoticeText.GetSubjectText(thisNotice);

                messageText = messageText.Replace("FACULTY_NAME", facultyName).Replace("STUDENT_NAME", studentName).Replace("STUDENT_ID", studentID).Replace("COURSE_SECTION", courseSection).Replace("DROP_DATE", dropDate).Replace("LAST_DATE", lastDate);

                SendGenericNotification(subjectText, messageText, emailRecipients);
                // extra insurance to not send emails to faculty when testing or debugging
                //SendGenericNotification(subjectText, messageText, (new string[] {"michaelmccloskey@delta.edu"}).ToList());

            }
        }

        private static void SendGenericNotification(string Subject, string Body, List<string> recipients)
        {
            try
            {
                SmtpClient exchmail = new SmtpClient();

                exchmail.Host = "exchmail.delta.edu";
                exchmail.Port = 25;

                MailMessage OutGoing = new MailMessage();

                OutGoing.Sender = new MailAddress("regis@delta.edu ");
                OutGoing.Bcc.Add(new MailAddress("webadmin@delta.edu"));
                OutGoing.Subject = Subject;
                OutGoing.From = new MailAddress("regis@delta.edu ");
                OutGoing.To.Add(new MailAddress(recipients[0]));
                OutGoing.IsBodyHtml = true;
                OutGoing.Body = Body;
                if (recipients.Count() > 1)
                {
                    OutGoing.CC.Add(new MailAddress(recipients[1]));
                }

                exchmail.Send(OutGoing);
            }
            catch (Exception ex)
            {
                // error attempting to send email, log the error and email helpdesk the error message and what the email message was trying to be.
                string errorMessage = Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ex.StackTrace;
                Logger.LogError(errorMessage);
                SmtpClient exchmail = new SmtpClient();

                exchmail.Host = "exchmail.delta.edu";
                exchmail.Port = 25;

                MailMessage OutGoing = new MailMessage();

                OutGoing.Sender = new MailAddress("error@delta.edu ");
                OutGoing.To.Add(new MailAddress("help@delta.edu"));
                OutGoing.Subject = "Error with LDOA Notification";
                OutGoing.From = new MailAddress("error@delta.edu ");
                OutGoing.Body = "<p>" + errorMessage + "</p><p>" + Body + "</p>";
                OutGoing.IsBodyHtml = true;


                exchmail.Send(OutGoing);
            }
        }

    }
}