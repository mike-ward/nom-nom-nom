using System;
using Microsoft.Extensions.DependencyInjection;
using nom_nom_nom.Infrastructure;
using nom_nom_nom.Services;

namespace nom_nom_nom
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = RegisterServices();
            var application = container.GetService<IApplication>();
            application.Run(args);
        }

        private static IServiceProvider RegisterServices() 
        {
            var services = new ServiceCollection();
            services.AddSingleton<IApplication, Application>();
            services.AddTransient<ICommandLineOptions, CommandLineOptions>();
            services.AddTransient<IDataFile, DataFile>();
            services.AddTransient<IReport, Report>();
            return services.BuildServiceProvider();
        }
    }
}
