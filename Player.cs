using Newtonsoft.Json;

namespace MonopoListGameServer;

//TODO create field UserDateRegister and UserLastSession
public class Player : SavedPlayer
{
    
    public int CellPosition = 0;
    public bool Bankrupt = false;
    public int Balance = 0;

    public Player(SavedPlayer registeredPlayer) : base(registeredPlayer.Id, registeredPlayer.NickName, registeredPlayer.Password)
    {
        this.Id = registeredPlayer.Id;
        this.NickName = registeredPlayer.NickName;
        this.Password = registeredPlayer.Password;
    }

    public string ToJson()
    {
        return "{\"ID\": " + Id + ", \"NickName\": \"" + NickName + "\", \"Password\": \"" + Password + "\", \"InGame\": " + InGame + ", \"Balance\": " + Balance + ", \"CellPosition\": " + CellPosition + "}";
    }
}

public class SavedPlayer
{
    [JsonIgnore] public ulong Id;
    public string NickName;
    [JsonIgnore] public string Password;
    [JsonIgnore] public bool InGame = false;
    
    public SavedPlayer(ulong id, string nickName, string password)
    {
        Id = id;
        NickName = nickName;
        Password = password;
    }
    
    public string ToJsonSaved()
    {
        return "{\"ID\": " + Id + ", \"NickName\": \"" + NickName + "\", \"Password\": \"" + Password + "\"}";
    }
    
    public string ToJsonSavedNick()
    {
        return "{\"NickName\": \"" + NickName + "\"}";
    }
}