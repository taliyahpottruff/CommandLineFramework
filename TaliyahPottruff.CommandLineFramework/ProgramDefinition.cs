namespace TaliyahPottruff.CommandLineFramework;

public class ProgramDefinition<T>(string name, IEnumerable<CommandDefinition<T>> commands)
{
    public CommandDefinition<T>[] Commands { get; } = commands.ToArray();
    public string Version { get; init; } = "v1.0.0";
    public string Name { get; } = name;

    /// <summary>
    /// Tries to execute a command from the given arguments.
    /// </summary>
    /// <param name="args">Args passed by the command line</param>
    /// <param name="passthrough">The passthrough object</param>
    /// <returns>Whether the specified command was found and executed.</returns>
    public bool ParseCommand(string[] args, T passthrough)
    {
        if (args.Length == 0)
        {
            Console.WriteLine($"{Name} - {Version}");
            OutputAvailableCommands();
            return false;
        }

        if (args[0] == "-h" || args[0] == "--help")
        {
            OutputAvailableCommands();
            return true;
        }

        if (args[0] == "--version" || args[0] == "-v")
        {
            Console.WriteLine(Version);
            return true;
        }

        foreach (var command in commands)
        {
            if (command.Command.Equals(args[0], StringComparison.OrdinalIgnoreCase))
            {
                return command.Parse(args[1..], passthrough);
            }
        }
        
        Console.WriteLine($"Unknown command '{args[0]}'.");
        OutputAvailableCommands();
        return false;
    }

    private void OutputAvailableCommands()
    {
        Console.WriteLine("Available commands:");
        foreach (var command in Commands)
        {
            Console.WriteLine($"{command.Command} - {command.Description}");
        }
    }
}