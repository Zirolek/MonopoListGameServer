namespace MonopoListGameServer;

//TODO create field UserDateRegister and UserLastSession
public class Player
{
    public ulong Id;
    public string NickName;
    public string Password;
    public bool InGame = false;
    public int CellPosition = 0;
    public bool Bankrupt = false;
    public int Balance = 0;
    public string ToJson()
    {
        return "{\"ID\": " + Id + ", \"NickName\": \"" + NickName + "\", \"Password\": \"" + Password + "\", \"InGame\": " + InGame + ", \"Balance\": " + Balance + ", \"CellPosition\": " + CellPosition + "}";
    }
}