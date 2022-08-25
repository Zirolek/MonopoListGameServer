// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)

using System.Text;
using System.Net;
using Microsoft.VisualBasic;

namespace MonopolyGameServer
{
    class MainServer
    {
        public static void Main(string[] args)
        {
            Url = args[0];
            DataBaseController.Path = args[1];
            DataBaseController.DeSerializeUserData();

            //Listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(5);
            //Listener.TimeoutManager.HeaderWait = TimeSpan.FromSeconds(5);
            Listener.TimeoutManager.IdleConnection = TimeSpan.FromSeconds(5);
            //Listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(5);
            Listener.TimeoutManager.DrainEntityBody = TimeSpan.FromSeconds(5);

            // Create a Http server and start listening for incoming connections
            Listener.Prefixes.Add(Url);
            Listener.Start();
            Console.WriteLine("Listening for connections on {0}", Url);
            
            // Handle requests
            //Task ListenTask = 
            HandleIncomingConnections();
            //ListenTask.GetAwaiter().GetResult();

            while (true)
            {
                GetCommand(Console.ReadLine());
            }
        }
        
        public static List<Room> Rooms = new List<Room>();
        static HttpListener Listener = new HttpListener();
        static bool RunServer = true;
        static string Url = "http://127.0.0.1:8080/";
        static int PageViews;
        static string PageData = 
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";
        
