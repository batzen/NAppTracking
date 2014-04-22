namespace NAppTracking.Server.Services
{
    using System;
    using System.IO;
    using System.Web.Hosting;
    using NAppTracking.Server.Configuration;

    public class FileSystemService : IFileSystemService
    {
        private readonly IAppConfiguration configuration;

        public FileSystemService(IAppConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public string BuildPath(string folderOrFileName)
        {
            return this.BuildPath(this.configuration.FileStorageDirectory, folderOrFileName);
        }

        public string BuildPath(string fileStorageDirectory, string folderOrFileName)
        {
            // Resolve the file storage directory
            fileStorageDirectory = ResolvePath(fileStorageDirectory);

            return Path.Combine(fileStorageDirectory, folderOrFileName);
        }

        public string ResolvePath(string fileStorageDirectory)
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