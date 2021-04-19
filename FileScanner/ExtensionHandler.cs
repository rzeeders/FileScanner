using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileScanner
{
    public class ExtensionHandler : IHandler
    {
        private Dictionary<string, int> extensions = new Dictionary<string, int>();
        private ILogger logger;

        public ExtensionHandler(ILogger logger)
        {
            this.logger = logger.ForContext<NameHandler>();
        }

        public string Name => "Extension statistics";

        public string GetStatistics()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in extensions.ToList().OrderByDescending(x => x.Value))
            {
                logger.Information("Number of files for extension {Extension} is {ExtensionFileCount}", kvp.Key, kvp.Value);

                sb.AppendLine($"{kvp.Key}: {kvp.Value} files");
            }
            return sb.ToString();
        }

        public void Handle(FileInfo file)
        {
            if (!extensions.TryGetValue(file.Extension, out int count)) count = 0;
            extensions[file.Extension] = count + 1;
        }
    }




}
