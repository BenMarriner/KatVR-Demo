using UnityEngine;
using UnityEngine.UI;

public class ConsoleCommandColors : ConsoleCommand
{
    public ConsoleCommandColors(string command, string help, bool displayinhelp, string[] AbstractedCommands)
        : base(command, help, displayinhelp, AbstractedCommands)
    {
    }

    public override void ExecuteCommand(Console console, string[] args)
    {
        base.ExecuteCommand(console, args);
        if (args.Length == 3 && args[1].Length == 6 && args[2].Length == 6)
        {
            console.SetTextColor(new Color32(System.Convert.ToByte(args[1].Substring(0, 2), 16), System.Convert.ToByte(args[1].Substring(2, 2), 16), System.Convert.ToByte(args[1].Substring(4, 2), 16), byte.MaxValue));
            console.SetBackgroundColor(new Color32(System.Convert.ToByte(args[2].Substring(0, 2), 16), System.Convert.ToByte(args[2].Substring(2, 2), 16), System.Convert.ToByte(args[2].Substring(4, 2), 16), byte.MaxValue));
        }
        else if (args.Length == 2 && args[1].Length == 6)
        {
            console.SetTextColor(new Color32(System.Convert.ToByte(args[1].Substring(0, 2), 16), System.Convert.ToByte(args[1].Substring(2, 2), 16), System.Convert.ToByte(args[1].Substring(4, 2), 16), byte.MaxValue));
        }
        else
        {
            HelpCommand(console);
        }
    }

    public override void HelpCommand(Console console)
    {
        console.WriteLine(command + ": " + help + "\n" + "The colors are represented with 6 hex numbers (RGB). The format is color {Foreground} {background} where background is optional. typing 'color 00ff00 000000' provides green text on a black background.");
    }
}