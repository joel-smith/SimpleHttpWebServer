/* 
 * FILE:            HttpMessage.cs
 * PROJECT:         SimpleHttpWebServer
 * AUTHORS:         Joel Smith & Ian Ewing
 * DATE:            Nov 17, 2020
 * DESCRIPTION:     This class contains the base and abstract HttpMessage class.
 *                  It also contains our Response/Request objects that maintiain the
 *                  message state inbetween methods.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace A06_WebServer
{
    /// <summary>
    /// abstract class with basic functions to return TopLine of a message
    /// </summary>
    public abstract class HttpMessage
    {
        public Dictionary<string, string> headers = new Dictionary<string, string>();
        public Byte[] bodyBytes;
    }
    


    /// <summary>
    /// Request class that contains various methods allowing us to populate a request object
    /// for ease of data access.
    /// </summary>
    public class Request : HttpMessage
    {      
        public RequestStartLine startLine;

        /// <summary>
        /// constructor to make a GET HTTP/1.1 request from target and host
        /// </summary>
        /// <param name="target"></param>
        /// <param name="host"></param>
        /// <param name="verb"></param>
        /// <param name="version"></param>
        public Request(string targetGET, string hostGET)
        {
            startLine.Verb = "GET"; //Hardcoded since our server only accepts GET
            startLine.Target = targetGET;
            startLine.Version = 1.1; //Hardocded as our server only uses HTTP/1.1
            headers.Add("HOST", hostGET);
        }
    }

    /// <summary>
    /// struct that holds individual data pieces for startline and returns good string for it
    /// </summary>
    public struct RequestStartLine
    {
        public RequestStartLine(string verb, string target, double version)
        {
            Verb = verb;
            Target = target;
            Version = version;
        }

        public string Verb { get; set; }
        public string Target { get; set; }
        public double Version { get; set; }

        public override string ToString() => $"{Verb}" + " " + $"{Target}" + " HTTP/" + $"{Version}";
    }



    /// <summary>
    /// Response, KISS keep it simple, store all headers in <string, string> dictionary
    /// </summary>
    public class Response : HttpMessage
    {
        public ResponseStartLine startLine;

        /// <summary>
        /// to be used for NewParseRequest() to fill
        /// </summary>
        /// <param name="version"></param>
        /// <param name="status"></param>
        /// <param name="contentType"></param>
        /// <param name="contentLength"></param>
        /// <param name="inputBytes"></param>
        public Response(double version, int status, string contentType, int contentLength, Byte[] inputBytes)
        {
            startLine.Version = version;
            startLine.Code = status;
            startLine.Text = "heck yeah brotha";

            headers.Add("Date", DateTime.Now.ToString());
            headers.Add("Content-Type", contentType);
            headers.Add("Content-Length", contentLength.ToString());
            bodyBytes = inputBytes.ToArray();
        }
    }



    /// <summary>
    /// struct to be used within HttpResponse 
    /// </summary>
    public struct ResponseStartLine
    {
        public ResponseStartLine(double version, int code, string text)
        {
            Version = version;
            Code = code;
            Text = text;
        }

        public double Version { get; set; } //double as they will all be HTTP
        public int Code { get; set; }
        public string Text { get; set; }

        public override string ToString() => "HTTP/" + $"{Version}, {Code}, {Text}";   
    }
}
