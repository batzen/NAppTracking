namespace NAppTracking.Server.Configuration
{
    using System.ComponentModel;

    public class AppConfiguration : IAppConfiguration
    {
        /// <summary>
        /// Gets the local directory in which to store files.
        /// </summary>
        [DefaultValue("~/App_Data/Files")]
        public string FileStorageDirectory { get; set; }

        [DefaultValue(true)]
        public bool IsDemoEnabled { get; set; }
    }
}