using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer.core
{
    public class FileContents : IFileContents
    {
        public byte[] GetFileContents(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }
    }
}
