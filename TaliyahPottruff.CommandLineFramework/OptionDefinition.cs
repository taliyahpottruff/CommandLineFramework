namespace TaliyahPottruff.CommandLineFramework;

public class OptionDefinition(string option, string description, params string[] aliases)
{
    public string Option { get; } = option;
    public string Description { get; } = description;
    public string[] Aliases { get; } = aliases;
    public bool Required { get; init; } = false;
    public string RequiredErrorMessage { get; init; } = $"Please use the -{option} option.";
}