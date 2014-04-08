namespace NAppTracking.Server.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using NAppTracking.Server.Configuration;

    public class FileSystemFileStorageService : IFileStorageService
    {
        private readonly IAppConfiguration configuration;
        private readonly IFileSystemService fileSystemService;

        public FileSystemFileStorageService(IAppConfiguration configuration, IFileSystemService fileSystemService)
        {
            this.configuration = configuration;
            this.fileSystemService = fileSystemService;
        }

        public Task DeleteFileAsync(string fileName)
        {
            EnsureFileName(fileName);

            var path = this.fileSystemService.BuildPath(fileName);
            if (this.fileSystemService.FileExists(path))
            {
                this.fileSystemService.DeleteFile(path);
            }

            return Task.FromResult(0);
        }

        public Task<bool> FileExistsAsync(string fileName)
        {
            EnsureFileName(fileName);

            var path = this.fileSystemService.BuildPath(fileName);
            var fileExists = this.fileSystemService.FileExists(path);

            return Task.FromResult(fileExists);
        }

        public Task<Stream> GetFileAsync(string fileName)
        {
            EnsureFileName(fileName);

            var path = this.fileSystemService.BuildPath(fileName);

            var fileStream = this.fileSystemService.FileExists(path) ? this.fileSystemService.OpenRead(path) : null;
            return Task.FromResult(fileStream);
        }

        public Task SaveFileAsync(string fileName, Stream filestream)
        {
            EnsureFileName(fileName);

            if (filestream == null)
            {
                throw new ArgumentNullException("filestream");
            }

            var storageDirectory = this.fileSystemService.ResolvePath(this.configuration.FileStorageDirectory);

            if (!this.fileSystemService.DirectoryExists(storageDirectory))
            {
                this.fileSystemService.CreateDirectory(this.configuration.FileStorageDirectory);
            }

            var directoryName = Path.GetDirectoryName(fileName);

            if (string.IsNullOrEmpty(directoryName) == false)
            {
                var folderPath = Path.Combine(storageDirectory, directoryName);
                if (!this.fileSystemService.DirectoryExists(folderPath))
                {
                    this.fileSystemService.CreateDirectory(folderPath);
                }
            }

            using (var file = this.fileSystemService.OpenWrite(fileName))
            {
                return filestream.CopyToAsync(file);
            }
        }

        public async Task MoveFileAsync(string source, string destination)
        {
            await this.CopyFileAsync(source, destination);

            await this.DeleteFileAsync(source);
        }

        public async Task CopyFileAsync(string source, string destination)
        {
            using (Stream sourceStream = File.Open(source, FileMode.Open))
            {
                using (Stream destinationStream = File.Create(destination))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }

        private static void EnsureFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
        }
    }
}