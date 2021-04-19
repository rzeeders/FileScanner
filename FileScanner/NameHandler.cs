using Serilog;
using System.IO;

namespace FileScanner
{
    public class NameHandler : IHandler
    {
        private (float avg, int count) averageNameLength = (avg: 0, count: 0);
        private ILogger logger;

        public NameHandler(ILogger logger)
        {
            this.logger = logger.ForContext<NameHandler>();
        }

        public string Name => "Name statistics";

        public string GetStatistics()
        {
            logger.Information("Average size is {AverageNameLength}", averageNameLength.avg);
            return $"Average name length: {averageNameLength.avg} characters";
        }

        public void Handle(FileInfo file)
        {
            averageNameLength = (avg: (averageNameLength.avg * averageNameLength.count + file.Name.Length) / (averageNameLength.count + 1), count: averageNameLength.count + 1);
        }
    }




}
