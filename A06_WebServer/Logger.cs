/*
* FILENAME: Logger.cs
* PROJECT: WDDA06 Server
* BY: Joel Smith
* DATE: Nov 17 2020
* DESCRIPTION: logger for http server
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
        class Logger
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
        /// Logger functionality to be used for HttpServer
        /// </summary>
        public class HttpServerLogger : LoggerCore
        {
          
            public bool logValid = false; //has the log file been verified
            public string filePath; //where is the log

            public void Init(string logPath)
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
                                byte[] info = new UTF8Encoding(true).GetBytes("Initialized HTTP Server Log");
                                // Add some info to the file.
                                fs.Write(info, 0, info.Length);
                                this.logValid = true;
                            }
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
                        this.logValid = true;
                    }
                }
            }


            /// <summary>
            /// safely puts the message/log line into the file
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
                            sw.WriteLine(message);
                            sw.Close();
                        }
                    }
                }
            }
        }
    }
}
