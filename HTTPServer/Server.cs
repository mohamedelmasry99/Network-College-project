using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        
        Socket serverSocket;
        bool xp;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
            EndPoint Iep = new IPEndPoint(IPAddress.Any,portNumber);
            serverSocket.Bind(Iep);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clients = serverSocket.Accept();
                //TODO: accept connections and start thread for each accepted connection.
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clients);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clients = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clients.ReceiveTimeout = 0;
            byte[] data;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    data = new byte[1024];
                    // TODO: Receive request
                 int recivedlength=   clients.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (recivedlength == 0)
                    {
                        return ;
                    }
                    String recivedrequest = Encoding.ASCII.GetString(data,0,recivedlength);
                    // TODO: Create a Request object using received request string
                    Request req = new Request(recivedrequest);
                   xp=  req.ParseRequest();
                    // TODO: Call HandleRequest Method that returns the response
                    
                    Response newResponse = HandleRequest(req);
                    // TODO: Send Response back to client
                    clients.Send(Encoding.ASCII.GetBytes(newResponse.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clients.Close();
        }
        //--------------------------------------------------------------------------------------------------------------

        Response HandleRequest(Request request)
        {
            

            
            string  content;
            try
            {

                
                if (xp == false)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content , "");
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physical = Configuration.RootPath +"\\"+ request.relativeURI;
                //TODO: check for redirect
                string newrelative = GetRedirectionPagePathIFExist(request.relativeURI);


                if (newrelative!=string.Empty) {
                    content = LoadDefaultPage(newrelative);

                    return new Response(StatusCode.Redirect, "text/html", content, newrelative);

                }
                bool fexist = true;
                //TODO: check file exists
                
                if (!File.Exists(physical)==true)
                {
                    fexist = false;
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", content, "");

                }
                //TODO: read the physical file
                content = LoadDefaultPage(request.relativeURI);

                // Create OK response
                
                return new Response(StatusCode.OK, "text/html", content, "");
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content,"");

            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
           
                if (Configuration.RedirectionRules.ElementAt(0).Key.Equals(relativePath))
                {

                    return Configuration.RedirectionRules.ElementAt(0).Value;
                }
            

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            StreamReader st = new StreamReader(filePath);
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception());
                return string.Empty;
            }
            else
            {
                string content = st.ReadToEnd();
                return content;
            }
            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // FileStream fs = new FileStream("redirectionRules.txt", FileMode.Open);
                string x = filePath + "\\redirectionRules.txt";

                StreamReader sr = new StreamReader(x);
                string rule="";
                while (sr.Peek() != -1)
                {
                   rule= sr.ReadLine();
                    string[] dir = rule.Split(',');
                    Configuration.RedirectionRules.Add(dir[0], dir[1]);
                }
                sr.Close();
                

              
                // then fill Configuration.RedirectionRules dictionary 
               
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
