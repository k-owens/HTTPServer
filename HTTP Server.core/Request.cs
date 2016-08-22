 using System.Text;

namespace HTTPServer.core
{
    public class Request
    {
        public string HttpVersion { get; }
        public  string Uri { get; }
        public  string Method { get; }
        public byte[] Body { get; }

        public Request(byte[] requestMessage)
        {
            if (requestMessage.Equals(""))
            {
                Method = "";
                Uri = "";
                HttpVersion = "";
                Body = new byte[0];
            }
            else
            {
                var endIndexOfFirstLine = Encoding.UTF8.GetString(requestMessage).IndexOf('\n');
                var firstLine = Encoding.UTF8.GetString(requestMessage).Substring(0, endIndexOfFirstLine + 1);
                var requestLine = firstLine.Split(' ', ' ');
                Method = requestLine[0];
                Uri = requestLine[1];
                HttpVersion = requestLine[2];
                var index = Encoding.UTF8.GetString(requestMessage).IndexOf("\r\n\r\n");
                if (index == -1)
                    Body = new byte[0];
                else
                {
                    Body = GetBodyOfMessage(index, requestMessage);
                }
            }
        }

        private byte[] GetBodyOfMessage(int startIndex, byte[] message)
        {
            var body = new byte[message.Length - startIndex];
            for (var index = startIndex; index < message.Length-1; index++)
            {
                body[index-startIndex] = message[index];
            }

            return body;
        }
    }
}
