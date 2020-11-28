using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace A06_WebServer
{
    /// <summary>
    /// abstract class with basic functions to return TopLine of a message and 
    /// </summary>
    public abstract class HttpMessage
    {
        /// <summary>
        /// returns the topline or startline for the message, format varies
        /// </summary>
        /// <returns>The TopLine as a string</returns>
        public abstract string TopLine();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all the headers the object has, as strings</returns>
        public abstract string Headers();

        //would this be useful?
        public abstract string WholeMessage();

        public string Body;
    }
    
    public class Request : HttpMessage
    {
        public override string TopLine()
        {
            return startLine.ToString();
        }

        /// <summary>
        /// gives you the headers from that dictionary in a nice way
        /// </summary>
        /// <returns>string containing the headers</returns>
        public override string Headers()
        {
            string outputString = "";
            foreach(KeyValuePair<string, string> key in headers)
            {
                outputString = outputString + key.Key + key.Value + "\n";
            }

            return outputString;
        }

        /// <summary>
        /// returns the entire Request as a nicely formatted string
        /// </summary>
        /// <returns></returns>
        public override string WholeMessage()
        {
            string everything = "";

            everything = this.TopLine() + "\n" + this.Headers() + "\n" + Body;

            return everything;
        }

        public RequestStartLine startLine;
        public Dictionary<string, string> headers = new Dictionary<string, string>();

        /// <summary>
        /// constructor to make a GET HTTP/1.1 request from target and host
        /// </summary>
        /// <param name="target"></param>
        /// <param name="host"></param>
        public Request(string targetGET, string hostGET)
        {
            startLine.Verb = "GET";
            startLine.Target = targetGET;
            startLine.Version = 1.1;
            headers.Add("HOST", hostGET);
        }
    }

    /// <summary>
    /// NewResponse, KISS keep it simple, store all headers in <string, string> dictionary
    /// </summary>
    public class NewResponse : HttpMessage
    {

        public ResponseStartLine startLine;
        public Dictionary<string, string> headers = new Dictionary<string, string>();

        public Byte[] bodyBytes;

        /// <summary>
        /// to be used for NewParseRequest() to fill
        /// </summary>
        /// <param name="version"></param>
        /// <param name="status"></param>
        /// <param name="contentType"></param>
        /// <param name="contentLength"></param>
        public NewResponse(double version, int status, string contentType, int contentLength)
        {
            startLine.Version = version;
            startLine.Code = status;
            startLine.Text = "heck yeah brotha";

            headers.Add("Date", DateTime.Now.ToString());
            headers.Add("Content-Type", contentType);
            headers.Add("Content-Length", contentLength.ToString());
        }



        public void FillBody(Byte[] inputBytes)
        {
            //for (int i = 0; i < inputBytes.Length; i++)
            //{
            //    bodyBytes[i] = inputBytes[i];
            //}

            bodyBytes = inputBytes.ToArray();
        }




        //do we even need these overrides??
        public override string Headers()
        {
            throw new NotImplementedException();
        }

        public override string TopLine()
        {
            throw new NotImplementedException();
        }

        public override string WholeMessage()
        {
            throw new NotImplementedException();
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
