namespace HTTPServer.core
{
    public interface IHttpHandler
    {
        byte[] Execute(Request request);
    }
}
