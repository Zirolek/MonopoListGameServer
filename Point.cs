using Newtonsoft.Json;

namespace MonopoListGameServer;

public class Point
{
    public Point(string name , int buyPrice, int rentalPrice, PointType type)
    {
        Name = name;
        BuyPrice = buyPrice;
        RentalPrice = rentalPrice;
        IsBought = false;
        Type = type;
    }

    private string Name;
    private int BuyPrice;
    private int RentalPrice;
    private PointType Type;
    private bool IsBought;
    [JsonIgnore] public bool IsBoughtP => IsBought;
    [JsonIgnore] public string NameP => Name;

    public void Buy()
    {
        if (Type == PointType.Game || Type == PointType.Shop)
        {
            IsBought = true;
        }
    }

    public int Price()
    {
        if (IsBought)
        {
            return RentalPrice;
        }
        else
        {
            return BuyPrice;
        }
    }
}

public enum PointType
{
    Start,
    Game,
    Luck,
    Shop,
    ToBan,
    Ban,
    Pass
}