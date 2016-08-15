using System.IO;

namespace HTTPServer.core
{
    public class ConcreteDirectoryContents: IDirectoryContents
    {
        public string DirectoryPath { get; }

        public ConcreteDirectoryContents(string directoryPath)
        {
            this.DirectoryPath = directoryPath;
        }

        public string[] GetFiles()
        {
            return Directory.GetFiles(DirectoryPath);
        }

        public string[] GetDirectories()
        {
            return Directory.GetDirectories(DirectoryPath);
        }
    }
}
