using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileScanner
{
    public class FileScanner
    {
        private IEnumerable<IHandler> handlers;
        private ILogger logger;

        public FileScanner(IEnumerable<IHandler> handlers, ILogger logger)
        {
            this.handlers = handlers;
            this.logger = logger.ForContext<FileScanner>();
        }

        public void Scan(string directory = ".")
        {
            foreach(var dir in EnumerateDirectories(directory))
            {
                Scan(dir);
            }

            int count = 0;

            foreach (var file in EnumerateFiles(directory))
            {
                count++;
                FileInfo fi = new FileInfo(file);
                foreach(var handler in handlers)
                {
                    this.logger.Verbose("Handling file {File}", fi.FullName);
                    handler.Handle(fi);
                }
            }

            this.logger.Information("{Count} files in {Directory}", count, directory);
        }

        private IEnumerable<string> EnumerateDirectories(string directory)
        {
            try
            {
                return Directory.EnumerateDirectories(directory);
            }
            catch(System.UnauthorizedAccessException)
            {
                logger.Warning("Not authorized to read {UnauthorizedDirectory}", directory);
                return new string[0];
            }
        }

        private IEnumerable<string> EnumerateFiles(string directory)
        {
            try
            {
                return Directory.EnumerateFiles(directory);
            }
            catch (System.UnauthorizedAccessException)
            {
                logger.Warning("Not authorized to read {UnauthorizedDirectory}", directory);
                return new string[0];
            }
        }


        public string GetStatistics()
        {
            StringBuilder sb = new StringBuilder();
            string lineSeperator = "=====================";
            foreach (var handler in handlers)
            {
                sb.AppendLine(lineSeperator);
                sb.AppendLine(handler.Name);
                sb.AppendLine(lineSeperator);
                sb.AppendLine(handler.GetStatistics());
                sb.AppendLine();
            }
            return sb.ToString();
        }

    }




}
