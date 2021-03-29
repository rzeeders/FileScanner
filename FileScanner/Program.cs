using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                .AddTransient<IHandler, NameHandler>()
                .AddTransient<IHandler, ExtensionHandler>()
                .AddTransient<IHandler, SizeHandler>()
                .AddTransient<IHandler, ModifiedDateHandler>()
                .AddTransient<FileScanner>()
                .BuildServiceProvider();

            var scanner = provider.GetRequiredService<FileScanner>();
            scanner.Scan();
            Console.WriteLine(scanner.GetStatistics());

        }
    }

    public class FileScanner
    {
        private IEnumerable<IHandler> handlers;

        public FileScanner(IEnumerable<IHandler> handlers)
        {
            this.handlers = handlers;
        }

        public void Scan(string directory = ".")
        {
            foreach(var dir in Directory.EnumerateDirectories(directory))
            {
                Scan(Path.Join(directory, dir));
            }

            foreach(var file in Directory.EnumerateFiles(directory))
            {
                FileInfo fi = new FileInfo(Path.Join(directory, file));
                foreach(var handler in handlers)
                {
                    handler.Handle(fi);
                }
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

    public interface IHandler
    {
        void Handle(FileInfo file);
        string Name { get; }

        string GetStatistics();
    }

    public class NameHandler : IHandler
    {
        private (float avg, int count) averageNameLength = (avg: 0, count: 0);

        public string Name => "Name statistics";

        public string GetStatistics()
        {
            return $"Average name length: {averageNameLength.avg} characters";
        }

        public void Handle(FileInfo file)
        {
            averageNameLength = (avg: (averageNameLength.avg * averageNameLength.count + file.Name.Length) / (averageNameLength.count + 1), count: averageNameLength.count + 1);
        }
    }

    public class ExtensionHandler : IHandler
    {
        private Dictionary<string, int> extensions = new Dictionary<string, int>();

        public string Name => "Extension statistics";

        public string GetStatistics()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string ext in extensions.Keys)
            {
                sb.AppendLine($"{ext}: {extensions[ext]} files");
            }
            return sb.ToString();
        }

        public void Handle(FileInfo file)
        {
            if (!extensions.TryGetValue(file.Extension, out int count)) count = 0;
            extensions[file.Extension] = count + 1;
        }
    }

    public class SizeHandler : IHandler
    {
        private (float avg, int count) averageSize = (avg: 0, count: 0);

        public string Name => "Size statistics";

        public string GetStatistics()
        {
            return $"Average file size: {averageSize.avg} bytes";
        }

        public void Handle(FileInfo file)
        {
            averageSize = (avg: (averageSize.avg * averageSize.count + file.Name.Length) / (averageSize.count + 1), count: averageSize.count + 1);
        }
    }

    public class ModifiedDateHandler : IHandler
    {
        private Dictionary<int, int> numberOfFilesPerYear = new Dictionary<int, int>();

        public string Name => "Modified date statistics";

        public string GetStatistics()
        {
            StringBuilder sb = new StringBuilder();
            foreach(int year in numberOfFilesPerYear.Keys)
            {
                sb.AppendLine($"{year}: {numberOfFilesPerYear[year]} files");
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
