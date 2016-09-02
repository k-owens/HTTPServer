namespace HTTPServer.core
{
    public interface IHttpHandler
    {
        Reply Execute(Request request);
    }
}
