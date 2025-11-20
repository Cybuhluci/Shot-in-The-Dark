using UnityEngine;

public class CommandGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CommandRegistry.Register(new ConsoleCommand {
            Name = "echo",
            Description = "Echoes the input.",
            Execute = args => Debug.Log(string.Join(" ", args))
        });

        CommandRegistry.Register(new ConsoleCommand {
            Name = "givepoints",
            Description = "Gives the player points. Usage: givepoints <amount>",
            Execute = args => {
                if (args.Length > 0 && int.TryParse(args[0], out int amount))
                    FindAnyObjectByType<PointsScript>()?.AddPoints(amount);
                else
                    Debug.Log("Usage: givepoints <amount>");
            }
        });

        // Add more commands here...
    }
}
