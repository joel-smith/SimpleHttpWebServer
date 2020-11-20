using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A06_WebServer
{
    /// <summary>
    /// to be used for functions related to webserver
    /// </summary>
    class HttpServer
    {
        private TcpListener serverListener;
        private int port = 5050; //make this choosable?
        public Logger.HttpServerLogger serverLog; 


        /// <summary>
        /// constructor for HttpServer class, currently takes one input the log file
        /// </summary>
        HttpServer()
        {
            
            serverLog = new Logger.HttpServerLogger("C:/temp/serverlog.txt");
        }

        /// <summary>
        /// start up server, listen?
        /// </summary>
        public void Init()
        {
            try
            {
               //initialize TcpListener
               //set Listener on Thread
            }
            catch (Exception e)
            {
                serverLog.Log("Exception occured initializing TcpListener for Server : " + e.ToString());
            }

        }

        /// <summary>
        /// basic function to parse the request on the http server
        /// </summary>
        /// <param name="Request"></param>
        public void ParseRequest(string Request)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void SendRequest()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {

        }

    }
}
