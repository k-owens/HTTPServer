namespace HTTPServer.core
{
    public interface IPathContents
    {
        string DirectoryPath { get; }
        string[] GetFiles(string directoryExtension);
        string[] GetDirectories(string directoryExtension);
        byte[] GetFileContents(string filePath);
        void PostContents(Request request);
    }
}
