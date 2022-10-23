using UnityEngine.UI;

public class ConsoleCommandPasswordTest : ConsoleCommand
{
    string user;

    public ConsoleCommandPasswordTest(string command, string help, bool displayinhelp, string[] AbstractedCommands)
        : base(command, help, displayinhelp, AbstractedCommands)
    {
    }

    public override void ExecuteCommand(Console console, string[] args)
    {
        base.ExecuteCommand(console, args);
        if (args.Length == 2 && args[1] != "?")
        {
            console.isPasswordLine = true;
            console.ReadLine(ValidatePassword);
            user = args[1];
            console.WriteLine("Enter password: (Hint it is: 'asd123')");
        }
        else
        {
            HelpCommand(console);
        }
    }

    public override void HelpCommand(Console console)
    {
        base.HelpCommand(console);
        console.WriteLine("format: 'login user' afterwards you will be asked for a password.");
    }

    private void ValidatePassword(Console console, string pass)
    {
        if (pass == "asd123")
        {
            console.WriteLine("Logged in for user was correct! Welcome " + user + "!");
        }
        else
        {
            console.WriteLine("Logged in was wrong!");
        }
        console.isPasswordLine = false;
    }
}
