namespace NAppTracking.Server.Configuration
{
    using System.ComponentModel;

    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
        }

        /// <summary>
        /// Gets the local directory in which to store files.
        /// </summary>
        [DefaultValue("~/App_Data/Files")]
        public string FileStorageDirectory { get; set; }

        public bool IsDemoEnabled { get; set; }

        [DefaultValue(25)]
        public int DefaultPageSize { get; set; }
    }
}