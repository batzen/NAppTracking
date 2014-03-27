namespace NAppTracking.Server.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web.Hosting;
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

        public Task DeleteFileAsync(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException("folderName");
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var path = BuildPath(this.configuration.FileStorageDirectory, folderName, fileName);
            if (this.fileSystemService.FileExists(path))
            {
                this.fileSystemService.DeleteFile(path);
            }

            return Task.FromResult(0);
        }

        public Task<bool> FileExistsAsync(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException("folderName");
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var path = BuildPath(this.configuration.FileStorageDirectory, folderName, fileName);
            var fileExists = this.fileSystemService.FileExists(path);

            return Task.FromResult(fileExists);
        }

        public Task<Stream> GetFileAsync(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException("folderName");
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var path = BuildPath(this.configuration.FileStorageDirectory, folderName, fileName);

            var fileStream = this.fileSystemService.FileExists(path) ? this.fileSystemService.OpenRead(path) : null;
            return Task.FromResult(fileStream);
        }

        public Task SaveFileAsync(string folderName, string fileName, Stream filestream)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (filestream == null)
            {
                throw new ArgumentNullException("filestream");
            }

            var storageDirectory = ResolvePath(this.configuration.FileStorageDirectory);

            if (!this.fileSystemService.DirectoryExists(storageDirectory))
            {
                this.fileSystemService.CreateDirectory(storageDirectory);
            }

            var folderPath = Path.Combine(storageDirectory, folderName);
            if (!this.fileSystemService.DirectoryExists(folderPath))
            {
                this.fileSystemService.CreateDirectory(folderPath);
            }

            var filePath = BuildPath(this.configuration.FileStorageDirectory, folderName, fileName);
            using (var file = this.fileSystemService.OpenWrite(filePath))
            {
                return filestream.CopyToAsync(file);
            }
        }

        public async Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            await this.CopyFileAsync(sourcePath, destinationPath);

            File.Delete(sourcePath);
        }

        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }

        private static string BuildPath(string fileStorageDirectory, string folderName, string fileName)
        {
            // Resolve the file storage directory
            fileStorageDirectory = ResolvePath(fileStorageDirectory);

            return Path.Combine(fileStorageDirectory, folderName, fileName);
        }

        private static string ResolvePath(string fileStorageDirectory)
        {
            if (fileStorageDirectory.StartsWith("~/", StringComparison.OrdinalIgnoreCase) 
                && HostingEnvironment.IsHosted)
            {
                fileStorageDirectory = HostingEnvironment.MapPath(fileStorageDirectory);
            }

            return fileStorageDirectory;
        }
    }
}