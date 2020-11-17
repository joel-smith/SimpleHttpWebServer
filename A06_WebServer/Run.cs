/*
 Filename: Run.cs
 Description: holds main for the single threaded web server.


Command Line Arguments:
–webRoot : which will be set to the root folder for the website data (html, jpg and gif files)
–webIP : which will be set to the IP address of the server
–webPort : which will be set to the Port number the server will be listening on

example: myOwnWebServer –webRoot=C:\localWebSite –webIP=192.168.100.23 –webPort=5300
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
        static void Main(string[] args)
        {

            //initialize server
        }
    }
}
