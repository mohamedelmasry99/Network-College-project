using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines=new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {

           
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line,
        /// header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            //TODO: parse the receivedRequest using the \r\n delimeter  
             requestLines = requestString.Split(new string[] { "\r\n" },StringSplitOptions.None);
           // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 4)
                return false;
            // Parse Request line

            if (ParseRequestLine() == false)
                return false;
            // Validate blank line exists
            if (ValidateBlankLine()==false)
            return false;

            // Load header lines into HeaderLines dictionary
            if (LoadHeaderLines() == false)
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            string[] req_line_tokens = requestLines[0].Split(' ');
            if (req_line_tokens.Length != 3)
            {
                if (req_line_tokens.Length == 2)
                {
                    httpVersion = HTTPVersion.HTTP09; ;
                }
                else return false; 
            }
            //--------------------------------Validate Method--------------------------------------------------------
            if (req_line_tokens[0] == "GET")
                method = RequestMethod.GET;
            else if (req_line_tokens[0] == "POST") { 
                method = RequestMethod.POST;
              contentLines[contentLines.Length]=requestLines[requestLines.Length];
            }
            else if (req_line_tokens[0] == "HEAD") { 
                method = RequestMethod.HEAD;
                contentLines[contentLines.Length] = requestLines[requestLines.Length-1];
            }
            else return false;
            //----------------------------------------------------------------------------------------------------
            //-------------------------------Validate http version--------------------------------------
            string[] httpver = req_line_tokens[2].Split('/');
            if (httpver[1] == "0.9")
                httpVersion = HTTPVersion.HTTP09;
            else if (httpver[1] == "1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (httpver[1] == "1.1")
                httpVersion = HTTPVersion.HTTP11;
            else return false;
            //-------------------------------------------------------------------------------------------------
            //-----------------------------Validate URI---------------------------------------------------
            if (ValidateIsURI(req_line_tokens[1]) == false)
                return false;
            //-------------------------------------------------------------------------------------------

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            if (!uri.Contains('/'))
                return false;
            string[] u = uri.Split('/');
            relativeURI = u[1];
            if (!relativeURI.Contains(".html"))
                return false;
            return true;
        }

        private bool LoadHeaderLines()
        {
            bool cond = true;
         //   bool host =false;
            if (httpVersion==HTTPVersion.HTTP10)
                cond = false;
            string[] headerliness=new string[requestLines.Length];
            requestLines.CopyTo(headerliness,0);
            for(int j =1; j < headerliness.Length-2; j++)
            {
                string []hd = headerliness[j].Split(new string[] { ": " }, StringSplitOptions.None);
                if (hd.Length != 2 && cond == false)
                    return false;
               
                //if (hd[0] == "Host")
                //    host = true;
                headerLines.Add(hd[0],hd[1]);

            }
            //if (host == false)
            //    return false;
            return true;
        }

        private bool ValidateBlankLine()
        {
            string blankline = requestLines[requestLines.Length-2];
            if (blankline.Length != 0)
                return false;
            return true;
        }

    }
}
