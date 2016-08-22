using System.IO;

namespace HTTPServer.core
{
    public class ConcretePathContents: IPathContents
    {
        public string DirectoryPath { get; }

        public ConcretePathContents(string directoryPath)
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
        public byte[] GetFileContents(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        public void PostContents(Request request)
        {
            System.IO.File.AppendAllText(DirectoryPath + "\\" + request.Uri.Substring(1), request.Body);
        }
    }
}
