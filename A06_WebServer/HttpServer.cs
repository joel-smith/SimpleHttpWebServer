using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A06_WebServer
{
    /// <summary>
    /// to be used for functions related to webserver
    /// </summary>
    class HttpServer
    {
        //Might be able to receie this? was encountering issues without it set.
        public Logger.HttpServerLogger serverLog = new Logger.HttpServerLogger("C:/temp/myOwnWebServer.log");

        private static TcpListener serverListener;

        //I think these make more sense as coming in from run.cs input
        private int port = 5050; //Has to be configurable by user input
        private IPAddress ipAddress; //Has to be configurable by user



        /// <summary>
        /// constructor for HttpServer class, currently takes one input the log file
        /// </summary>
        HttpServer()
        {
            
            serverLog = new Logger.HttpServerLogger("C:/temp/myOwnWebServer.log");
        }

        /// <summary>
        /// start up server, listen?
        /// </summary>
        public static void Init(IPAddress address, int port)
        {
            try
            {
                //initialize TcpListener
                serverListener = new TcpListener(address, port);
                serverListener.Start();
                Thread thread = new Thread(new ThreadStart(GetRequest));
                thread.Start();
               //set Listener on Thread
            }
            catch (Exception e)
            {
                //Commented out temporarily. Issues with object instatiation?
                //serverLog.Log("Exception occured initializing TcpListener for Server : " + e.ToString());
            }

        }

        /*
         * Listen for the browser request, ensure it meets our needs, pass to ParseRequest for heavy lifting
         * 
         */
        private static void GetRequest()
        {
            int index = 0;
            string request = null;
            string version = null;

            
            while (true)
            {
                Socket clientSocket = serverListener.AcceptSocket();

                if (clientSocket.Connected)
                {
                    //Array of bytes to hold data received
                    Byte[] bytes = new byte[1024];
                    //Store the data in the new bytes array
                    clientSocket.Receive(bytes, bytes.Length, 0);
                    //Translate the received bytes into the HTTP request
                    string buffer = Encoding.ASCII.GetString(bytes);

                    //Check the first 3 characters of our request. If not get, send back response, log, shut down.
                    if (buffer.Substring(0,3) != "GET")
                    {
                        //serverLog.Log("405"); //Status 405 Method Not Allowed
                        //Send back 405 status code
                        clientSocket.Close();
                        return; //Maybe find a different way to do this? break?
                    }
                    index = buffer.IndexOf("HTTP");

                    //Grab the 8 characters comprising the HTTP version
                    version = buffer.Substring(index, 8);

                    //Will grab a substring from beginning to just before position of the HTTP version
                    request = buffer.Substring(0, (index - 1));
                }
            }
        }

        /// <summary>
        /// basic function to parse the request on the http server
        /// </summary>
        /// <param name="Request"></param>
        public void ParseRequest(string Request)
        {
            //plain text(specifically the .txt extension)
            //HTML files(and their various extensions)
            //JPG images(and their various extensions)
            //GIF

            //make our own HttpError object/class maybe?
            //maybe even make this function be: 
            //public HttpError ParseRequest(string Request)
            //and return void if is successful?
            //maybe need HttpResponse class? HttpError can be a subclass?
        }

        /// <summary>
        /// Is this the sending back of a response?
        /// </summary>
        public void SendRequest()
        {
            //request object/class needed maybe?
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            serverLog.Log("Closing server");
        }

    }
}
