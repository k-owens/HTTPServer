 namespace HTTPServer.core
{
    public class Request
    {
        public string EntireMessage { get; }
        public string HttpVersion { get; }
        public  string Uri { get; }
        public  string Method { get; }
        public string Body { get; }

        public Request(string requestMessage)
        {
            EntireMessage = requestMessage;
            if (requestMessage.Equals(""))
            {
                Method = "";
                Uri = "";
                HttpVersion = "";
                Body = "";
            }
            else
            {
                var endIndexOfFirstLine = requestMessage.IndexOf('\n');
                var firstLine = requestMessage.Substring(0, endIndexOfFirstLine + 1);
                var requestLine = firstLine.Split(' ', ' ');
                Method = requestLine[0];
                Uri = requestLine[1];
                HttpVersion = requestLine[2];
                int index = requestMessage.IndexOf("\r\n\r\n");
                if (index == -1)
                    Body = "";
                else
                {
                    string testBody = requestMessage.Substring(index + 4);
                    Body = testBody;
                }
            }
        }
    }
}
