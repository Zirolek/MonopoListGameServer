// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)
using System.Net;
using MonopoListGameServer_Test;

namespace MonopoListGameServer
{
    class ServerStarter
    {
        static private Commands Cmds;
        public static void Main(string[] args)
        {
            bool IsTest = false;
            
            if (IsTest)
            {
                DataBase.DeSerializeUserData();
                UnitTests.Test();
                DataBase.SerializeUserData();
                while (true)
                {
                    Console.ReadLine();
                }
            }

            string Ip = args[0];
            string Port = args[1];
            string PathToFiles = args[2];
            
            if (args.Length < 2)
            {
                Console.WriteLine("Error! Invalid Arguments. Launch line example: ./File {BindInterface:port} {path to config (with out \"users.json\")}/");
                Environment.Exit(-1);
            }
            
            int InputChecksResult = CheckInputArgs(Ip, Port, PathToFiles);

            DataBase.Path = PathToFiles;
            DataBase.DeSerializeUserData();
            Console.WriteLine("Accounts Length: " + DataBase.Players.Length);
            
            switch (InputChecksResult)
            {
                case 0:
                    Console.WriteLine("Starting server...");
                    Server.Start(Ip + ":" + Port);
                    Cmds = new Commands();
                    break;
                case -1:
                    Console.WriteLine("Invalid IP! ex. 0.0.0.0");
                    Environment.Exit(0);
                    break;
                case -2:
                    Console.WriteLine("Invalid port! ex. 8080");
                    Environment.Exit(0);
                    break;
                case -3:
                    Console.WriteLine("Invalid file/path! If the file does not exist create it.");
                    Environment.Exit(0);
                    break;
            }
        }

        private static int CheckInputArgs(string interfaceAddress, string port, string pathToConfig)
        {
            IPAddress Ip = IPAddress.Any;
            if (IPAddress.TryParse(interfaceAddress, out Ip) == false)
            {
                return -1;
            }

            int Port = 0;
            bool PortIsNumber = Int32.TryParse(port, out Port);
            if (PortIsNumber && (Port > 1 && Port < 65535) == false)
            {
                return -2;
            }

            Console.WriteLine(pathToConfig + "users.json");
            
            if (File.Exists(pathToConfig + "users.json") == false)
            {
                return -3;
            }
            
            return 0;
        }
    }
}
