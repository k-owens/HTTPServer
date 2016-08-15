namespace HTTPServer.core
{
    public interface IDirectoryContents
    {
        string DirectoryPath { get; }
        string[] GetFiles();
        string[] GetDirectories();
    }
}
