using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Console))]
public class CommandSetup : MonoBehaviour
{
    Console con;
    void Start ()
    {
        con = this.GetComponent<Console>();
        InitCommands();
    }

    public void InitCommands ()
    {
        con.RegisterCommand(new ConsoleCommandInvoke("help", "Shows thus help message.", true, new string[] { "?" }, con.DisplayCommands));
        con.RegisterCommand(new ConsoleCommandInvoke("cls", "Clears the console.", true, new string[] { "clear" }, con.ClearConsole));
        con.RegisterCommand(new ConsoleCommandArgsInvoke("echo", "Displays messages.", true, new string[] { }, Echo));
        con.RegisterCommand(new ConsoleCommandArgsInvoke("babble", "Displays messages a numbers of times, format: cmd number message.", true, new string[] { }, Babble));
        con.RegisterCommand(new ConsoleCommandColors("color", "Changes the color of the text and/or background.", true, new string[] { "colour" }));
        con.RegisterCommand(new ConsoleCommandGenius("genius", "Starts a game of genius.", true, new string[] { }));
        con.RegisterCommand(new ConsoleCommandPasswordTest("login", "Tests a login command.", true, new string[] { "logon" }));


    }

    private void Echo(string[] args)
    {
        string s = "";
        for (int i = 1; i < args.Length; i++)
        {
            if (i == args.Length - 1)
            {
                s += args[i];
            }
            else
            {
                s += args[i] + " ";
            }
        }
        con.WriteLine(s);
    }

    private void Babble(string[] args)
    {
        string s = "";
        int r = 0;
        if (args.Length >= 3 && int.TryParse(args[1], out r))
        {
            for (int i = 2; i < args.Length; i++)
            {
                if (i == args.Length - 1)
                {
                    s += args[i];
                }
                else
                {
                    s += args[i] + " ";
                }
            }

            for (int i = 0; i < r; i++)
            {
                con.WriteLine(s);
            }
        }
        else
        {
            con.WriteLine("Displays messages a numbers of times, format: cmd number message.");
        }
    }
}
