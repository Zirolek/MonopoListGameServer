namespace MonopoListGameServer;

public class Commands
{
    public Commands()
    {
        while (true)
        {
            string? Command = Console.ReadLine();
            if (Command != null) Commands.GetCommand(Command);
        }
    }

    public static void GetCommand(string? command)
    {
        switch (command)
        {
            case "/help":
                Console.WriteLine("/stop : disable bot");
                Console.WriteLine("/save : save user data");
                Console.WriteLine("/help : get commands list");
                break;

            case "/save":
                Console.WriteLine("Starting save");
                DataBase.SerializeUserData();
                break;
            
            case "/stop":
                Console.WriteLine("Save? (yes / no / cancel)");
                string Input = Console.ReadLine();
                switch (Input)
                {
                    case "yes":
                        DataBase.SerializeUserData();
                        Server.StopListener();
                        Environment.Exit(0);
                        break;

                    case "no":
                        Server.StopListener();
                        Environment.Exit(0);
                        break;

                    case "cancel":
                        return;

                    default:
                        Console.WriteLine("yes / no / cancel !");
                        GetCommand("/stop");
                        break;
                }

                break;

            default:
                Console.WriteLine("Unknow command. Use /help to get commands.");
                break;

        }
    }
}