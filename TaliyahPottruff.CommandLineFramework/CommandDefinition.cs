namespace TaliyahPottruff.CommandLineFramework;

public abstract class CommandDefinition<T>(string command, string description)
{
    public string Command { get; } = command;
    public string Description { get; } = description;
    public CommandDefinition<T>[] SubCommands { get; init; } = [];
    public OptionDefinition[]? OptionDefinitions { get; init; }
    
    public abstract void Execute(Dictionary<string, string> options, T passthrough);

    public bool Parse(string[] args, T passthrough)
    {
        if (args.Length == 0)
        {
            if (OptionDefinitions is not null && OptionDefinitions.Any(d => d.Required))
            {
                WriteError("Missing required option(s).");
                return false;
            }
            
            Execute([], passthrough);
            return true;
        }
        
        if (args[0] == "-h" || args[0] == "--help")
        {
            Console.WriteLine("Available commands:");
            foreach (var command in SubCommands)
            {
                Console.WriteLine($"{command} - {command.Description}");
            }
            return true;
        }

        if (args[0].StartsWith('-'))
        {
            var unparsed = new Dictionary<string, string>();

            var flag = string.Empty;
            foreach (var arg in args)
            {
                if (arg.StartsWith('-'))
                {
                    if (!string.IsNullOrWhiteSpace(flag))
                        unparsed.Add(flag, "");
                    
                    flag = arg;
                    continue;
                }
                
                if (string.IsNullOrEmpty(flag))
                    continue;
                
                unparsed.Add(flag, arg);
                flag = string.Empty;
            }
            
            if (!string.IsNullOrWhiteSpace(flag))
                unparsed.Add(flag, "");
            
            var options = new Dictionary<string, string>();

            if (OptionDefinitions is not null)
            {
                foreach (var optionDefinition in OptionDefinitions)
                {
                    var found = false;
                    if (unparsed.ContainsKey("-" + optionDefinition.Option))
                    {
                        options.Add(optionDefinition.Option, !string.IsNullOrWhiteSpace(unparsed["-" + optionDefinition.Option]) ? unparsed["-" + optionDefinition.Option] : "true");
                        found = true;
                    }

                    foreach (var alias in optionDefinition.Aliases)
                    {
                        if (unparsed.ContainsKey("-" + alias))
                        {
                            options.Add(optionDefinition.Option, !string.IsNullOrWhiteSpace(unparsed["-" + alias]) ? unparsed["-" + alias] : "true");
                            found = true;
                        }
                    }
                    
                    if (found)
                        continue;

                    if (!optionDefinition.Required) continue;
                    WriteError(optionDefinition.RequiredErrorMessage);
                    return false;
                }
            }
            
            Execute(options, passthrough);
            return true;
        }

        foreach (var command in SubCommands)
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
    
    protected void OutputAvailableCommands()
    {
        Console.WriteLine("Available commands:");
        foreach (var command in SubCommands)
        {
            Console.WriteLine($"{command.Command} - {command.Description}");
        }
    }
    
    protected void WriteWarning(string warning)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(warning);
        Console.ResetColor();
    }

    protected void WriteError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
    }
    
    protected void WriteInfo(string info)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(info);
        Console.ResetColor();
    }
}