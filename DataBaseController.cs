using Newtonsoft.Json;

namespace MonopolyGameServer;

public class DataBaseController
{
    public static string Path = AppDomain.CurrentDomain.BaseDirectory;
    public static Player[] Players = new Player[0];
    public static void SerializeUserData()
    {
        try
        {
            for (int I = 0; I < Players.Length; I++)
            {
                Players[I].InGame = false;
                Players[I].CellPosition = 0;
                Players[I].Balance = 0;
            }
            string[] Lines = new string[1];
            for (int I = 0; I < Players.Length; I++)
            {
                Lines = Lines.Concat(new[] { JsonConvert.SerializeObject(Players[I]) }).ToArray();
            }
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
                Player Player = JsonConvert.DeserializeObject<Player>(UsersFileContent[I]);
                if (Player != null)
                {
                    Players = Players.Concat(new [] { Player }).ToArray();
                    //Console.WriteLine(Player.ToJson());
                }
            }
            for (int I = 0; I < Players.Length; I++)
            {
                Players[I].InGame = false;
                Players[I].CellPosition = 0;
                Players[I].Balance = 0;
            }
        }
        catch (Exception Exception)
        {
            Console.WriteLine("Failed load user data! \n" + Exception);
        }
    }

    public static int RegisterNewPlayer(string nick, string passWord)
    {
        Random Random = new Random();
        Player NewPlayer = new Player();
        NewPlayer.Id = (ulong) Random.NextInt64(999999999999999999);
        NewPlayer.NickName = nick;
        NewPlayer.Password = passWord;
        if (CheckPlayer(NewPlayer))
        {
            Players = Players.Concat(new [] { NewPlayer }).ToArray();
        }
        return GetIdInList(NewPlayer.NickName, NewPlayer.Password);
    }

    public static bool CheckPlayer(Player player)
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