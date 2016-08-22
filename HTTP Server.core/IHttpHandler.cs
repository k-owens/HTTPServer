namespace HTTPServer.core
{
    public interface IHttpHandler
    {
        byte[] Execute(Request request);
        bool ShouldRun(Request request, IPathContents pathContents);
    }
}
