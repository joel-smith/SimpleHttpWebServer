/* 
 * FILE:            HttpServer.cs
 * PROJECT:         WDD A06 Web Server
 * AUTHORS:         Joel Smith & Ian Ewing
 * DATE:            Nov 18, 2020
 * DESCRIPTION:     This class contains the main server logic and functionality. This is 
 *                  where the single threaded server is started. It is responsible for the 
 *                  parsing of client requests, retrieval of requested data, as well as the 
 *                  creation and transmission of our response. Its the workhorse
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

        //Will allow us to log any pertinent events
        public Logger.HttpServerLogger serverLog;

        //Both of these exist to allow for connection of client-browser
        private static TcpListener serverListener;
        Socket clientSocket;

        Response errorResponse;

        /// <summary>
        /// constructor for HttpServer that takes command line arguments
        /// </summary>
        /// <param name="serverRoot">command line arg for root of server directory</param>
        /// <param name="serverIP">command line arg as IPAddress for serverIP </param>
        /// <param name="serverPort">port number for server</param>
        /// <param name="serverLog">directory for all of our server logs to be written</param>
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
                //Log any exception that was thrown
                serverLog.Log("Exception occured initializing TcpListener for Server : " + e.ToString());
            }
        }



        /*
         * Method:          GetRequest
         * Description:     Listen for the browser request, ensure it meets our needs by  
         *                  seperating the request into substrings before it passes to 
         *                  ParseRequest for heavy lifting
         * Parameter:       None
         * Returns:         Void
         */
        private void GetRequest()
        {
            int index = 0; //Will allow us to grab substrings from input
            string target = null; //The targeted file the user wants
            string version = null; //HTTP version
            string verb = null; //Method for data transmission
            int statusCode = 0; //HTTP status codes

            while (Run.Go)
            {
                try
                {

                    //Establish a socket and listen for connections
                    clientSocket = serverListener.AcceptSocket();

                    if (clientSocket.Connected)
                    {
                        //Array of bytes to hold data received
                        Byte[] bytes = new byte[1024];

                        //Store the data in the new bytes array
                        clientSocket.Receive(bytes, bytes.Length, 0);

                        //Translate the received bytes into a HTTP request string
                        string buffer = Encoding.ASCII.GetString(bytes);

                        //Check to make sure the request was using GET
                        if (buffer.IndexOf("GET") == -1)
                        {
                            statusCode = 405;
                            string error = "<h2>405: Method Not Allowed</h2>";
                            Byte[] errorBytes = Encoding.UTF8.GetBytes(error);
                            int messageLength = error.Length;
                            errorResponse = new Response(1.1, statusCode, "text/html", messageLength, errorBytes);
                            clientSocket.Close();
                            break;
                        }
                        else
                        {
                            //Grab the HTTP method and store it. 
                            verb = buffer.Substring(0, 3); //3 = num of characters in GET
                        }
                        //Grab the location of HTTP within the request string
                        index = buffer.IndexOf("HTTP");

                        //Grab the 8 characters comprising the HTTP version
                        version = buffer.Substring(index, 8);
                        if (version != "HTTP/1.1")
                        {
                            statusCode = 505;
                            string error = "<h2>505: HTTP Version Not Supported</h2>";
                            Byte[] errorBytes = Encoding.UTF8.GetBytes(error);
                            int messageLength = error.Length;
                            errorResponse = new Response(1.1, statusCode, "text/html", messageLength, errorBytes);
                            clientSocket.Close();
                            break;
                        }

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
                catch (SocketException ex)
                {
                    Thread.Sleep(5);
                    serverLog.Log($"Ignore this");
                }
                catch (Exception e)
                {
                    serverLog.Log($"[ERROR] {e.ToString()}");
                }
            }
        }



        /*
         * Method:          ParseRequest
         * Description:     Takes a request object filled by GetRequest and creates the corresponding
         *                  response object by transferring relevant metadata and processing the requested
         *                  files into bytes. This information is stored in a Response object.
         * Parameters:      Request inputReq - Request object representing the request sent to the server by client
         * Returns:         Void
         */
        public void ParseRequest(Request inputReq)
        {
            //declare our return
            Response returnResponse;
            //Declare some integers that will be used
            int messageLength;
            int statusCode;

            //Grab the file we're searching for
            string targetFile = inputReq.startLine.Target;
            //Checks if the there was no requested target URL
            if (targetFile == "")
            {
                //If true, we direct the request to our index.
                targetFile = "index.html";
            }

            //Grab our mime type and file path.
            string mimeType = MimeMapping.GetMimeMapping(targetFile);
            string filePath = webRoot + @"/" + targetFile;

            if (File.Exists(filePath) == false) //The file doesn't exist, classic 404
            {
                //Build a response object for the error message
                statusCode = 404;
                string error = "<h2>404: Not Found</h2>";
                Byte[] bytes = Encoding.UTF8.GetBytes(error);
                messageLength = error.Length;
                errorResponse = new Response(1.1, statusCode, "text/html", messageLength, bytes);
                SendResponse(errorResponse);
            }
            else if (mimeType.Contains("text/html") || mimeType.Contains("text/plain") || mimeType.Contains("image/jpeg") || mimeType.Contains("image/gif"))
            { //Filter into here if our media type is one accepted by our server
                //get the length
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                messageLength = (int)fs.Length;
                statusCode = 200; //Sucessful request
                
                //make Response object for text
                

                BinaryReader reader = new BinaryReader(fs);
                //Create an array of bytes equal in size to the length of the file stream
                Byte[] bytes = new byte[fs.Length];

                reader.Read(bytes, 0, bytes.Length);

                //Create an object to hold all pertinent response information
                returnResponse = new Response(1.1, statusCode, mimeType, messageLength, bytes);

               // returnResponse.FillBody(bytes);
                SendResponse(returnResponse);
                //Close all our resources
                reader.Close();
                fs.Close();
            }
            else
            {
                //Build a response object for the error message
                statusCode = 415;
                string error = "<h2>415: Unsupported Media Type</h2>";
                Byte[] bytes = Encoding.UTF8.GetBytes(error);
                messageLength = error.Length;
                errorResponse = new Response(1.1, statusCode, "text/html", messageLength, bytes);
                SendResponse(errorResponse);
            }

        }



        /*
         * Method:          SendResponse
         * Description:     Takes in a response object populated by ParseRequest. This method takes
         *                  the metadata, creates a header then transmits both the header and file 
         *                  in byte form to the client. The method checks the status code before creating 
         *                  the proper log entry based off success state. Finishes by closing the socket 
         *                  connection
         * Paramters:       Response serverSend - Response object containing the information needed to 
         *                  send a response to the client.
         * Returns:         Void
         */
        public void SendResponse(Response serverSend)
        {
            //Grab local copies of information needed to send our response
            double version = serverSend.startLine.Version;
            int contentLength = Int32.Parse(serverSend.headers["Content-Length"]);
            string contentType = serverSend.headers["Content-Type"];
            int statusCode = serverSend.startLine.Code;
            
            //Format our datetime string to display day of the week, 24 hour clock, and timezone
            string dateString = DateTime.Now.ToString("ddd, dd MMM yyyy H:mm:ss %K");

            //Send just the header to the client. This allows us to send back negative status codes too.
            string header = $"HTTP/{version} {statusCode}\r\n" + $"Date: {dateString}\r\n" + $"Content-Type: {contentType}\r\n" + $"Content-Length: {contentLength}\r\n\r\n";
            Byte[] msg = Encoding.UTF8.GetBytes(header);
            
            //Send our header
            clientSocket.Send(msg);
            //Send the actual contents of the webpage requested
            clientSocket.Send(serverSend.bodyBytes);

            //We all good in the hood
            if (statusCode != 200)
            {
                //If we're in this block, there was an issue. We need only to log the status code.
                serverLog.Log($"[RESPONSE] { statusCode }"); //Log the failed status code
            }
            else
            {

                serverLog.Log($"[RESPONSE] Content-Type: {contentType} Content-Length: {contentLength} Server: J&I Servers'R'Us Date: {dateString}");
            }
            clientSocket.Close(); //needed to have repeated requests
        }


        
        /*
         * Method:          Close
         * Description:     Handles safe shutdown of the server
         * Parameters:      None
         * Returns:         Void
         */
        public void Close()
        {
            Run.Go = false;

            serverListener.Stop();

            serverLog.Log("[SERVER STOPPED]");
        }

    }
}
