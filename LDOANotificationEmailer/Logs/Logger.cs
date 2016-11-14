using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LDOANotificationEmailer.Logs
{
    public class Logger
    { 
        public static void LogError(string error)
        {            
            //System.Console.WriteLine(error);
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Logs\LODANotificationEmailer.log");
            System.IO.File.AppendAllText(path, Environment.NewLine + System.DateTime.Now + " - ERROR" +Environment.NewLine + error + Environment.NewLine);
        }

        public static void LogLDOADataSet(string dataRow)
        {
            //System.Console.WriteLine(dataRow);           
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Logs\LODANotificationEmailer.log");
            System.IO.File.AppendAllText(path, Environment.NewLine + dataRow);
        }

        public static void LogWriteLine(string line)
        {
            //System.Console.WriteLine(dataRow);           
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"Logs\LODANotificationEmailer.log");
            System.IO.File.AppendAllText(path, Environment.NewLine + line);
        }

        public static void LogLDOAHeader(string dataHeader)
        {
            //System.Console.WriteLine(dataHeader);          
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Logs\LODANotificationEmailer.log");
            System.IO.File.AppendAllText(path, Environment.NewLine + System.DateTime.Now + " - Dataset:" + Environment.NewLine + dataHeader);
        }

        public static bool CheckForLogFile()
        {
            //System.Console.WriteLine("Checking for log file...");            
            string logLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"Logs\LODANotificationEmailer.log");
            // Check if log file exists, if not create it
            if (!System.IO.File.Exists(logLocation))
            {
                System.IO.File.Create(logLocation);
                // check that it was created and then return true so program can continue running, log is ESSENTIAL
                if (!System.IO.File.Exists(logLocation))
                {
                    //System.Console.WriteLine("Log file exists, continue...");                   
                    return true;
                }
                else
                {
                    // no log file exists still and this is a problem!
                    //System.Console.WriteLine("No log file exists, continue...");                    
                    return false;
                }
            }
            else
            {
                //System.Console.WriteLine("Log file exists, continue...");                
                // log file exists so continue
                return true;
            }
        }
    }
}
