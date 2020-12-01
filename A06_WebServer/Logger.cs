/*
* FILE:         Logger.cs
* PROJECT:      WDDA06 Server
* AUTHORS:      Joel Smith & Ian Ewing
* DATE:         Nov 17 2020
* DESCRIPTION:  This class handles all logging for our server functionality
*               It allows for a variety of message strings to be logged with 
*               a consistent date format.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace A06_WebServer
{
        /// <summary>
        /// find the logger type you need in here
        /// </summary>
        public class Logger
        {
        
        /// <summary>
        /// abstract class to be used to base a logger
        /// </summary>
        public abstract class LoggerCore
        {
            protected readonly object lockObj = new object();
            public abstract void Log(string message);
        }

        /// <summary>
        /// Logger functionality to be used by HttpServer
        /// </summary>
        public class HttpServerLogger : LoggerCore
        {
          
            public bool logValid = false; //has the log file been verified
            public string filePath; //where is the log

            public HttpServerLogger(string logPath)
            {
                //lock for safety
                lock (lockObj)
                {
                    if (!File.Exists(logPath))
                    {
                        try
                        {
                            using (FileStream fs = File.Create(logPath))
                            {
                                //need to add datetime stamp
                                string firstEntry = DateTime.Now.ToString() + " : Initialized HTTP Server Log\n";

                                byte[] info = new UTF8Encoding(true).GetBytes(firstEntry);
                                // Add some info to the file.
                                fs.Write(info, 0, info.Length);
                                this.logValid = true;
                            }
                            filePath = logPath; //store log data in class
                        }
                        //cannot write to text if we can't open it, not supposed to write to console?
                        //maybe we can here
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else
                    {
                        //If file exists, delete it so we can start new log
                        File.Delete(logPath);
                        filePath = logPath;
                        this.logValid = true;
                    }
                }
            }



            /// <summary>
            /// safely writes the message/log line into the file
            /// </summary>
            /// <param name="message"></param>
            public override void Log(string message)
            {
                if (this.logValid == true)
                {
                    lock (lockObj)
                    {
                        using (StreamWriter sw = new StreamWriter(filePath, true))
                        {
                            //Uses custom date formatting to meet specs
                            sw.WriteLine(DateTime.Now.ToString("yyyy-M-d H:mm:s ") + message);
                            sw.Close();
                        }
                    }
                }
            }
        }
    }
}
