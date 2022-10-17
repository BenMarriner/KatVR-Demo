using UnityEngine.UI;

public class ConsoleCommandArgsInvoke : ConsoleCommand
{
    public CommandArgsCallBack callbackMethod { get; protected set; }

    public ConsoleCommandArgsInvoke(string command, string help, bool displayinhelp, string[] AbstractedCommands, CommandArgsCallBack callbackMethod)
        : base(command, help, displayinhelp, AbstractedCommands)
    {
        this.callbackMethod = callbackMethod;
    }

    public override void ExecuteCommand(Console console, string[] args)
    {
        callbackMethod.Invoke(args);
    }
}

public delegate void CommandArgsCallBack(string[] args);