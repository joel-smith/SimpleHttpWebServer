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
        Logger.HttpServerLogger serverLog;

        private static TcpListener serverListener;

        //assigned only in constructor
        readonly string webRoot;
        readonly IPAddress webIP;
        readonly int webPort; 
       

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
                    //Grab the index of the last forward slash + 1
                    index = (target.LastIndexOf("/") + 1);
                    //This will grab the string beginning with the first character of the filename
                    target = target.Substring(index);

                    //Log the http verb and the requested resource
                    serverLog.Log($"HTTP Verb {verb} Resource: {target}");

                    Request browserRequest = new Request(target, "localhost");//May need to adjust this instead of "localhost" being hardcoded.

                    //Pass our request string into ParseRequest to find out what directory and filetype to retrieve.
                    ParseRequest(browserRequest);
                }
            }
        }

        /// <summary>
        /// basic function to parse the request on the http server
        /// Calls SendResponse() based off the mimtype of the file requested
        /// no return
        /// </summary>
        /// <param name="Request"></param>
        public void ParseRequest(Request Request)
        {

            //Grab the file we're searching for
            string targetFile = Request.startLine.Target;

            //Theoretically this should get the file type extension
            //but cosmic rays so...
            string mimeType = MimeMapping.GetMimeMapping(targetFile);
            Console.WriteLine(mimeType);
            //Gotta grab the -webroot here somehow. Will figure that out later
            //Maybe include the -webroot in the Request object?
            string filePath = @"C:\webServerResources\" + targetFile;

            if (File.Exists(filePath) == false) //The file doesn't exist, give them the classic 404
            {
                //Return a 404 here to browser
                SendResponse(404, null);
            }
            else if (mimeType.Contains("text")) //Filter here if contains text
            {
                int totalBytesRead = 0;
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                BinaryReader reader = new BinaryReader(fs);
                //Create an array of bytes equal in size to the length of the file stream
                Byte[] bytes = new byte[fs.Length];

                int byteCountLoop;
                string fileContents = "";
                //Do a binaryread.read
                while((byteCountLoop = reader.Read(bytes, 0, bytes.Length)) != 0)
                {
                    fileContents += Encoding.ASCII.GetString(bytes, 0, byteCountLoop);
                    totalBytesRead += byteCountLoop;
                }
                SendResponse(200, fileContents);
                reader.Close();
                fs.Close();
            }
            else if (mimeType.Contains("image"))
            {
                //Enter if requested file is image (jpeg or gif)
            }
            else
            {
                //This will catch things like .aspx requests
                SendResponse(415, null); // Unsupported Media Type
            }
        }

        /// <summary>
        /// Is this the sending back of a response?
        /// Consider changing the name to SendResponse()?
        /// </summary>
        public void SendRequest()
        {
            //request object/class needed maybe?
        }

        public void SendResponseObject(Response serverSend)
        {

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
            int contentLength = response.Length;
            string dateString = DateTime.Now.ToString();

            //sw.WriteLine($"Content-type:text");
            ////sw.WriteLine($"Content-Type:{contentType}"); //Will need to either be parsed from string response, or passed in separately?
            //sw.WriteLine($"Server:JSmith-IEwing-Server9000"); //This should always be the same?
            //sw.WriteLine($"Content-Length:{contentLength}"); //This should be easy to grab? SizeOf response?
            //sw.WriteLine($"Date:{dateString}");

            //Byte[] msg = Encoding.UTF8.GetBytes("HTTP/1.1 200\r\n");
            //clientSocket.Send(msg);
            //msg = Encoding.UTF8.GetBytes($"Content-Type:text\r\nServer:JSmmith-IEwing-Server9000\r\nContent-Length:{contentLength}\r\nDate:{dateString}\r\n");
            //clientSocket.Send(msg);
            //msg = Encoding.UTF8.GetBytes(response);
            //clientSocket.Send(msg);

            string message = $"HTTP/1.1 {statusCode}\r\n" + $"Date: {dateString}\r\n" + $"Content-Type: text/html\r\n" + $"Content-Length: {contentLength}\r\n\r\n" + response;
            Byte[] msg = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(msg);
        }

        /// <summary>
        /// clears up server to exit
        /// </summary>
        public void Close()
        {
            serverListener.Stop();
            clientSocket.Close();
            
            serverLog.Log("Closing server");
        }

    }
}
