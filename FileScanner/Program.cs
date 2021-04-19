using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;

namespace FileScanner
{
    /**
     * Deze applicatie demonstreert logging & monitoring mbv serilog en de ELK stack.
     * De app scant de huidige working directory en alle subfolders, en genereert 
     * een aantal statistieken adhv de bestanden die het tegen komt.
     * 
     * Om de logging en de ELK stack in actie te zien, neem je de volgende stappen:
     * 
     *  1. Install docker (https://www.docker.com/get-started)
     *  2. Install ELK stack in docker (zie ook https://logz.io/blog/elk-stack-on-docker/)
     *    i.   open Powershell als administrator
     *    ii.  ga naar directory waar je de solution wilt hebben
     *    iii. run `git clone https://github.com/deviantony/docker-elk.git`
     *    iv.  run `cd docker-elk`
     *    v.   run `docker-compose up -d` - dit duurt eventjes, daarna is de 
     *         ELK stack beschikbaar (kibana standaard gebr naam = 'elastic' en wachtwoord = 'changeme')
     *  3. Run de FileScanner applicatie minstens 1 keer (verander de working directory via File 
     *     scanner project properties - rechter muisknop op project FileScanner > properties,
     *     dan op tab 'Debug' de 'Working Directory'.
     *  4. Open Kibana via localhost:5601
     *  5. Creeer een nieuw dashboard
     *  6. voeg een Panel toe, kies voor 'Lens'
     *  7. Kies 'Index pattern' 'logstash*'
     *  8. Kies Donut chart type
     *  9. Sleep 'fields.Extension.keyword' naar 'Slice by'
     *  10. Sleep 'field.ExtensionFileCount' naar 'Size by'
     *  11. Klik op 'Save and return'
     * 
     */

    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("StartingDirectory", Directory.GetCurrentDirectory())
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "changeme"),
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                })
                .WriteTo.File(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FileScanner", "log.txt"), rollingInterval: RollingInterval.Day)
                //.WriteTo.File(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FileScanner", "logext.txt"), 
                //    rollingInterval: RollingInterval.Day, 
                //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Properties:SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var logger = Log.Logger.ForContext<Program>();

            logger.Information("Started application in directory {StartingDirectory}", Directory.GetCurrentDirectory());

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
                scanner.Scan();
                string stats = scanner.GetStatistics();
                Console.WriteLine(stats);
            }
            catch(Exception e)
            {
                logger.Fatal(e, "Fatal exception occurred, terminating application");
                Console.Error.WriteLine("An unexpected error occurred. Please, contact your system administrator.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
