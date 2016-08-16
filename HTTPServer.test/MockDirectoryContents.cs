using HTTPServer.core;

namespace HTTPServer.test
{
    public class MockDirectoryContents: IDirectoryContents
    {
        public string DirectoryPath { get; }

        public MockDirectoryContents(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }
        public string[] GetFiles()
        {
            return new []{ "C:\\gitwork\\HTTP Server\\.gitattributes" };
        }

        public string[] GetDirectories()
        {
            return new []{ "C:\\gitwork\\HTTP Server\\.git" };
        }
    }
}
