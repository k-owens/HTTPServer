using System.IO;

namespace HTTPServer.core
{
    public class ConcretePathContents: IPathContents
    {
        public string DirectoryPath { get; }

        public ConcretePathContents(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public string[] GetFiles(string directoryExtension)
        {
            return Directory.GetFiles(DirectoryPath + directoryExtension);
        }

        public string[] GetDirectories(string directoryExtension)
        {
            return Directory.GetDirectories(DirectoryPath + directoryExtension);
        }

        public byte[] GetFileContents(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        public void PostContents(Request request)
        {
            System.IO.File.WriteAllBytes(DirectoryPath + "\\" + request.Uri.Substring(1), request.Body);
        }
    }
}
