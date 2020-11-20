﻿/*
 Filename: Run.cs
 Description: holds main for the single threaded web server.


Command Line Arguments:
–webRoot : which will be set to the root folder for the website data (html, jpg and gif files)
–webIP : which will be set to the IP address of the server
–webPort : which will be set to the Port number the server will be listening on

example: myOwnWebServer –webRoot=C:\localWebSite –webIP=192.168.100.23 –webPort=5300

Must be able to handle these parametes in any order
 */

/*
Req'ts:
- compile as executable named "myOwnWebServer"

- All incoming and outgoing functions supported by the server 
must comply with the HTTP/1.1 Protocol Specification.
https://www.w3.org/Protocols/rfc2616/rfc2616.html

- The server is to be single threaded, so only 
1 browser session needs to be supported at a time.

- The server only needs to support the GET method in any incoming requests.

- The server only needs to support the returned content types:
        plain text (specifically the .txt extension)
        HTML files (and their various extensions)
        JPG images (and their various extensions) 
        GIF

- The server must be able to parse the incoming HTTP request, and act accordingly. 
  For example - If the request is for a text-based resource (e.g. default.html) 
  and this file exists, then the server must be able to open the file, 
  read the file contents and send its contents as a properly formatted HTTP response.

- If an exception or error condition that arises in the server – you need to ensure that the 
  proper HTTP status code is returned to the client as part of the response.

- Besides having the mandatory first line in the response header, your server should also populate 
  and return the following items in response header lines: 
        the content-type 
        the content-length
        the server
        the date elements

- You are not allowed to use any of the System.Net.Http* set of classes 
(where * is a wildcard – so basically any helper classes beginning with “Http”)

- The incoming request for all resources will be guaranteed of being in the server’s root folder 
(or a sub-folder found within it). [even request for non-existent resources]

- Following best practices when creating any type of server-based application – ensure:
    • That the server outputs absolutely nothing to the console window it is running in
    • That the server generates a log file that tracks at least:
        i. The incoming HTTP Request header as well as the associated outgoing HTTP Response header (formatted so that it is easily readable)
        ii. Ensure that any other logging activity you have is also formatted to be readable
    • That each log entry has a date and timestamp (e.g. 2015-11-01 14:05:00 <log entry goes here>)

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A06_WebServer
{
    class Run
    {
        static int Main(string[] args)
        {
            string path = null;
            string[] result = null;
            int port = 0;
            string ipAddress = null;

            if (args.Length == 0)
            {
                Console.WriteLine("You need to enter things homie.");
                return 1;
            }

            //We're going to step through each of the command line arguments received.
            //We're going to check which command it contains, split it grabbing the instruction
            //and throw the instruction into the appropriate variable. This approach will allow
            //us to handle user input of the instructions in any order.
            switch (parseArg(args[0]))
            {
                case 1:
                    result = args[0].Split('=');
                    path = result[1];
                    break;
                case 2:
                    result = args[0].Split('=');
                    ipAddress = result[1];
                    break;
                case 3:
                    result = args[0].Split('=');
                    port = int.Parse(result[1]);
                    break;

            }
            switch (parseArg(args[1]))
            {
                case 1:
                    result = args[1].Split('=');
                    path = result[1];
                    break;
                case 2:
                    result = args[1].Split('=');
                    ipAddress = result[1];
                    break;
                case 3:
                    result = args[1].Split('=');
                    port = int.Parse(result[1]);
                    break;

            }
            switch (parseArg(args[2]))
            {
                case 1:
                    result = args[2].Split('=');
                    path = result[1];
                    break;
                case 2:
                    result = args[2].Split('=');
                    ipAddress = result[1];
                    break;
                case 3:
                    result = args[2].Split('=');
                    port = int.Parse(result[1]);
                    break;

            }

            Console.WriteLine($"The port is {port}");
            Console.WriteLine($"The path is {path}");
            Console.WriteLine($"The Ip Adress is {ipAddress}");

            //pass parameter to parseArg
            //    check return
            //    assign value to variable
            ////initialize server
            ///            

            //initialize logger
            Logger.HttpServerLogger serverLog = new Logger.HttpServerLogger("C:/temp/serverlog.txt");
            // serverLog.Init("C:/temp/serverlog.txt"); //make this definable?
            return 0;
        }

        //Grab each argument from the command line, remove the 
        static int parseArg(string argument)
        {
            if (argument.Contains("webroot"))
            {
                return 1;
            }
            else if (argument.Contains("webIP"))
            {
                return 2;
            }
            else if (argument.Contains("webPort"))
            {
                return 3;
            }
            return 0;
        }
    }
}
