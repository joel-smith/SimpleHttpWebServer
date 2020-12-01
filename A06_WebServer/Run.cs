/*
* FILE:         Run.cs
* PROJECT:      A06 Webserver
* AUTHORS:      Joel Smith & Ian Ewing
* DATE:         November 17, 2020
* DESCRIPTION:  This contains the actual runtime code our our single
*               threaded web server. It is responsible for receving 
*               startup parameters and initalizing the server processes
*               that are found in HttpServer
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace A06_WebServer
{
    class Run
    {
        public static volatile bool Go = true;

        static int Main(string[] args)
        {
            //Variables that will be used in Main
            string path = null;
            string[] result = null;
            int port = 0;
            string ipAddress = null;

            //Check to make sure arguments were inputted.
            if (args.Length != 3)
            {
                //User did not supply sufficient, or supplied too many, arguments.
                Console.WriteLine("[ERROR] Non-Recoverable Error. Insufficient or too many command-line arguments provided.");
                Console.ReadKey();
                return 1; //Exit
            }

            //We're going to step through each of the command line arguments received.
            //We're going to check which command it contains, split it (grabbing the instruction)
            //and throw the instruction into the appropriate variable. This approach will allow
            //us to handle user input of the instructions in any order.
            switch (ParseArg(args[0]))
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
            switch (ParseArg(args[1]))
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
            switch (ParseArg(args[2]))
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
            //Format our user inputted string into an IPAddress
            IPAddress address = IPAddress.Parse(ipAddress);

            //server creation
            HttpServer server = new HttpServer(path, address, port);

            //Log the server startup parameters
            server.serverLog.Log($"[SERVER STARTED] {args[0]} {args[1]} {args[2]}"); 

            //Call to begin server process, passing in our ipaddress and port number
            server.Init();

            //Keep server running until user presses any key
            Console.ReadKey();
            server.Close();
            return 0;
        }



        /*
         * Method:          ParseArg
         * Description:     Takes in a string argument, checks against 1 of 3 commands
         *                  and returns an int based off which condition it fulfils
         * Parameter:       string argument - string representing a command line argument
         * Returns:         int - 1 for webroot, 2 for webIP, 3 for webPort
         * 
         */
        static int ParseArg(string argument)
        {
            if (argument.Contains("webRoot"))
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
