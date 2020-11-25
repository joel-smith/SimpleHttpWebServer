using System;
using System.Collections.Generic;
using System.IO;
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
    public class HttpServer
    {
        Logger.HttpServerLogger serverLog;

        private static TcpListener serverListener;

        //I think these make more sense as coming in from run.cs input
        private readonly int port; // = 5050; //Has to be configurable by user input
        private readonly IPAddress ipAddress; //Has to be configurable by user

        Socket clientSocket;


        /// <summary>
        /// constructor for HttpServer class, currently takes one input the log file
        /// </summary>
        public HttpServer()
        {
            
            serverLog = new Logger.HttpServerLogger("C:/temp/myOwnWebServer.log");
        }

        /// <summary>
        /// start up server, listen?
        /// </summary>
        public void Init(IPAddress address, int port)
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
                //Should log our exception that was thrown
                serverLog.Log("Exception occured initializing TcpListener for Server : " + e.ToString());
            }

        }

        /*
         * Listen for the browser request, ensure it meets our needs, pass to ParseRequest for heavy lifting
         * 
         */
        private void GetRequest()
        {
            int index = 0;
            string target = null;
            string version = null;
            string verb = null;
            int statusCode = 0;

            
            while (true)
            {
                clientSocket = serverListener.AcceptSocket();

                if (clientSocket.Connected)
                {
                    //Array of bytes to hold data received
                    Byte[] bytes = new byte[1024];
                    //Store the data in the new bytes array
                    clientSocket.Receive(bytes, bytes.Length, 0);
                    //Translate the received bytes into the HTTP request
                    string buffer = Encoding.ASCII.GetString(bytes);

                    //Grab the HTTP verb from the request and store it
                    verb = buffer.Substring(0, 3);

                    //Check the HTTP verb. If not get, send back response, log, shut down.
                    if (verb != "GET")
                    {
                        statusCode = 405; //405: Method not allowed
                        SendResponse(statusCode, null);
                        clientSocket.Close(); //This might need to come out?
                        return; //Maybe find a different way to do this? break?
                    }
                    //Grab the location of HTTP within the request string
                    index = buffer.IndexOf("HTTP");

                    //Grab the 8 characters comprising the HTTP version
                    version = buffer.Substring(index, 8);

                    //Will grab a substring from beginning to just before position of the HTTP version
                    target = buffer.Substring(0, (index - 1));

                    //Log the http verb and the requested resource
                    serverLog.Log($"HTTP Verb {verb} Resourse: {target}");



                    Request browserRequest = new Request(target, "localhost");//May need to adjust this instead of "localhost" being hardcoded.

                    //Pass our request string into ParseRequest to find out what directory and filetype to retrieve.
                    ParseRequest(browserRequest);
                }
            }
        }

        /// <summary>
        /// basic function to parse the request on the http server
        /// this function can return a HttpRequest object
        /// </summary>
        /// <param name="Request"></param>
        public static void ParseRequest(Request Request)
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
        /// Consider changing the name to SendResponse()?
        /// </summary>
        public void SendRequest()
        {
            //request object/class needed maybe?
        }

        /// I created this so I don't hijack SendRequest in case that was meant to do something different
        /// This will handle sending the reponse back to the browser
        /// takes in the status code and other stuff? string response holding the actual constructed response?
        public void SendResponse(int statusCode, string response)
        {
            //NetworkStream into a streamwriter back to the client?
            NetworkStream stream = new NetworkStream(clientSocket);
            StreamWriter sw = new StreamWriter(stream);


            //This is probably real messy, sorry. Just "coding out loud"
            sw.WriteLine($"HTTP/1.1 {statusCode} ");

            if (statusCode != 200)
            {
                //If we're in this block, there was an issue. We need only to log the status code.
                serverLog.Log($"{ statusCode }"); //Status 405 Method Not Allowed
                return; //Probably wrong? But kick out of SendResponse
            }
            string dateString = DateTime.Now.ToString();

            sw.WriteLine($"Content-Type:{contentType}"); //Will need to either be parsed from string response, or passed in separately?
            sw.WriteLine($"Server:JSmith-IEwing-Server9000"); //This should always be the same?
            sw.WriteLine($"Content-Length:{contentLength}"); //This should be easy to grab? SizeOf response?
            sw.WriteLine($"Date:{dateString}");


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
