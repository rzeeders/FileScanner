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
            this.logger = logger.ForContext<ExtensionHandler>();
        }

        public string Name => "Extension statistics";

        public string GetStatistics()
        {
            logger.Information("Getting statistics");
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in extensions.ToList().OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value} files");
            }
            return sb.ToString();
        }

        public void Handle(FileInfo file)
        {
            logger.Verbose("Handling {File}", file.FullName);
            if (!extensions.TryGetValue(file.Extension, out int count)) count = 0;
            extensions[file.Extension] = count + 1;
        }
    }




}
