namespace NAppTracking.Server.Services
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileStorageService
    {
        Task DeleteFileAsync(string folderName, string fileName);

        Task<bool> FileExistsAsync(string folderName, string fileName);

        Task<Stream> GetFileAsync(string folderName, string fileName);

        Task SaveFileAsync(string folderName, string fileName, Stream filestream);

        Task MoveFileAsync(string sourcePath, string destinationPath);

        Task CopyFileAsync(string sourcePath, string destinationPath);
    }
}
