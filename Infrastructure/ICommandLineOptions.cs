using nom_nom_nom.Services;

namespace nom_nom_nom.Infrastructure
{
    public interface ICommandLineOptions
    {
        CommandLineOptions.ActionType Action { get;  }
        string Parameters { get; }
        ICommandLineOptions Parse(string[] args);
    }
}