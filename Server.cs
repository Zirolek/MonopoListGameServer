using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonopoListGameServer;

public class Server
{
    private static string _Url = "http://0.0.0.0/";

    static bool _RunServer = false;
    
    static HttpListener _Listener = new HttpListener();

    public static HttpListenerContext _Ctx { get; private set; }
    public static HttpListenerRequest _Request { get; private set; }
    public static HttpListenerResponse _Response { get; private set; }

    public static void Start(string ip)
    {
        if (_RunServer == false)
        {
            StartServer(ip);
        }
        else
        {
            throw new Exception("Server already started! Listener?");
        }
    }

    private static async Task StartServer(string ip)
    {
        _Url = "http://" + ip + "/";

        StartListener();
        
        Console.WriteLine("Listening for connections on {0}", _Url);

        _RunServer = true;
        HandleIncomingConnections();
        CheckWebServer();
    }
    
    public static async Task HandleIncomingConnections()
    {
        try
        {
            while (_RunServer)
            {
                _Ctx = await _Listener.GetContextAsync();
                _Request = _Ctx.Request;
                _Response = _Ctx.Response;
                
                if (_Request.HttpMethod.ToLower() != "post" && _Request.HttpMethod.ToLower() != "get")
                {
                    SendResponse("Method not allowed!");
                    continue;
                }

                if (!_Request.UserAgent.Contains("python-requests") && !_Request.UserAgent.Contains("Dalvik/2.1.0 (Linux; U; Android 11; SM-M127F Build/RP1A.200720.012)"))
                {
                    SendResponse("Request Error!");
                    continue;
                }

                string? StrId = _Request.Headers.Get("ID");
                string? User = _Request.Headers.Get("user");
                string? Pass = _Request.Headers.Get("pass");
                string? StrRoomId = _Request.Headers.Get("RoomID");
                string? RoomName = _Request.Headers.Get("RoomName");
                string? StrMaxPlayers = _Request.Headers.Get("MaxPlayers");

                ulong ClientId = (ulong) ParseInt64(StrId);
                int RoomId = DataBase.GetRoomByName(StrRoomId);
                int MaxPlayers = ParseInt32(StrMaxPlayers);
                
                int ClientIdInList = DataBase.GetIdInList(ClientId);

                await ClientHandler.HandleClient(ClientIdInList, User, Pass, RoomId, RoomName, MaxPlayers);
            }
        }
        catch (Exception Exception)
        {
            Console.WriteLine("Error! " + Exception.Message.Replace("\n", " "));
        }
    }
    public static async void SendResponse(string responseString)
    {
        responseString = responseString.Replace("False", "\"false\"").Replace("True", "\"true\"");
        // Write the response info
        byte[] DataToResponse = Encoding.UTF8.GetBytes(responseString);
        _Response.ContentType = "application/json";
        //Response.ContentType = "text/html";
        _Response.ContentEncoding = Encoding.UTF8;
        _Response.ContentLength64 = DataToResponse.LongLength;

        // Write out to the response stream (asynchronously), then close it
        await _Response.OutputStream.WriteAsync(DataToResponse, 0, DataToResponse.Length);
        _Response.Close();
    }

    static async Task CheckWebServer()
    {
        await Task.Delay(5000);
        while (_RunServer)
        {
            string Method = "post";
            //Console.WriteLine(Url + "alive");
            HttpWebRequest HttpRequest = (HttpWebRequest)HttpWebRequest.Create("http://127.0.0.1:8080/" + "alive");
            HttpRequest.Timeout = 1000;
            HttpRequest.ContentType = "application/json";
            HttpRequest.UserAgent = "python-requests";
            HttpRequest.Method = Method;

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
                RestartListener();
                Console.WriteLine("Restarted!");
            }

            await Task.Delay(1000);
        }
    }

    public static void RestartListener()
    {
        StopListener();
        StartListener();
    }

    public static void StartListener()
    {
        _Listener = new HttpListener();
        _Listener.TimeoutManager.IdleConnection = TimeSpan.FromSeconds(2);
        _Listener.TimeoutManager.DrainEntityBody = TimeSpan.FromSeconds(2);

        _Listener.Prefixes.Add(_Url);
        _Listener.Prefixes.Add("http://127.0.0.1:8080/");
        _Listener.Start();
        _RunServer = true;
        CheckWebServer();
        HandleIncomingConnections();
    }

    public static void StopListener()
    {
        _RunServer = false;
        _Listener.Close();
    }

    private static int ParseInt32(string toParse)
    {
        if (toParse == null)
        {
            return -1;
        }
        
        try
        {
            return Int32.Parse(toParse);
        }
        catch (Exception Exception)
        {
            Console.WriteLine("Error! " + Exception.Message.Replace("\n", " "));
        }

        return -1;
    }
    
    private static long ParseInt64(string toParse)
    {
        if (toParse == null)
        {
            return -1;
        }

        try
        {
            return Int64.Parse(toParse);
        }
        catch (Exception Exception)
        {
            Console.WriteLine("Error! " + Exception.Message.Replace("\n", " "));
        }

        return -1;
    }
}