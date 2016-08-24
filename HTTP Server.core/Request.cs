 using System;
 using System.Text;

namespace HTTPServer.core
{
    public class Request
    {
        public string HttpVersion { get; }
        public  string Uri { get; }
        public  string Method { get; }
        public string[] Headers { get; }
        public byte[] Body { get; }

        public Request(byte[] requestMessage)
        {
            if (requestMessage.Length == 0)
            {
                Method = "";
                Uri = "";
                HttpVersion = "";
                Body = new byte[0];
                Headers = new string[0];
            }
            else
            {
                string[] messageLines = Encoding.UTF8.GetString(requestMessage).Split('\n');
                var firstLine = messageLines[0].Substring(0,messageLines[0].Length-1);
                Headers = DivideHeaders(messageLines);
                var requestLine = firstLine.Split(' ', ' ');
                Method = requestLine[0];
                Uri = requestLine[1];
                HttpVersion = requestLine[2];
                var index = Encoding.UTF8.GetString(requestMessage).IndexOf("\r\n\r\n");
                if (index == -1)
                    Body = new byte[0];
                else
                    Body = GetBodyOfMessage(index+4, requestMessage);
            }
        }

        private byte[] GetBodyOfMessage(int startIndex, byte[] message)
        {
            var body = new byte[message.Length - startIndex];
            for (var index = startIndex; index < message.Length; index++)
            {
                body[index-startIndex] = message[index];
            }
            return body;
        }

        private string[] DivideHeaders(string[] allLines)
        {
            if(allLines.Length < 3)
                return new string[0];

            string[] headers = new string[allLines.Length-3];
            for (int i = 0; i < allLines.Length - 3; i++)
            {
                if(allLines[i+1].Length > 0)
                    headers[i] = allLines[i + 1].Substring(0, allLines[i+1].Length-1);
            }
            return headers;
        }
    }
}
