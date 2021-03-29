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

            foreach(var file in EnumerateFiles(directory))
            {
                FileInfo fi = new FileInfo(file);
                foreach(var handler in handlers)
                {
                    handler.Handle(fi);
                }
            }
        }

        private IEnumerable<string> EnumerateDirectories(string directory)
        {
            try
            {
                return Directory.EnumerateDirectories(directory);
            }
            catch(System.UnauthorizedAccessException)
            {
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
