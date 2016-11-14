using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDOANotificationEmailer.Data;
using LDOANotificationEmailer.Net;
using System.Net.Mail;
using LDOANotificationEmailer.Logs;

namespace LDOANotificationEmailer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if log file exists, if not create it.  If there is an issue in this and it could not create and returns false, then send an email
            if (!Logger.CheckForLogFile())
            {
                SmtpClient exchmail = new SmtpClient();

                exchmail.Host = "exchmail.delta.edu";
                exchmail.Port = 25;

                MailMessage OutGoing = new MailMessage();

                OutGoing.Sender = new MailAddress("error@delta.edu ");
                OutGoing.To.Add(new MailAddress("help@delta.edu"));
                //OutGoing.To.Add(new MailAddress("michaelmccloskey@delta.edu"));
                OutGoing.Subject = "Error with LDOA Notification";
                OutGoing.From = new MailAddress("error@delta.edu ");
                OutGoing.Body = "<p>No log file exists for LDOANotificationEmail.  Create LDOANotificationEmail.log file in Logs folder and run the process again manually.</p>";
                OutGoing.IsBodyHtml = true;

                exchmail.Send(OutGoing);
            }
            else
            {

                try
                {
                    string term = DataAccess.GetCurrentTerm();
                    // string term = "16/FA";

                    // Skip emailing faculty and division chairs if there is no current term
                    if (term != "")
                    {
                        // Initialize list of recipients for log
                        List<string> recipientData = new List<string>();

                        var usersToProcess = DataAccess.GetOutstandingLDOA(term);
                        if (usersToProcess.Count == 0)
                        {
                            // No users to process to write this to log
                            Logger.LogError("There were zero outstanding notifications to process.");

                            SmtpClient exchmail = new SmtpClient();

                            exchmail.Host = "exchmail.delta.edu";
                            exchmail.Port = 25;

                            MailMessage OutGoing = new MailMessage();

                            OutGoing.Sender = new MailAddress("error@delta.edu ");
                            OutGoing.To.Add(new MailAddress("help@delta.edu"));
                            //OutGoing.To.Add(new MailAddress("michaelmccloskey@delta.edu"));
                            OutGoing.Subject = "LDOA Notification - Verify Zero Outstanding";
                            OutGoing.From = new MailAddress("error@delta.edu ");
                            OutGoing.Body =
                                "<p>There were zero outstanding notifications to process.  Verify if this is true in the database.</p>";
                            OutGoing.IsBodyHtml = true;

                            exchmail.Send(OutGoing);
                        }
                        else
                        {
                            // Log the LDOA dataset for tracking and validation
                            //string dataHeader = "StudentName, StudentEmail, StudentNumber, StudentGrade, InstructorName, InstructorEmail, CourseSection, Term, DateDropped";
                            //Logger.LogLDOAHeader(dataHeader);

                            // Get each row of data from GetUnifinishedLDOA stored procedure and log it
                            //foreach (ERPDataSet.GetUnfinishedLDOARow thisItem in usersToProcess)
                            //{
                            //    string dataRow = thisItem.StudentName + ", " + thisItem.StudentEmail + ", " + thisItem.StudentNumber + ", " + thisItem.StudentGrade + ", " +
                            //      thisItem.InstructorName + ", " + thisItem.InstructorEmail + ", " + thisItem.CourseSection + ", " + thisItem.Term + ", " +
                            //      thisItem.DateDropped;
                            //    Logger.LogLDOADataSet(dataRow);
                            //}
                            // Goes with dataset logging in line above
                            //Logger.LogLWriteLine(System.DateTime.Now + " - CHECK FOR ERROR BELOW, IF NEW DATA SET, WE GOOD");

                            Logger.LogWriteLine(System.DateTime.Now + " - Email Recipient Info For This Run:");
                            
                            // Figure out the email recipient and setup email if date since drop matches # from xml file and execute send then logging
                            int droppedStudentCount = 0;
                            foreach (ERPDataSet.GetUnfinishedLDOARow thisItem in usersToProcess)
                            {
                                // Determine how long it has been since this has been run
                                int daysSinceDrop = DateTime.Now.Subtract(thisItem.DateDropped).Days;
                                DateTime lastDate = thisItem.DateDropped.AddDays(14);

                                // Determine which notification to send
                                SendEmail.NoticeType noticeToSend = SendSettings.GetNoticeType(daysSinceDrop);
                                List<string> emailRecipients = new List<string>();

                                // Add faculty email recipient
                                if (SendSettings.SendFacultyEmailOnDay(daysSinceDrop))
                                {
                                    droppedStudentCount++;
                                    //First we need to get the instructors email and build info for log list

                                    emailRecipients.Add(thisItem.InstructorEmail);

                                    //emailRecipients.Add("michaelmccloskey@delta.edu");
                                    recipientData.Add(droppedStudentCount + " Instructor - " + thisItem.InstructorEmail +
                                                      ", Student - " + thisItem.StudentName + ", Course = " +
                                                      thisItem.CourseSection);
                                }

                                // If necessary, also add division chair as email recipient
                                if (SendSettings.SendDivisionChairEmailOnday(daysSinceDrop))
                                {
                                    //Next we need to get the division chairs information and add info for log list
                                    string divisionChairEmail =
                                        GetDivisionChairInfo.GetDivisionChairEmail(thisItem.CourseSection);

                                    if (divisionChairEmail != "")
                                    {
                                        emailRecipients.Add(divisionChairEmail);

                                        //emailRecipients.Add("michaelmccloskey@delta.edu");
                                        recipientData.Add(droppedStudentCount + " Division Chair - " + divisionChairEmail +
                                                          ", Student - " + thisItem.StudentName + ", Course = " +
                                                          thisItem.CourseSection);
                                    }
                                }

                                // If we have faculty and division chair recipients in list, send emails
                                if (emailRecipients.Any())
                                {
                                    SendEmail.SendNotice(noticeToSend, emailRecipients, thisItem.InstructorName, thisItem.StudentName, thisItem.StudentNumber.ToString(), thisItem.CourseSection, thisItem.DateDropped.ToString("MM/dd/yyyy"), lastDate.ToString("MM/dd/yyyy"));
                                }
                            }

                            // Log how many emails were sent and the info for who they went to and for what student and course
                            Logger.LogWriteLine("Number of emails sent: " + recipientData.Count);
                            foreach (var line in recipientData)
                            {
                                Logger.LogWriteLine(line);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // We hit an error, so send an email to notify helpdesk about this for us to look into
                    // Construct error message
                    string errorMessage;

                    // Uncomment code below if there is an inner exception to have it print when debugging
                    // Check for inner exception, SQL errors usually reside here
                    //if (ex.InnerException.ToString() == "")
                    //{
                        errorMessage = Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
                                              "Stack Trace:" + Environment.NewLine + ex.StackTrace;
                    //}
                    //else
                    //{
                    //    errorMessage = Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
                    //                          "Inner Exception:" + Environment.NewLine + ex.InnerException +
                    //                          Environment.NewLine + Environment.NewLine + "Stack Trace:" +
                    //                          Environment.NewLine + ex.StackTrace;
                   //}

                    // Log error message
                    Logger.LogError(errorMessage);

                    // Send error message in email
                    SmtpClient exchmail = new SmtpClient();

                    exchmail.Host = "exchmail.delta.edu";
                    exchmail.Port = 25;

                    MailMessage OutGoing = new MailMessage();

                    OutGoing.Sender = new MailAddress("error@delta.edu ");
                    OutGoing.To.Add(new MailAddress("help@delta.edu"));
                    //OutGoing.To.Add(new MailAddress("michaelmccloskey@delta.edu"));
                    OutGoing.Subject = "Error with LDOA Notification";
                    OutGoing.From = new MailAddress("error@delta.edu ");
                    OutGoing.Body = "<p>" + errorMessage + "</p>";
                    OutGoing.IsBodyHtml = true;

                    exchmail.Send(OutGoing);
                }
            }
        }
    }
}