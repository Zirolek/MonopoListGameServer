namespace MonopoListGameServer;

public class ClientHandler
{
    public static async Task HandleClient(int clientIdInList, string user, string pass, int roomId, string roomName, int maxPlayers)
    {
        int ClientIdInList = clientIdInList;
        string User = user;
        string Pass = pass;
        int RoomId = DataBase.GetRoomByName(roomName);
        string RoomName = roomName;
        int MaxPlayers = maxPlayers;
        switch (Server._Request.Url.AbsolutePath.ToLower())
        {
            case "/exit":
                Console.WriteLine("Shutdown requested");
                Server.StopListener();
                SendResponse(" ");
                break;
            case "/register":
                int IdInList =
                    DataBase.RegisterNewPlayer(User, Pass);
                if (IdInList != -1)
                {
                    SendResponse(DataBase.Players[IdInList].ToJsonSaved());
                }
                else
                {
                    SendResponse("{\"ID\": \"" + "Error!" + "\", \"NickName\": \"" + "Error!" +
                                 "\", \"Password\": \"" + "Error!" + "\"}");
                }

                break;
            case "/login":
                //int IdInList1 = DataBase.GetIdInList(Req.Headers.Get("user"), Req.Headers.Get("pass"));
                int IdInList1 = DataBase.IsAccountExists(User);
                if (IdInList1 != -1)
                {
                    int ItIsIt = DataBase.GetIdInList(User, User);
                    if (ItIsIt != -1)
                    {
                        SendResponse(DataBase.Players[ItIsIt].ToJsonSaved());
                    }
                    else
                    {
                        SendResponse("{\"ID\": " + "-2" + ", \"NickName\": \"" + "User found!" + "\", \"Password\": \"" + "Wrong password!" + "\"}");
                    }
                }
                else
                {
                    SendResponse("{\"ID\": " + "-1" + ", \"NickName\": \"" + "User not found!" +
                                 "\", \"Password\": \"" + "Error!" + "\"}");
                }

                break;
            case "/save":
                DataBase.SerializeUserData();
                break;
            case "/room/new":
                if (ClientIdInList != -1 && DataBase.Players[ClientIdInList].Id != 0)
                {
                    SendResponse("{\"RoomCreated\": " + DataBase.CreateNewRoom(RoomName, MaxPlayers) + "}");
                }
                else
                {
                    SendResponse("{\"RoomCreated\": False}");
                }

                break;
            case "/room/state":
                if (ClientIdInList != -1 && DataBase.Players[ClientIdInList].Id != 0)
                {
                    SendResponse(DataBase.GetRoomState(RoomName));
                }
                else
                {
                    SendResponse("null");
                }

                break;
            case "/room/join":
                if (ClientIdInList != -1 && DataBase.Players[ClientIdInList].Id != 0 && RoomId != -1)
                {
                    bool Res = DataBase.Rooms[RoomId].JoinToRoom(ClientIdInList);
                    if (Res)
                    {
                        Console.WriteLine($"Player {DataBase.Players[ClientIdInList].NickName} joined to " + RoomName);
                    }

                    SendResponse("{\"Joined\": " + Res + "}");
                }
                else
                {
                    SendResponse("{\"Joined\": " + "null}");
                }

                break;
            case "/room/leave":
                if (ClientIdInList != -1 && DataBase.Players[ClientIdInList].Id != 0 && RoomId != -1)
                {
                    bool Res = DataBase.Rooms[RoomId].LeaveFromRoom(ClientIdInList);
                    if (Res)
                    {
                        Console.WriteLine($"Player {DataBase.Players[ClientIdInList].NickName} leaved from " + RoomName);
                    }

                    SendResponse(DataBase.GetJsonWithOneKey("Leaved", Res.ToString()));
                }
                else
                {
                    SendResponse("{\"Leaved\": " + "null}");
                }

                break;
            case "/rooms":
                string ToResponse = "{";
                DataBase.Rooms.Remove(null);
                for (int I = 0; I < DataBase.Rooms.Count; I++)
                {
                    string Players = "[";

                    for (int J = 0; J < DataBase.Rooms[I].PlayersP.Count; J++)
                    {
                        string Player = $"\"{DataBase.Rooms[I].PlayersP[J].NickName}\"";

                        if (J == DataBase.Rooms[I].PlayersP.Count - 1)
                        {
                            Players = Players + $"{Player}";
                        }
                        else
                        {
                            Players = Players + Player + ", ";
                        }
                    }

                    Players += "]";

                    string Room = "{" + $"\"RoomName\": \"{DataBase.Rooms[I].RoomName}\", " +
                                  $"\"CurrentPlayers\": {DataBase.Rooms[I].CurrentPlayersP}, " +
                                  $"\"MaxPlayers\": {DataBase.Rooms[I].MaxPlayersP}, " +
                                  $"\"IsGameStarted\": {DataBase.Rooms[I].IsGameStartedP}, " + $"\"Players\": {Players}" +
                                  "}";


                    if (I != DataBase.Rooms.Count - 1)
                    {
                        ToResponse = ToResponse + $"\"{I}\": {Room}" + ", ";
                    }

                    if (I == DataBase.Rooms.Count - 1)
                    {
                        ToResponse = ToResponse + $"\"{I}\": {Room}";
                    }
                }

                SendResponse(ToResponse + "}");
                break;
            case "/room/move":
                if (RoomId != -1)
                {
                    DataBase.Rooms[RoomId].DropCubes(ClientIdInList);
                    SendResponse(DataBase.Rooms[RoomId].Cubes.ToJson());
                }
                break;
            //For debug
            case "/player":
                SendResponse(DataBase.Players[ClientIdInList].ToJsonSaved());
                break;
            //End For debug
            case "/alive":
                SendResponse("True");
                break;
            default:
                SendResponse("Bad Request");
                break;
        }
    }

    static void SendResponse(string toResponse)
    {
        Server.SendResponse(toResponse);
    }
}