namespace MonopoListGameServer;

public class Room
{
    public Room(string roomName, int maxPlayers)
    {
        RoomName = roomName;
        MaxPlayers = maxPlayers;
        if (maxPlayers <= 10)
        {
            Players = new Player[maxPlayers];
            int I = 0;
            while (I < Players.Length)
            {
                Players[I] = null;
                I++;
            }
        }
        StayAlive();
    }
    public string RoomName;
    private int MaxPlayers;
    //May be to 1 more from MaxPlayers 
    private int CurrentPlayers = 0;
    private Point[] Products = new Point[36];
    public readonly Cubes Cubes = new Cubes();
    private Player[] Players;
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
                    if (Players[I] == null)
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
                    Players[PlayerWaitingId].Bankrupt = true;
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
                    for (int I = 0; I < Players.Length; I++)
                    {
                        if (Players[I] == null)
                        {
                            Players[I] = new Player(DataBaseController.Players[IdInList]);
                            DataBaseController.Players[IdInList].InGame = true;
                            Players[I].InGame = true;
                            Players[I].Bankrupt = false;
                            Players[I].InGame = true;
                            Players[I].Balance = 0;
                            Players[I].CellPosition = 0;
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
                for (int I = 0; I < Players.Length; I++)
                {
                    if (Players[I].Id == DataBaseController.Players[IdInList].Id)
                    {
                        Players[I] = new Player(DataBaseController.Players[IdInList]);
                        DataBaseController.Players[IdInList].InGame = true;
                        Players[I].InGame = true;
                        Players[I].Bankrupt = false;
                        Players[I].InGame = true;
                        Players[I].Balance = 0;
                        Players[I].CellPosition = 0;
                        Players[I] = null;
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
        string MaxPlayers = Players.Length.ToString();
        string AllPlayers = "{" + "\"RoomName\": \"" + RoomName + "\", \"IsGameStarted\": " + IsGameStarted + ", \"WaitedTime\": " + GlobalWaitedTime + ", \"CurrentPlayers\": " + CurrentPlayers + ", \"MaxPlayers\": " + MaxPlayers;
        for (int I = 0; I < this.Players.Length; I++)
        {
            if (Players[I] != null)
            {
                AllPlayers = AllPlayers + $", \"Player_{I}\": \"{DataBaseController.Players[DataBaseController.GetIdInList(Players[I].Id)].NickName}\"";
            }
        }
        AllPlayers = AllPlayers + "}";
        return  AllPlayers;
    }
    
    public string ToJsonForState()
    {
        return null;
    }
}