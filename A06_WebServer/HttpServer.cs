/* FILE: HttpServer.cs
 * DATE: Nov 2020
 * AUTHORS: Joel Smith & Ian Ewing
 * PROJECT: WDD A06 Web Server
 * DESCRIPTION: main server logic and functionality here
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace A06_WebServer
{
    /// <summary>
    /// to be used for functions related to webserver
    /// </summary>
    public class HttpServer
    {
        //assigned only in constructor
        readonly string webRoot;
        readonly IPAddress webIP;
        readonly int webPort;

        public Logger.HttpServerLogger serverLog;

        private static TcpListener serverListener;
        Socket clientSocket;

        /// <summary>
        /// constructor for HttpServer that takes command line arguments
        /// </summary>
        /// <param name="serverRoot">command line arg for root of server directory</param>
        /// <param name="serverIP">command line arg as IPAddress for serverIP </param>
        /// <param name="serverPort">port number for server</param>
        public HttpServer(string serverRoot, IPAddress serverIP, int serverPort)
        {
            webRoot = serverRoot;
            webIP = serverIP;
            webPort = serverPort;
            serverLog = new Logger.HttpServerLogger("./myOwnWebServer.log");
        }

        /// <summary>
        /// start up server, listen?
        /// </summary>
        public void Init()
        {
            try
            {
                //initialize TcpListener
                serverListener = new TcpListener(webIP, webPort);
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
         * Listen for the browser request, ensure it meets our needs, 
         * pass to ParseRequest for heavy lifting
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
                //Establish a socket and listen for connections
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
                        SendResponse(statusCode, "<h2>405: Method Not Allowed</h2>");
                        clientSocket.Close(); //This might need to come out?
                        return; //Maybe find a different way to do this? break?
                    }
                    //Grab the location of HTTP within the request string
                    index = buffer.IndexOf("HTTP");

                    //Grab the 8 characters comprising the HTTP version
                    version = buffer.Substring(index, 8);

                    //Will grab a substring from beginning to just before position of the HTTP version
                    target = buffer.Substring(0, (index - 1));
                    //Grab the index of the last forward slash + 1
                    index = (target.LastIndexOf("/") + 1);
                    //This will grab the string beginning with the first character of the filename
                    target = target.Substring(index);

                    //Log the http verb and the requested resource
                    serverLog.Log($"[REQUEST] HTTP Verb {verb} Resource: {target}");

                    //webRoot works here
                    Request browserRequest = new Request(target, webRoot);

                    //Pass our request string into ParseRequest to find out what directory and filetype to retrieve.
                    ParseRequest(browserRequest);
                }
            }
        }

        /// <summary>
        /// takes a request object and turns it into a response by transferring relevant metadata,
        /// measures the length and takes the bytes
        /// </summary>
        /// <param name="inputReq">request object to turn into response</param>
        public void ParseRequest(Request inputReq)
        {
            //declare our return
            Response returnResponse;
            
            //Grab the file we're searching for
            string targetFile = inputReq.startLine.Target;
            string mimeType = MimeMapping.GetMimeMapping(targetFile);
            string filePath = webRoot + @"/" + targetFile;
            int messageLength;

            if (File.Exists(filePath) == false) //The file doesn't exist, classic 404
            {
                //Return a 404 here to browser
                SendResponse(404, "<h2>404: Not Found</h2>");
            }
            else if (mimeType.Contains("text") || mimeType.Contains("image")) //Filter here if contains text
            {
                //get the length
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                messageLength = (int)fs.Length;
                
                //make Response object for text
                returnResponse = new Response(1.1, 200, mimeType, messageLength);

                BinaryReader reader = new BinaryReader(fs);
                //Create an array of bytes equal in size to the length of the file stream
                Byte[] bytes = new byte[fs.Length];

              
                //Do a binaryread.read
                reader.Read(bytes, 0, bytes.Length);
                
                //SendResponse(200, fileContents);

                returnResponse.FillBody(bytes);
                NewSendResponse(returnResponse);
                reader.Close();
                fs.Close();
            }
            else
            {
                SendResponse(415, "<h2>415: Unsupported Media Type</h2>"); // Unsupported Media Type
            }

        }


        /// <summary>
        /// working sendresponse
        /// </summary>
        /// <param name="serverSend">the response to send back</param>
        public void NewSendResponse(Response serverSend)
        {
            double version = serverSend.startLine.Version;
            int contentLength = Int32.Parse(serverSend.headers["Content-Length"]);
            string contentType = serverSend.headers["Content-Type"];
            int statusCode = serverSend.startLine.Code;
            
            string dateString = DateTime.Now.ToString();

            //Send just the header to the client. This allows us to send back negative status codes too.
            string header = $"HTTP/{version} {statusCode}\r\n" + $"Date: {dateString}\r\n" + $"Content-Type: {contentType}\r\n" + $"Content-Length: {contentLength}\r\n\r\n";
            Byte[] msg = Encoding.UTF8.GetBytes(header);
            clientSocket.Send(msg);

            //Send the actual contents of the webpage requested
            clientSocket.Send(serverSend.bodyBytes);

            if (statusCode != 200)
            {
                //If we're in this block, there was an issue. We need only to log the status code.
                serverLog.Log($"[RESPONSE] { statusCode }"); //Log the failed status code
            }
            else
            {
                //Remove our carriage returns/new lines so we can log all in one nice tidy line
                header = header.Replace("\n", " ");
                header = header.Replace("\r", "");
                serverLog.Log($"[RESPONSE] {header}");
            }
            clientSocket.Close(); //needed to have repeated requests
        }

        /// <summary>
        /// currently only used for sending errors
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="response"></param>
        public void SendResponse(int statusCode, string response)
        {
            //Only grab the string length if there's actual content to send to client
            int contentLength = response.Length;
            string dateString = DateTime.Now.ToString();

            //Send just the header to the client. This allows us to send back negative status codes too.
            string header = $"HTTP/1.1 {statusCode}\r\n" + $"Date: {dateString}\r\n" + $"Content-Type: text/html\r\n" + $"Content-Length: {contentLength}\r\n\r\n";
            Byte[] msg = Encoding.UTF8.GetBytes(header);
            clientSocket.Send(msg);

            //Send the actual contents of the webpage requested
            string contents = response;
            msg = Encoding.UTF8.GetBytes(contents);
            clientSocket.Send(msg);
            
            if (statusCode != 200)
            {
                //If we're in this block, there was an issue. We need only to log the status code.
                serverLog.Log($"[RESPONSE] { statusCode }"); //Log the failed status code
            }
            else
            {
                //Remove our carriage returns/new lines so we can log all in one nice tidy line
                header = header.Replace("\n", " ");
                header = header.Replace("\r", "");
                serverLog.Log($"[RESPONSE] {header}");
            }

        }

        /// <summary>
        /// clears up server to exit
        /// </summary>
        public void Close()
        {
            serverListener.Stop();


            serverLog.Log("Closing server");
        }



        

    }
}
