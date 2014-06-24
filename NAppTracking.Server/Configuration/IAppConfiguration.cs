namespace NAppTracking.Server.Configuration
{
    using System;
    using System.Net.Mail;

    public interface IAppConfiguration
    {
        /// <summary>
        /// Gets the local directory in which to store files.
        /// </summary>
        string FileStorageDirectory { get; set; }

        bool IsDemoEnabled { get; set; }

        int DefaultPageSize { get; set; }

        MailAddress Owner { get; set; }

        Uri SmtpUri { get; set; }
    }
}