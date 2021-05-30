using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            headerLines.Add("content-type: " + contentType + "\r\n");
            headerLines.Add("content-length: " + content.Length + "\r\n");
            headerLines.Add("Date: " + DateTime.Now.ToString() + "\r\n");

            if (code == StatusCode.Redirect)
            {
                headerLines.Add(redirectoinPath+ "\r\n");
            }

            // TODO: Create the response string
            responseString = GetStatusLine(code) + "\r\n" + "content-type: ";
            responseString += contentType + "\r\n" + "content-length: " + content.Length;
            responseString+= "\r\n" + "Date: " + DateTime.Now.ToString() + redirectoinPath + "\r\n" + "\r\n" + content;

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = Configuration.ServerHTTPVersion+code+"ok";
            

            return statusLine;
        }
    }
}
