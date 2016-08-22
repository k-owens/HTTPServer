namespace HTTPServer.core
{
    public interface IPathContents
    {
        string DirectoryPath { get; }
        string[] GetFiles();
        string[] GetDirectories();
        byte[] GetFileContents(string filePath);
        void PostContents(Request request);
    }
}
