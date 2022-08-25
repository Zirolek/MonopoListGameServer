namespace MonopolyGameServer;

public class Room
{
    public Room(string roomName, int maxPlayers)
    {
        RoomName = roomName;
        MaxPlayers = maxPlayers;
        if (maxPlayers <= 10)
        {
            Players = new int[maxPlayers];
        }
        StayAlive();
    }
    public string RoomName;
    private int MaxPlayers;
    //May be to 1 more from MaxPlayers 
    private int CurrentPlayers = 0;
    private Point[] Products = new Point[36];
    public readonly Cubes Cubes = new Cubes();
    private int[] Players;
    private bool IsGameStarted = false;

    private int GlobalWaitedTime = 0;
    private int PlayerWaitingId = 0;
    private int PlayerWaitTime = 0;
    
    
    
    public async Task StayAlive()
    {
        while (true)
        {
            bool AllPlayersInRoom = true;
            await Task.Delay(200);
            if (!IsGameStarted)
            {
                for (int I = 0; I < Players.Length; I++)
                {
                    if (Players[I] == 0)
                    {
                        AllPlayersInRoom = false;
                    }
                }
                //Console.WriteLine(WaitedTime + " " + AllPlayersInRoom);
                if (GlobalWaitedTime >= 25 && AllPlayersInRoom)
                {
                    IsGameStarted = true;
                    GlobalWaitedTime = 0;
                }

                if (GlobalWaitedTime > 300 && !(CurrentPlayers > 0))
                {
                    MainServer.Rooms[MainServer.GetRoomByName(RoomName)] = null;
                    MainServer.Rooms.Remove(null);
                }
            }
            
            if (IsGameStarted)
            {
                if (PlayerWaitTime >= 600)
                {
                    DataBaseController.Players[PlayerWaitingId].Bankrupt = true;
                }
                PlayerWaitTime++;
            }
            
            GlobalWaitedTime++;
        }
    }

    public async Task DropCubes(int idInList)
    {
        int Id = FindIdOnLocalList(idInList);
        if (Id != -1 && PlayerWaitingId == Id)
        {
            Cubes.Next();
        }
    }

    public bool JoinToRoom(int IdInList)
    {
        if (CurrentPlayers < MaxPlayers + 1)
        {
            try
            {
                if (!DataBaseController.Players[IdInList].InGame)
                {
                    DataBaseController.Players[IdInList].Bankrupt = false;
                    DataBaseController.Players[IdInList].InGame = true;
                    DataBaseController.Players[IdInList].Balance = 0;
                    DataBaseController.Players[IdInList].CellPosition = 0;
                    for (int I = 0; I < Players.Length; I++)
                    {
                        if (Players[I] == 0)
                        {
                            Players[I] = IdInList;
                            CurrentPlayers++;
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
                return false;
            }
        }

        return false;
    }
    
    public bool LeaveFromRoom(int IdInList)
    {
        try
        {
            if (DataBaseController.Players[IdInList].InGame)
            {
                DataBaseController.Players[IdInList].Bankrupt = false;
                DataBaseController.Players[IdInList].InGame = false;
                DataBaseController.Players[IdInList].Balance = 0;
                DataBaseController.Players[IdInList].CellPosition = 0;
                for (int I = 0; I < Players.Length; I++)
                {
                    if (Players[I] == IdInList)
                    {
                        Players[I] = 0;
                        CurrentPlayers--;
                        return true;
                    }
                }
                
                return false;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
            return false;
        }
    }

    private int FindIdOnLocalList(int IdInList)
    {
        for (int I = 0; I < Players.Length; I++)
        {
            if (Players[I] == IdInList)
            {
                return I;
            }
        }

        return -1;
    }

    public string ToJsonForLobby()
    {
        string IsGameStarted = this.IsGameStarted.ToString();
        string MaxPlayers = Players.Length.ToString();
        string AllPlayers = "{" + "\"RoomName\": \"" + RoomName + "\", \"IsGameStarted\": " + IsGameStarted + ", \"WaitedTime\": " + GlobalWaitedTime + ", \"CurrentPlayers\": " + CurrentPlayers + ", \"MaxPlayers\": " + MaxPlayers;
        for (int I = 0; I < this.Players.Length; I++)
        {
            AllPlayers = AllPlayers + $", \"Player_{I}\": \"{DataBaseController.Players[Players[I]].NickName}\"";
        }
        AllPlayers = AllPlayers + "}";
        return  AllPlayers;
    }
    
    public string ToJsonForState()
    {
        return null;
    }
}