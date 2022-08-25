namespace MonopolyGameServer;

public class Point
{
    public Point(int buyPrice, int rentalPrice)
    {
        BuyPrice = buyPrice;
        RentalPrice = rentalPrice;
        IsBought = false;
    }

    public int BuyPrice { get; set; }
    public int RentalPrice { get; set; }
    public bool IsBought = false;
}