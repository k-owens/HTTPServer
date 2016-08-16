using System.Text;
using HTTPServer.core;

namespace HTTPServer.test
{
    public class MockFileContents : IFileContents
    {
        public byte[] GetFileContents(string filePath)
        {
            return Encoding.UTF8.GetBytes("This is the content of the file.");
        }
    }
}
