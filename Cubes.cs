using System.Security.Cryptography;

namespace MonopolyGameServer;

public class Cubes
{
    private int FirstCubeResult;
    private int SecondCubeResult;

    public void Next()
    {
        Random rnd = new Random();
        FirstCubeResult = rnd.Next(7);
        SecondCubeResult = rnd.Next(7);
    }

    public string ToJson()
    {
        return "{\"FirstCubeResult\": " + FirstCubeResult + ", \"SecondCubeResult\": " + SecondCubeResult + "}";
    }

}