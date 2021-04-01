using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace FileScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FileScanner", "log.txt"), rollingInterval: RollingInterval.Day)
                .WriteTo.File(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FileScanner", "logext.txt"), 
                    rollingInterval: RollingInterval.Day, 
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Properties:SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Started application in directory {StartingDirectory}", Directory.GetCurrentDirectory());

            var provider = new ServiceCollection()
                .AddTransient<IHandler, NameHandler>()
                .AddTransient<IHandler, ExtensionHandler>()
                .AddTransient<IHandler, SizeHandler>()
                .AddTransient<IHandler, ModifiedDateHandler>()
                .AddTransient<FileScanner>()
                .AddSingleton<ILogger>(Log.Logger)
                .BuildServiceProvider();

            var scanner = provider.GetRequiredService<FileScanner>();
            try
            {
                Log.Verbose("Starting scan");
                scanner.Scan();
                Log.Verbose("Getting statistics");
                string stats = scanner.GetStatistics();
                Log.Debug(stats);
                Console.WriteLine(stats);
            }
            catch(Exception e)
            {
                Log.Error(e, "An error occurred that caused to program to exit");
                Console.Error.WriteLine("An unexpected error occurred. Please, contact your system administrator.");
            }
            finally
            {
                Log.Information("Done, exiting application");
                Log.CloseAndFlush();
            }

        }
    }
}
