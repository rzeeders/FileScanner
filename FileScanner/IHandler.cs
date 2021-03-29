using System.IO;

namespace FileScanner
{
    public interface IHandler
    {
        void Handle(FileInfo file);
        string Name { get; }

        string GetStatistics();
    }




}
