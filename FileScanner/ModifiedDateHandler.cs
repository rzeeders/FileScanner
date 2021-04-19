using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileScanner
{
    public class ModifiedDateHandler : IHandler
    {
        private Dictionary<int, int> numberOfFilesPerYear = new Dictionary<int, int>();
        private ILogger logger;

        public ModifiedDateHandler(ILogger logger)
        {
            this.logger = logger.ForContext<NameHandler>();
        }

        public string Name => "Modified date statistics";

        public string GetStatistics()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var kvp in numberOfFilesPerYear.ToList().OrderByDescending(x => x.Key))
            {
                logger.Information("Number of files for year {ModifiedDate_YearStr} is {ModifiedDate_NumberOfFiles}", $"{kvp.Key}", kvp.Value);
                sb.AppendLine($"{kvp.Key}: {kvp.Value} files");
            }
            return sb.ToString();
        }

        public void Handle(FileInfo file)
        {
            if (!numberOfFilesPerYear.TryGetValue(file.LastWriteTimeUtc.Year, out int count)) count = 0;
            numberOfFilesPerYear[file.LastWriteTimeUtc.Year] = count + 1;
        }
    }




}
