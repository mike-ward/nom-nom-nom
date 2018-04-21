using nom_nom_nom.Infrastructure;

namespace nom_nom_nom.Services
{
    public class CommandLineOptions : ICommandLineOptions
    {
        public enum ActionType { Report, Append, Help }

        public ActionType Action { get; private set; }
        public string Parameters { get; private set; }

        public ICommandLineOptions Parse(string[] args)
        {
            var action = ActionType.Report;
            if (args.Length == 0) action = ActionType.Report;
            else if (args[0].Length > 1) action = ActionType.Append;
            var parameters = string.Join("", args);
            return new CommandLineOptions { Action = action, Parameters = parameters };
        }
    }
}
