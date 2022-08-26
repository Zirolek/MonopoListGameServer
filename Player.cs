namespace MonopoListGameServer;

//TODO create field UserDateRegister and UserLastSession
public class Player : RegisteredPlayer
{
    public int CellPosition = 0;
    public bool Bankrupt = false;
    public int Balance = 0;

    public Player(RegisteredPlayer registeredPlayer) : base(registeredPlayer.Id, registeredPlayer.NickName, registeredPlayer.Password)
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

public class RegisteredPlayer
{
    public ulong Id;
    public string NickName;
    public string Password;
    public bool InGame = false;

    public RegisteredPlayer(ulong id, string nickName, string password)
    {
        Id = id;
        NickName = nickName;
        Password = password;
    }

    public string ToJsonRegistered()
    {
        return "{\"ID\": " + Id + ", \"NickName\": \"" + NickName + "\", \"Password\": \"" + Password + "}";
    }
}