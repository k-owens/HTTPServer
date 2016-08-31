using System.Text;
using HTTPServer.core;

namespace HTTPServer.test
{
    public class MockPathContents: IPathContents
    {
        public string DirectoryPath { get; }

        public MockPathContents(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public string[] GetFiles(string directoryExtension)
        {
            return new []{ "C:\\gitwork\\HTTP Server\\file.txt" };
        }

        public string[] GetDirectories(string directoryExtension)
        {
            return new []{ "C:\\gitwork\\HTTP Server\\.git" };
        }

        public byte[] GetFileContents(string filePath)
        {
            return Encoding.UTF8.GetBytes("This is the content of the file.");
        }

        public void PostContents(Request request)
        {
            
        }
    }
}
