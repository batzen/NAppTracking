namespace NAppTracking.Server.Configuration
{
    public interface IAppConfiguration
    {
        /// <summary>
        /// Gets the local directory in which to store files.
        /// </summary>
        string FileStorageDirectory { get; set; }

        bool IsDemoEnabled { get; set; }

        int DefaultPageSize { get; set; }
    }
}