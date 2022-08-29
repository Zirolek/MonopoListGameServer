// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)

using System.Text;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace MonopoListGameServer
{
    class MainServer
    {
        public static async Task Main(string[] args)
        {
            /*Room Room = new Room("Name", 2);

            DataBaseController.Path = args[1];
            DataBaseController.DeSerializeUserData();
            
            Console.WriteLine("Accounts Length: " + DataBaseController.Players.Length);
            
            Room.JoinToRoom(1);
            Room.JoinToRoom(2);

            Task Task = Room.DropCubes(1);
            Task.GetAwaiter().GetResult();
            
            Console.WriteLine("Cubes: " + Room.Cubes.ToJson());

            Console.WriteLine("State: " + Room.ToJsonForState());
            Console.WriteLine("Lobby: " + Room.ToJsonForLobby());

            await Task.Delay(5500);
            
            Task = Room.DropCubes(2);
            Task.GetAwaiter().GetResult();
            
            Console.WriteLine("State: " + Room.ToJsonForState());
            Console.WriteLine("Lobby: " + Room.ToJsonForLobby());
            
            await Task.Delay(130000);
            
            Task = Room.DropCubes(1);
            Task.GetAwaiter().GetResult();
            
            Console.WriteLine("State: " + Room.ToJsonForState());
            Console.WriteLine("Lobby: " + Room.ToJsonForLobby());
            
            await Task.Delay(130000);
            
            Console.WriteLine("State: " + Room.ToJsonForState());
            Console.WriteLine("Lobby: " + Room.ToJsonForLobby());*/
            
            Url = args[0];
            DataBaseController.Path = args[1];
            DataBaseController.DeSerializeUserData();
    
            Console.WriteLine("Accounts Length: " + DataBaseController.Players.Length);
            
            //Listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(5);
            //Listener.TimeoutManager.HeaderWait = TimeSpan.FromSeconds(5);
            Listener.TimeoutManager.IdleConnection = TimeSpan.FromSeconds(5);
            //Listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(5);
            Listener.TimeoutManager.DrainEntityBody = TimeSpan.FromSeconds(5);
    
            // Create a Http server and start listening for incoming connections
            Listener.Prefixes.Add(Url);
            Listener.Prefixes.Add("http://127.0.0.1:8080/");
            Listener.Start();
            Console.WriteLine("Listening for connections on {0}", Url);
            
            // Handle requests
            //Task ListenTask = 
            HandleIncomingConnections();
            CheckWebServer();
            //ListenTask.GetAwaiter().GetResult();
    
            while (true)
            {
                GetCommand(Console.ReadLine());
            }
        }

    public static List<Room> Rooms = new List<Room>();
    static HttpListener Listener = new HttpListener();

    static bool RunServer = true;

    //static string Url = "http://127.0.0.1:8080/";
    static string Url;
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
        try
        {
            while (RunServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext Ctx = await Listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest Req = Ctx.Request;
                HttpListenerResponse Response = Ctx.Response;

                if (Req.HttpMethod.ToLower() != "post" && Req.HttpMethod.ToLower() != "get")
                {
                    SendResponse("Request Error!");
                    continue;
                }

                if (!Req.UserAgent.Contains("python-requests") &&
                    !Req.UserAgent.Contains("Dalvik/2.1.0 (Linux; U; Android 11; SM-M127F Build/RP1A.200720.012)"))
                {
                    SendResponse("Request Error!");
                    continue;
                }

                ulong ClientId = (ulong)Convert.ToInt64(Req.Headers.Get("ID"));

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

                //Console.Write("hfdi;jfdhlk;" + ClientIdInList);

                switch (Req.Url.AbsolutePath.ToLower())
                {
                    case "/exit":
                        Console.WriteLine("Shutdown requested");
                        RunServer = false;
                        SendResponse(PageData.Replace("{0}", PageViews.ToString()));
                        break;
                    case "/register":
                        int IdInList =
                            DataBaseController.RegisterNewPlayer(Req.Headers.Get("user"), Req.Headers.Get("pass"));
                        if (IdInList != -1)
                        {
                            SendResponse(DataBaseController.Players[IdInList].ToJsonRegistered());
                        }
                        else
                        {
                            SendResponse("{\"ID\": \"" + "Error!" + "\", \"NickName\": \"" + "Error!" +
                                         "\", \"Password\": \"" + "Error!" + "\"}");
                        }

                        break;
                    case "/login":
                        //int IdInList1 = DataBaseController.GetIdInList(Req.Headers.Get("user"), Req.Headers.Get("pass"));
                        int IdInList1 = DataBaseController.IsAccountExists(Req.Headers.Get("user"));
                        if (IdInList1 != -1)
                        {
                            int ItIsIt = DataBaseController.GetIdInList(Req.Headers.Get("user"),
                                Req.Headers.Get("pass"));
                            if (ItIsIt != -1)
                            {
                                SendResponse(DataBaseController.Players[ItIsIt].ToJsonRegistered());
                            }
                            else
                            {
                                SendResponse("{\"ID\": " + "-2" + ", \"NickName\": \"" + "User found!" +
                                             "\", \"Password\": \"" + "Wrong password!" + "\"}");
                            }
                        }
                        else
                        {
                            SendResponse("{\"ID\": " + "-1" + ", \"NickName\": \"" + "User not found!" +
                                         "\", \"Password\": \"" + "Error!" + "\"}");
                        }

                        break;
                    case "/save":
                        DataBaseController.SerializeUserData();
                        break;
                    case "/room/new":
                        if (ClientIdInList != -1 && DataBaseController.Players[ClientIdInList].Id != 0 && Rooms.Count <= 20 && GetRoomByName(Req.Headers.Get("RoomName")) == -1)
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
                            SendResponse(Rooms[RoomId].ToJsonForState());
                        }
                        else
                        {
                            SendResponse("null");
                        }
                        break;
                    case "/room/join":
                        if (ClientIdInList != -1 && DataBaseController.Players[ClientIdInList].Id != 0)
                        {
                            bool Res = Rooms[RoomId].JoinToRoom(ClientIdInList);
                            if (Res)
                            {
                                Console.WriteLine(
                                    $"Player {DataBaseController.Players[ClientIdInList].NickName} joined to " +
                                    Req.Headers.Get("RoomName"));
                            }

                            SendResponse("{\"Joined\": " + Res + "}");
                        }
                        else
                        {
                            SendResponse("{\"Joined\": " + "None}");
                        }
                        
                        break;
                    case "/room/leave":
                        if (ClientIdInList != -1 && DataBaseController.Players[ClientIdInList].Id != 0 && RoomId != -1)
                        {
                            bool Res = Rooms[RoomId].LeaveFromRoom(ClientIdInList);
                            if (Res)
                            {
                                Console.WriteLine(
                                    $"Player {DataBaseController.Players[ClientIdInList].NickName} leaved from " +
                                    Req.Headers.Get("RoomName"));
                            }

                            SendResponse("{\"Leaved\": " + Res + "}");
                        }
                        else
                        {
                            SendResponse("{\"Leaved\": " + "None}");
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
                    case "/room/move":
                        if (RoomId != -1)
                        {
                            Rooms[RoomId].DropCubes(ClientIdInList);
                            SendResponse(Rooms[RoomId].Cubes.ToJson());
                        }
                        break;
                    //For debug
                    case "/player":
                        SendResponse(DataBaseController.Players[ClientIdInList].ToJsonRegistered());
                        break;
                    //End For debug
                    case "/alive":
                        SendResponse("True");
                        break;
                    default:
                        SendResponse("Bad Request");
                        break;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (Req.Url.AbsolutePath != "/favicon.ico")
                {
                    PageViews += 1;
                }

                async void SendResponse(string responseString)
                {
                    responseString = responseString.Replace("False", "\"false\"").Replace("True", "\"true\"");
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
        catch (Exception Exception)
        {
            Console.WriteLine("Error! " + Exception.Message.Replace("\n", " "));
        }
    }

    static async Task CheckWebServer()
    {
        await Task.Delay(5000);
        while (true)
        {
            string Method = "post";
            //Console.WriteLine(Url + "alive");
            HttpWebRequest HttpRequest = (HttpWebRequest)HttpWebRequest.Create("http://127.0.0.1:8080/" + "alive");
            HttpRequest.Timeout = 1000;
            HttpRequest.ContentType = "application/json";
            HttpRequest.UserAgent = "python-requests";
            HttpRequest.Method = Method;

            /*using (StreamWriter streamwriter = new StreamWriter(HttpRequest.GetRequestStream()))
            {
                streamwriter.Write(QueryJson);
                streamwriter.Flush();
                streamwriter.Close();
            }*/

            try
            {
                using (StreamReader StreamReader = new StreamReader(HttpRequest.GetResponse().GetResponseStream()))
                {
                    StreamReader.ReadToEnd();
                }
            }
            catch
            {
                Console.WriteLine("Server not responding in 1000ms \nRestarting...");
                Restart();
                Console.WriteLine("Restarted!");
            }

            static void Restart()
            {
                RunServer = false;
                Listener.Close();
                Listener = new HttpListener();
                Listener.Prefixes.Add(Url);
                Listener.Prefixes.Add("http://127.0.0.1:8080/");
                Listener.Start();
                RunServer = true;
                HandleIncomingConnections();
            }

            await Task.Delay(100);
        }
    }

    static void GetCommand(string command)
    {
        switch (command)
        {
            case "/help":
                Console.WriteLine("/stop : disable bot");
                Console.WriteLine("/save : save user data");
                Console.WriteLine("/help : get commands list");
                break;

            case "/save":
                Console.WriteLine("Starting save");
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
                Console.WriteLine("Save? (yes / no / cancel)");
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
                        return;

                    default:
                        Console.WriteLine("yes / no / cancel !");
                        GetCommand("/stop");
                        break;
                }

                break;

            default:
                Console.WriteLine("Unknow command. Use /help to get commands.");
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
