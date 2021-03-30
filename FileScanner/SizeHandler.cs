using Serilog;
using System.IO;

namespace FileScanner
{
    public class SizeHandler : IHandler
    {
        private (float avg, int count) averageSize = (avg: 0, count: 0);
        private ILogger logger;

        public SizeHandler(ILogger logger)
        {
            this.logger = logger.ForContext<NameHandler>();
        }

        public string Name => "Size statistics";

        public string GetStatistics()
        {
            return $"Average file size: {averageSize.avg} bytes";
        }

        public void Handle(FileInfo file)
        {
            averageSize = (avg: (averageSize.avg * averageSize.count + file.Length) / (averageSize.count + 1), count: averageSize.count + 1);
        }
    }




}
