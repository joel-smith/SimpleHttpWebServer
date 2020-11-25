using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A06_WebServer
{
    public class HttpMessage
    {
        public string Body;
    }
    
    public class Request : HttpMessage
    {

    }

    
    public class Response : HttpMessage 
    {
        //status line
        //example: "HTTP/1.1 404 Not Found"
        

        //headers, required content-type, content-length, server, date
        public HttpContentType ContentType;
        public int ContentLength;
        public DateTime Date;

        //body
        


        //constructor to be used 
        //need to set body after in the server
        public Response(HttpContentType contentType, int contentLength)
        {
            ContentType = contentType;
            ContentLength = contentLength;
            Date = DateTime.Now;
        }
    }



    /// <summary>
    /// struct to be used within HttpResponse 
    /// </summary>
    public struct HttpStartLine
    {
        public HttpStartLine(double version, int code, string text)
        {
            Version = version;
            Code = code;
            Text = text;
        }

        public double Version { get; } //double as they will all be HTTP
        public int Code { get; }
        public string Text { get; }

        public override string ToString() => "HTTP/" + $"{Version}, {Code}, {Text}";
        
    }

    public struct HttpContentType
    {
        public HttpContentType(string media, string extension)
        {
            Media = media;
            Extension = extension;
        }

        public string Media { get; } //MIME type of the resource (text, image)
        public string Extension { get; } //File extension (txt, html)

        public override string ToString() => $"{Media}" + "/" + "{Extension}";
    }
}
