using System;
using nom_nom_nom.Infrastructure;
using nom_nom_nom.Services;

public class Application : IApplication
{
    private readonly ICommandLineOptions _options;
    private readonly IReport _report;

    public Application(ICommandLineOptions options, IReport report)
    {
        _options = options;
        _report = report;
    }

    public void Run(string[] args) 
    {
        var options = _options.Parse(args);

        switch (options.Action)
        {
            case CommandLineOptions.ActionType.Report:
                Console.WriteLine(_report.Summary());
                break;

            default:
                Console.WriteLine("There is no help for you");
                break;
        }
    }
}