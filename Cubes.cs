using Newtonsoft.Json;

namespace MonopoListGameServer;

public class Cubes
{
    private int FirstCubeResult;
    private int SecondCubeResult;

    [JsonIgnore] public int FirstCubeResultP => FirstCubeResult;
    [JsonIgnore] public int SecondCubeResultP => FirstCubeResult;

    public void Next()
    {
        Random Rnd = new Random();
        FirstCubeResult = Rnd.Next(7);
        SecondCubeResult = Rnd.Next(7);
    }

    public string ToJson()
    {
        return "{\"FirstCubeResult\": " + FirstCubeResult + ", \"SecondCubeResult\": " + SecondCubeResult + "}";
    }
}