namespace NAppTracking.Server.Services
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileStorageService
    {
        Task DeleteFileAsync(string fileName);

        Task<bool> FileExistsAsync(string fileName);

        Task<Stream> GetFileAsync(string fileName);

        Task SaveFileAsync(string fileName, Stream filestream);

        Task MoveFileAsync(string source, string destination);

        Task CopyFileAsync(string source, string destination);
    }
}