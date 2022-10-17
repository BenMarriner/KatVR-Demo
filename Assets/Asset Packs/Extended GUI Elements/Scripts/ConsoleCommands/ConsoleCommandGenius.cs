using UnityEngine;
using UnityEngine.UI;

public class ConsoleCommandGenius : ConsoleCommand
{
    int guesses = 0;
    int number = 0;

    public ConsoleCommandGenius(string command, string help, bool displayinhelp, string[] AbstractedCommands)
        : base(command, help, displayinhelp, AbstractedCommands)
    {
    }

    public override void ExecuteCommand(Console console, string[] args)
    {
        base.ExecuteCommand(console, args);
        guesses = 0;
        number = Random.Range(1, 100);
        console.ReadLine(guessentered);
        console.WriteLine("Are you a genius? try to guess the number I'm thinking of between 1 and 100! :)");
    }

    private void guessentered(Console console, string input)
    {
        int guess = 0;

        if (int.TryParse(input, out guess))
        {
            if (guess == number)
            {
                console.WriteLine("You got it! In \"only\" " + guesses + " tries! :)");
                return;
            }
            else if (guess > number)
            {
                console.WriteLine("The number is smaller!");
            }
            else if (guess < number)
            {
                console.WriteLine("The number is larger!");
            }
            guesses++;
        }
        else
        {
            console.WriteLine("Only numbers can be entered!");
        }
        console.ReadLine(guessentered);
    }
}
