using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
           
            // 1) Make server object on port 1000
            Server x = new Server(1000,Configuration.RootPath);
           
            // 2) Start Server
            x.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt

            string x = Configuration.RootPath + "\\redirectionRules.txt";
           // FileStream fs = new FileStream(Configuration.RootPath+"redirectionRules.txt", FileMode.Open);
            StreamWriter sw = new StreamWriter(x);
            // each line in the file specify a redirection rule
            
            sw.WriteLine("aboutus.html,aboutus2.html");
            sw.Close();
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}