        //public static string pageData = "{\"viewCount\": \"{0}\", \"status\": \"{1}\"}";
        public static async Task HandleIncomingConnections()
        {
            Console.WriteLine("Server thread started");
            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (RunServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext Ctx = await Listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest Req = Ctx.Request;
                HttpListenerResponse Response = Ctx.Response;

                if (Req.Url == null) continue;

                if (Req.HttpMethod.ToLower() != "post" && Req.HttpMethod.ToLower() != "get") 
                {
                    SendResponse("Request Error!");
                    continue;
                }
                if (!Req.UserAgent.Contains("python-requests")) 
                {
                    SendResponse("Request Error!");
                    continue;
                }
                
                ulong ClientId = (ulong) Convert.ToInt64(Req.Headers.Get("ID"));
                int RoomId = 0;
                
                if (Req.Headers.Get("RoomID") != null)
                {
                    RoomId = Convert.ToInt32(Req.Headers.Get("RoomID"));
                }

                if (Req.Headers.Get("RoomName") != null)
                {
                    RoomId = GetRoomByName(Req.Headers.Get("RoomName"));
                }

                int ClientIdInList = DataBaseController.GetIdInList(ClientId);
                
                switch (Req.Url.AbsolutePath.ToLower())
                {
                    /*case "/shutdown":
                        Console.WriteLine("Shutdown requested");
                        RunServer = false;
                        SendResponse(PageData.Replace("{0}", PageViews.ToString()));
                        break;*/
                    case "/register":
                        int IdInList = DataBaseController.RegisterNewPlayer(Req.Headers.Get("user"), Req.Headers.Get("pass"));
                        if (IdInList != -1)
                        {
                            SendResponse(DataBaseController.Players[IdInList].ToJson());
                        }
                        else
                        {
                            SendResponse("{\"ID\": \"" + "Error!" + "\", \"NickName\": \"" + "Error!" + "\", \"Password\": \"" + "Error!" + "\"}");
                        }
                        break;
                    case "/login":
                        //int IdInList1 = DataBaseController.GetIdInList(Req.Headers.Get("user"), Req.Headers.Get("pass"));
                        int IdInList1 = DataBaseController.IsAccountExists(Req.Headers.Get("user"));
                        if (IdInList1 != -1)
                        {
                            int ItIsIt = DataBaseController.GetIdInList(Req.Headers.Get("user"), Req.Headers.Get("pass"));
                            if (ItIsIt != -1)
                            {
                                SendResponse(DataBaseController.Players[ItIsIt].ToJson());
                            }
                            else
                            {
                                SendResponse("{\"ID\": " + "-2" + ", \"NickName\": \"" + "User found!" + "\", \"Password\": \"" + "Wrong password!" + "\"}");
                            }
                        }
                        else
                        {
                            SendResponse("{\"ID\": " + "-1" + ", \"NickName\": \"" + "User not found!" + "\", \"Password\": \"" + "Error!" + "\"}");
                        }
                        break;
                    case "/save":
                        DataBaseController.SerializeUserData();
                        break;
                    case "/room/new":
                        if (ClientIdInList != -1 && DataBaseController.Players[ClientIdInList].Id != 0 && Rooms.Count <= 20)
                        {
                            int PlayersCount = Convert.ToInt32(Req.Headers.Get("MaxPlayers"));
                            Rooms.Add(new Room(Req.Headers.Get("RoomName"), PlayersCount));
                            SendResponse("{\"RoomCreated\": True}");
                        }
                        else
                        {
                            SendResponse("{\"RoomCreated\": False}");
                        }
                        break;
                    case "/room/state":
                        if (Rooms[RoomId] != null)
                        {
                            SendResponse(Rooms[RoomId].ToJsonForLobby());
                        }
                        else
                        {
                            SendResponse("null");
                        }
                        break;
                    case "/rooms":
                        string ToResponse = "{";
                        Rooms.Remove(null);
                        for (int I = 0; I < Rooms.Count; I++)
                        {
                            if (I != Rooms.Count - 1)
                            {
                                ToResponse = ToResponse + $"\"{I}\": " + Rooms[I].ToJsonForLobby() + ", ";
                            }
                            if (I == Rooms.Count - 1)
                            {
                                ToResponse = ToResponse + $"\"{I}\": " + Rooms[I].ToJsonForLobby();
                            }
                        }
                        SendResponse(ToResponse + "}");
                        break;
                    case "/join":
                        if (ClientIdInList != -1 && DataBaseController.Players[ClientIdInList].Id != 0)
                        {
                            bool Res = Rooms[RoomId].JoinToRoom(ClientIdInList);
                            if (Res)
                            {
                                Console.WriteLine($"Player {DataBaseController.Players[ClientIdInList].NickName} joined to " + Req.Headers.Get("RoomName"));
                            }
                            SendResponse("{\"Joined\": " + Res + "}");
                        }
                        else
                        {
                            SendResponse("{\"Joined\": " + "None}");
                        }
                        break;
                    case "/leave":
                        if (ClientIdInList != -1 && DataBaseController.Players[ClientIdInList].Id != 0)
                        {
                            bool Res = Rooms[RoomId].LeaveFromRoom(ClientIdInList);
                            if (Res)
                            {
                                Console.WriteLine($"Player {DataBaseController.Players[ClientIdInList].NickName} leaved from " + Req.Headers.Get("RoomName"));
                            }
                            SendResponse("{\"Leaved\": " + Res + "}");
                        }
                        else
                        {
                            SendResponse("{\"Leaved\": " + "None}");
                        }
                        break;
                    case "/game/state":
                        /*ulong ClientId1 = (ulong) Convert.ToInt64(Req.Headers.Get("ID"));
                        int RoomId1 = Convert.ToInt32(Req.Headers.Get("RoomID"));
                        int ClientIdInList1 = DataBaseController.GetIdInList(ClientId1);
                        if (ClientIdInList1 != -1 && DataBaseController.Players[ClientIdInList1].Id != 0)
                        {
                            SendResponse("\"Joined\": " + Rooms[RoomId1].JoinToRoom(ClientIdInList1));
                        }
                        else
                        {
                            SendResponse("\"Joined\": " + "None");
                        }*/
                        SendResponse("{\"Response\": \"For bidden\"}");
                        break;
                    case "/game/move":
                        Rooms[RoomId].DropCubes(ClientIdInList);
                        SendResponse(Rooms[RoomId].Cubes.ToJson());
                        break;
                    //For debug
                    case "/player":
                        SendResponse(DataBaseController.Players[ClientIdInList].ToJson());
                        break;
                    //End For debug
                    default:
                        SendResponse("Bad request");
                        break;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (Req.Url.AbsolutePath != "/favicon.ico")
                {
                    PageViews += 1;
                }
                async void SendResponse(string responseString)
                {
                    // Write the response info
                    byte[] DataToResponse = Encoding.UTF8.GetBytes(responseString);
                    Response.ContentType = "application/json";
                    //Response.ContentType = "text/html";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.ContentLength64 = DataToResponse.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await Response.OutputStream.WriteAsync(DataToResponse, 0, DataToResponse.Length);
                    Response.Close();
                }
            }
        }
        
        static void GetCommand(string command)
        {
            switch (command) 
            {
                case "/help":
                    Console.WriteLine("\n" + "/stop : disable bot");
                    Console.WriteLine("/save : save user data");
                    Console.WriteLine("/help : get commands list" + "\n");
                    break;

                case "/save":
                    Console.WriteLine("\n" + "Starting save");
                    DataBaseController.SerializeUserData();
                    Console.WriteLine();
                    break;
                
                case "/room":
                    string ToResponse = "{";
                    Rooms.Remove(null);
                    for (int I = 0; I < Rooms.Count; I++)
                    {
                        if (I != Rooms.Count - 1 && Rooms[I] != null)
                        {
                            ToResponse = ToResponse + $"\"{I}\": " + Rooms[I].ToJsonForLobby() + ", ";
                        }
                        if (I == Rooms.Count - 1 && Rooms[I] != null)
                        {
                            ToResponse = ToResponse + $"\"{I}\": " + Rooms[I].ToJsonForLobby();
                        }
                        if (I != Rooms.Count - 1 && Rooms[I] == null)
                        {
                            ToResponse = ToResponse + $"\"{I}\": " + " \"null\", ";
                        }
                        if (I == Rooms.Count - 1 && Rooms[I] == null)
                        {
                            ToResponse = ToResponse + $"\"{I}\": " + " \"null\"";
                        }
                    }
                    Console.WriteLine(ToResponse + "}");
                    break;

                case "/stop":
                    Console.WriteLine("\nSave? (yes / no / cancel)\n");
                    string Input = Console.ReadLine();
                    switch (Input) 
                    {
                        case "yes":
                            DataBaseController.SerializeUserData();
                            RunServer = false;
                            Listener.Close();
                            Environment.Exit(0);
                            break;

                        case "no":
                            RunServer = false;
                            Listener.Close();
                            Environment.Exit(0);
                            break;

                        case "cancel":
                            Console.WriteLine("\n");
                            break;
                        
                        default:
                            Console.WriteLine("yes / no / cancel !" + "\n");
                            GetCommand("/stop");
                            break;
                    }
                    break;
                
                default:
                    Console.WriteLine("\n" + "Unknow command. Use /help to get commands." + "\n");
                    break;

            }
        }

        public static int GetRoomByName(string name)
        {
            for (int I = 0; I < Rooms.Count; I++)
            {
                if (Rooms[I].RoomName == name)
                {
                    return I;
                }
            }

            return -1;
        }
    }
}
