namespace NAppTracking.Server.Services
{
    using System.IO;

    public interface IFileSystemService
    {
        void CreateDirectory(string path);

        bool DirectoryExists(string path);

        void DeleteDirectory(string path, bool recursive);

        bool FileExists(string path);

        void DeleteFile(string path);

        Stream OpenRead(string path);

        Stream OpenWrite(string path);

        string BuildPath(string folderOrFileName);

        string ResolvePath(string path);
    }
}