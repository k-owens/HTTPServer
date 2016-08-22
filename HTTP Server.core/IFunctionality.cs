namespace HTTPServer.core
{
    public interface IFunctionality
    {
        byte[] Execute(Request request);
        bool ShouldRun(Request request, IPathContents pathContents);
    }
}
