using MonopoListGameServer;

namespace MonopoListGameServer_Test;

public class UnitTests
{
    static Room Room = new Room("Name", 2);

    public static void Test()
    {
        //DataBase.Rooms = new List<Room>();
    }

    public static async Task TestGetJsonWithOneKey()
    {
        Console.WriteLine(DataBase.GetJsonWithOneKey("Leaved", true.ToString()));
    }

    public static async Task RoomTest()
    {
        //Console.WriteLine("Accounts Length: " + DataBase.Players.Length);
        Room.JoinToRoom(1);
        Room.JoinToRoom(2);

        Task Task = Room.DropCubes(1);
        Task.GetAwaiter().GetResult();
        
        await Task.Delay(5000);

        RoomToJsonTest();
    }
    
    public static void RoomToJsonTest()
    {
        RoomTest();
        Console.WriteLine("State: " + Room.ToJsonForState());
    }
}