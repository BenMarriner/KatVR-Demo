using UnityEngine.UI;

public abstract class ConsoleCommand
{
    public string command { get; protected set; }
    public string[] AbstractedCommands { get; private set; }
    public string help { get; protected set; }
    public bool displayInHelp { get; protected set; }

    public ConsoleCommand(string command, string help, bool displayinhelp, string[] AbstractedCommands)
    {
        this.command = command;
        this.help = help;
        this.displayInHelp = displayinhelp;
        this.AbstractedCommands = AbstractedCommands;
    }

    public virtual void ExecuteCommand(Console console, string[] args)
    {
        if (args.Length == 2 && args[1] == "?")
        {
            HelpCommand(console);
        }
    }

    public virtual void HelpCommand(Console console)
    {
        console.WriteLine(command + ": " + help);
    }
}

