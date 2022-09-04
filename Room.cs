using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MonopoListGameServer;

public class Room
{
    public Room(string roomName, int maxPlayers)
    {
        if (maxPlayers <= 10)
        {
            RoomName = roomName;
            MaxPlayers = maxPlayers;
            Players = new List<Player>(maxPlayers);
            int I = 0;
            while (I < Players.Count)
            {
                Players[I] = null;
                I++;
            }
            SetPoints();
            StayAlive();
        }
        else
        {
            DataBase.DeleteRoom(roomName);
        }
    }
    public string RoomName;
    private int MaxPlayers;
    private int CurrentPlayers;
    public readonly Cubes Cubes = new Cubes();
    private List<Player> Players;
    private bool IsGameStarted;

    private int GlobalWaitedTime;
    private int PlayerWaitingId;
    private int PlayerWaitedTime;
    
    private Point[] Points = new Point[36];
    
    //Links
    [JsonIgnore] public int MaxPlayersP => MaxPlayers;
    [JsonIgnore] public int CurrentPlayersP => CurrentPlayers;
    [JsonIgnore] public List<Player> PlayersP => Players;
    [JsonIgnore] public bool IsGameStartedP => IsGameStarted;
    
    public async Task StayAlive()
    {
        bool Alive = true;
        while (Alive)
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
                    DataBase.DeleteRoom(RoomName);
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
                    DataBase.DeleteRoom(RoomName);
                    Alive = false;
                }

                PlayerWaitedTime++;
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
    
    private void PlayerNextCellPosition(int id, int x, int y)
    {
        PlayerWaitedTime = 0;
        Players[id].CellPosition = Players[id].CellPosition + (x + y);
        if (Players[id].CellPosition >= Points.Length)
        {
            Players[id].CellPosition = -(Points.Length - Players[id].CellPosition);
        }
    }

    public bool JoinToRoom(int idInList)
    {
        try
        {
            if (CurrentPlayers <= MaxPlayers && !DataBase.Players[idInList].InGame)
            {
                DataBase.Players[idInList].InGame = true; 
                Players.Add(new Player(DataBase.Players[idInList]));
                CurrentPlayers++;
                return true; 
            }
            else
            {
                return false;
            }
        }
        catch (Exception E)
        {
            Console.WriteLine(E);
            throw;
        }
    }
    
    public bool LeaveFromRoom(int idInList)
    {
        try
        {
            if (DataBase.Players[idInList].InGame)
            {
                for (int I = 0; I < Players.Count; I++)
                {
                    if (Players[I].Id == DataBase.Players[idInList].Id)
                    {
                        DataBase.Players[idInList].InGame = false;
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
        catch (Exception E)
        {
            Console.WriteLine(E);
            throw;
        }
    }

    private void SetPoints()
    {
        Points[0] = new Point(PointType.Start.ToString(), 0, 0, PointType.Start);
        Points[1] = new Point("Blocada", 50, 0, PointType.Game);
        Points[2] = new Point("Luck", 200, 0, PointType.Luck);
        Points[3] = new Point("PayDay2", 200, 0, PointType.Game);
        Points[4] = new Point("Warface",130, 0, PointType.Game);
        Points[5] = new Point("Microsoft Store",300, 0, PointType.Shop);
        Points[6] = new Point("World Of Tanks", 160, 0, PointType.Game);
        Points[7] = new Point("Tanki Online", 160, 0, PointType.Game);
        Points[8] = new Point("Tanki X", 200, 0, PointType.Game);
        Points[9] = new Point("Ban", 0, 0, PointType.Ban);
        Points[10] = new Point("Block Strike", 260, 0, PointType.Game);
        Points[11] = new Point("Alpha ACE", 280, 0, PointType.Game);
        Points[12] = new Point("Luck", 280, 0, PointType.Luck);
        Points[13] = new Point("Standoff 2", 280, 0, PointType.Game);
        Points[14] = new Point("UPlay",300, 0, PointType.Shop);
        Points[15] = new Point("CyberPunk 2077",300, 0, PointType.Game);
        Points[16] = new Point("Luck",300, 0, PointType.Luck);
        Points[17] = new Point("Stray",300, 0, PointType.Game);
        Points[18] = new Point("I'm Smoke",300, 0, PointType.Pass);
        Points[19] = new Point("Scrap Mechanik",300, 0, PointType.Game);
        Points[20] = new Point("Terraria",300, 0, PointType.Game);
        Points[21] = new Point("Minecraft",320, 0, PointType.Game);
        Points[22] = new Point("Epic Games",300, 0, PointType.Shop);
        Points[23] = new Point("Enlisted",320, 0, PointType.Game);
        Points[24] = new Point("Luck",320, 0, PointType.Luck);
        Points[25] = new Point("RavenField",320, 0, PointType.Game);
        Points[26] = new Point("BattleField 2042",320, 0, PointType.Game);
        Points[27] = new Point("ToBan",320, 0, PointType.ToBan);
        Points[28] = new Point("Half-Life",380, 0, PointType.Game);
        Points[29] = new Point("Portal 2",380, 0, PointType.Game);
        Points[30] = new Point("CS:GO",400, 0, PointType.Game);
        Points[31] = new Point("Steam",300, 0, PointType.Shop);
        Points[32] = new Point("Luck",400, 0, PointType.Luck);
        Points[33] = new Point("Crossout",500, 0, PointType.Game);
        Points[34] = new Point("Luck",500, 0, PointType.Luck);
        Points[35] = new Point("War Thunder",500, 0, PointType.Game);
    }

    private int FindIdOnLocalList(int idInList)
    {
        for (int I = 0; I < Players.Count; I++)
        {
            if (Players[I].Id == DataBase.Players[idInList].Id)
            {
                return I;
            }
        }

        return -1;
    }

    public string ToJsonForState()
    {
        var Settings = new JsonSerializerSettings() { ContractResolver = new ContractResolver() };
        return JsonConvert.SerializeObject(this, Settings);
    }

    /*public string ToJsonForLobby()
    {
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
        string ToReturn = "{" + "\"PlayerWaitingId\": " + PlayerWaitingId + ", \"PlayerWaitTime\": " + PlayerWaitedTime  + ", \"IsGameStarted\": " + IsGameStarted + ", \"MaxPlayers\": " + MaxPlayers + ", \"CurrentPlayers\": " + CurrentPlayers;
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
    }*/
}