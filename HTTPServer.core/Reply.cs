using System;
using System.Text;

namespace HTTPServer.core
{
    public class Reply
    {
        public byte[] StartingLine { get; set; }
        public byte[] Headers { get; set; }
        public byte[] Body { get; set; }

        public Reply()
        {
            StartingLine = new byte[0];
            Headers = new byte[0];
            Body = new byte[0];
        }

        public byte[] ReplyMessage()
        {
            var startingLine = StartingLine;
            var headers = Headers;
            var newLine = Encoding.UTF8.GetBytes("\r\n");
            var body = Body;

            var startAndHeaders = new byte[startingLine.Length + headers.Length];
            CombineArrays(startingLine, startAndHeaders, headers);

            var startHeadersNewLine = new byte[startAndHeaders.Length + newLine.Length];
            CombineArrays(startAndHeaders, startHeadersNewLine, newLine);

            var wholeMessage = new byte[startHeadersNewLine.Length + body.Length];
            CombineArrays(startHeadersNewLine, wholeMessage, body);
            return wholeMessage;
        }

        private static void CombineArrays(byte[] messageBytes, byte[] combinedMessage, byte[] bodyMessage)
        {
            Buffer.BlockCopy(messageBytes, 0, combinedMessage, 0, messageBytes.Length);
            Buffer.BlockCopy(bodyMessage, 0, combinedMessage, messageBytes.Length, bodyMessage.Length);
        }
    }
}
