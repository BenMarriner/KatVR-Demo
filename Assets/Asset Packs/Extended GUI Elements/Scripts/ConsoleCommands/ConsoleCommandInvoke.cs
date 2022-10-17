using UnityEngine.UI;

public class ConsoleCommandInvoke : ConsoleCommand
{
    public CommandCallBack callbackMethod { get; protected set; }

    public ConsoleCommandInvoke(string command, string help, bool displayinhelp, string[] AbstractedCommands, CommandCallBack callbackMethod)
        : base(command, help, displayinhelp, AbstractedCommands)
    {
        this.callbackMethod = callbackMethod;
    }

    public override void ExecuteCommand(Console console, string[] args)
    {
        callbackMethod.Invoke();
    }
}

public delegate void CommandCallBack();