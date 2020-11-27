namespace BramrApi.Service.Interfaces
{
    public interface IServerBlockWriterService
    {
        string LoadFile(string fileName);
        string CreateServerBlock(string url, string alias);
    }
}
