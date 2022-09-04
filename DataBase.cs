using System.Text;
using Newtonsoft.Json;

namespace MonopoListGameServer;

public class DataBase
{
    protected internal static string Path = AppDomain.CurrentDomain.BaseDirectory;
    protected internal static List<Room> Rooms = new List<Room>();
    protected internal static SavedPlayer[] Players = new SavedPlayer[0];
    public static void SerializeUserData()
    {
        try
        {
            string[] Lines = new string[0];
            for (int I = 0; I < Players.Length; I++)
            {
                //Lines = Lines.Concat(new[] { JsonConvert.SerializeObject(Players[I]) }).ToArray();
                Lines = Lines.Concat(new[] { Players[I].ToJsonSaved() }).ToArray();
            }
            Console.WriteLine("Players.Length: " + Players.Length + " Lines.Length: " + Lines.Length);
            File.WriteAllLines(Path + "users.json", Lines);
        }
        catch (Exception Exception)
        { 
            Console.WriteLine("Can't create file! \n" + Exception);
        }
    }
    public static void DeSerializeUserData()
    {
        try
        {
            string[] UsersFileContent = File.ReadAllLines("users.json");
            for (int I = 0; I < UsersFileContent.Length; I++)
            {
                SavedPlayer Player = JsonConvert.DeserializeObject<SavedPlayer>(UsersFileContent[I]);
                if (Player != null)
                {
                    Players = Players.Concat(new [] { Player }).ToArray();
                    Console.WriteLine(Player.ToJsonSaved());
                }
            }
        }
        catch (Exception Exception)
        {
            Console.WriteLine("Failed load user data! \n" + Exception);
        }
    }

    public static bool CreateNewRoom(string name, int maxPlayers)
    {
        int IsExists = GetRoomByName(name);
        if (Rooms.Count <= 20 && IsExists == -1 && maxPlayers >= 2)
        {
            Rooms.Add(new Room(name, maxPlayers));
            return true;
        }

        return false;
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
    
    public static string GetRoomState(string name)
    {
        int Id = GetRoomByName(name);
        if (Id != -1)
        {
            return Rooms[Id].ToJsonForState();
        }
        
        return "";
    }

    
    public static void DeleteRoom(string name)
    {
        int RoomId = DataBase.GetRoomByName(name);
        if (RoomId != -1)
        {
            Rooms[RoomId] = null;
            Rooms.Remove(null);
        }
    }

    public static string GetJsonWithOneKey(string key, string value)
    {
        if (value == "")
        {
            return "{\"" + key + "\": " + null + "}";
        }
        
        if (value == "True")
        {
            return "{\"" + key + "\": " + value + "}";
        }
        
        if (value == "False")
        {
            return "{\"" + key + "\": " + value + "}";
        }
        
        return "{\"" + key + "\": \"" + value + "\"}";
    }

    public static int RegisterNewPlayer(string nick, string passWord)
    {
        Random Random = new Random();
        SavedPlayer NewPlayer = new SavedPlayer(((ulong)Random.NextInt64(999999999999999999)), nick, passWord);
        if (CheckPlayer(NewPlayer))
        {
            Players = Players.Concat(new [] { NewPlayer }).ToArray();
        }
        return GetIdInList(NewPlayer.NickName, NewPlayer.Password);
    }

    public static bool CheckPlayer(SavedPlayer player)
    {
        bool ToReturn = true;
        for (int I = 0; I < Players.Length; I++)
        {
            if (player.NickName == Players[I].NickName)
            {
                ToReturn = false;
            }
            if (player.Id == Players[I].Id)
            {
                ToReturn = false;
            }
        }
        return ToReturn;
    }

    public static int IsAccountExists(string login)
    {
        for (int I = 0; I < Players.Length; I++)
        {
            if (Players[I].NickName == login)
            {
                return I;
            }
        }

        return -1;
    }

    public static int GetIdInList(string login, string pass)
    {
        try
        {
            int IsExists = IsAccountExists(login);
            if (Players[IsExists].Password == pass)
            {
                return IsExists;
            }
        }
        catch
        {
            return -1;
        }

        return -1;
    }
    public static int GetIdInList(ulong playerId)
    {
        try
        {
            for (int I = 0; I < Players.Length; I++)
            {
                if (Players[I].Id == playerId)
                {
                    return I;
                }
            }
        }
        catch
        {
            return -1;
        }

        return -1;
    }
}