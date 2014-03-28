﻿namespace NAppTracking.Server.Configuration
{
    public interface IAppConfiguration
    {
        /// <summary>
        /// Gets the local directory in which to store files.
        /// </summary>
        string FileStorageDirectory { get; set; }
    }
}