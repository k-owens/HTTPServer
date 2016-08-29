namespace HTTPServer.core
{
    public interface ICriteria
    {
        bool ShouldRun(Request request);
    }
}
