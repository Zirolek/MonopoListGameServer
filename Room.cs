namespace MonopoListGameServer;

public class Room
{
    public Room(string roomName, int maxPlayers)
    {
        RoomName = roomName;
        MaxPlayers = maxPlayers;
        Console.WriteLine(MaxPlayers);
        if (maxPlayers <= 10)
        {
            Players = new List<Player>(maxPlayers);
            int I = 0;
            while (I < Players.Count)
            {
                Players[I] = null;
                I++;
            }
            StayAlive();
        }
    }
    public string RoomName;
    private int MaxPlayers;
    //May be to 1 more from MaxPlayers 
    private int CurrentPlayers = 0;
    private Point[] Products = new Point[36];
    public readonly Cubes Cubes = new Cubes();
    private List<Player> Players;
    private bool IsGameStarted = false;

    private int GlobalWaitedTime = 0;
    private int PlayerWaitingId = 0;
    private int PlayerWaitedTime = 0;
    
    
    
    public async Task StayAlive()
    {
        while (true)
        {
            bool AllPlayersInRoom = true;
            await Task.Delay(200);
            if (!IsGameStarted)
            {
                if (Players.Count < MaxPlayers)
                {
                    AllPlayersInRoom = false;
                }

                if (GlobalWaitedTime >= 25 && AllPlayersInRoom)
                {
                    IsGameStarted = true;
                    GlobalWaitedTime = 0;
                }

                if (GlobalWaitedTime > 300 && CurrentPlayers == 0)
                {
                    CloseRoom();
                }
            }
            
            if (IsGameStarted)
            {
                if (PlayerWaitedTime >= 600)
                {
                    Players[PlayerWaitingId].Bankrupt = true;
                    PlayerWaitedTime = 0;
                    StartWaitingNextPlayer();
                }

                if (Players.Count == 1)
                {
                    CloseRoom();
                }

                PlayerWaitedTime++;
            }
            
            GlobalWaitedTime++;
        }
    }

    private void CloseRoom()
    {
        MainServer.Rooms[MainServer.GetRoomByName(RoomName)] = null;
        MainServer.Rooms.Remove(null);
    }

    public async Task DropCubes(int idInList)
    {
        int Id = FindIdOnLocalList(idInList);
        if (Id != -1 && PlayerWaitingId == Id)
        {
            Cubes.Next();
            PlayerNextCellPosition(Id, Cubes.FirstCubeResultP, Cubes.SecondCubeResultP);
            if (Cubes.FirstCubeResultP != Cubes.SecondCubeResultP)
            {
                StartWaitingNextPlayer();
            }
        }
    }

    private void StartWaitingNextPlayer()
    {
        PlayerWaitedTime = 0;
        PlayerWaitingId++;
        if (PlayerWaitingId == Players.Count)
        {
            for (int I = 0; I < Players.Count; I++)
            {
                if (!Players[I].Bankrupt)
                {
                    PlayerWaitingId = I;
                    break;
                }
            }
        }
    }
    
    private void PlayerNextCellPosition(int Id, int x, int y)
    {
        PlayerWaitedTime = 0;
        Players[Id].CellPosition = Players[Id].CellPosition + (x + y);
        if (Players[Id].CellPosition >= Products.Length)
        {
            Players[Id].CellPosition = -(Products.Length - Players[Id].CellPosition);
        }
    }

    public bool JoinToRoom(int IdInList)
    {
        try
        {
            if (CurrentPlayers <= MaxPlayers && !DataBaseController.Players[IdInList].InGame)
            {
                DataBaseController.Players[IdInList].InGame = true; 
                Players.Add(new Player(DataBaseController.Players[IdInList]));
                CurrentPlayers++;
                return true; 
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

        return false;
    }
    
    public bool LeaveFromRoom(int IdInList)
    {
        try
        {
            if (DataBaseController.Players[IdInList].InGame)
            {
                for (int I = 0; I < Players.Count; I++)
                {
                    if (Players[I].Id == DataBaseController.Players[IdInList].Id)
                    {
                        DataBaseController.Players[IdInList].InGame = false;
                        Players[I] = null;
                        Players.Remove(null);
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
        for (int I = 0; I < Players.Count; I++)
        {
            if (Players[I].Id == DataBaseController.Players[IdInList].Id)
            {
                return I;
            }
        }

        return -1;
    }

    public string ToJsonForLobby()
    {
        string IsGameStarted = this.IsGameStarted.ToString();
        string MaxPlayers = this.MaxPlayers.ToString();
        string ToReturn = "{" + "\"RoomName\": \"" + RoomName + "\", \"IsGameStarted\": " + IsGameStarted + ", \"CurrentPlayers\": " + CurrentPlayers + ", \"MaxPlayers\": " + MaxPlayers;
        for (int I = 0; I < this.Players.Count; I++)
        {
            if (Players[I] != null)
            {
                ToReturn = ToReturn + $", \"Player_{I}\": \"{DataBaseController.Players[DataBaseController.GetIdInList(Players[I].Id)].NickName}\"";
            }
        }
        ToReturn = ToReturn + "}";
        return  ToReturn;
    }
    
    public string ToJsonForState()
    {
        string ToReturn = "{" + "\"PlayerWaitingId\": " + PlayerWaitingId + ", \"PlayerWaitTime\": " + PlayerWaitedTime  + ", \"IsGameStarted\": " + IsGameStarted;
        for (int I = 0; I < this.Players.Count; I++)
        {
            if (Players[I] != null)
            {
                ToReturn = ToReturn + $", \"PlayerIsBankrupt_{I}\": {Players[I].Bankrupt}";
            }
        }
        for (int I = 0; I < this.Players.Count; I++)
        {
            if (Players[I] != null)
            {
                ToReturn = ToReturn + $", \"PlayerCellPosition_{I}\": {Players[I].CellPosition}";
            }
        }
        ToReturn = ToReturn + "}";
        return ToReturn;
    }
}