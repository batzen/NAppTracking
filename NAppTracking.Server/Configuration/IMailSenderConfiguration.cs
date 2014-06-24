namespace NAppTracking.Server.Configuration
{
    using System.Net;
    using System.Net.Mail;

    public interface IMailSenderConfiguration
    {
        SmtpDeliveryMethod DeliveryMethod { get; set; }

        string Host { get; set; }

        int Port { get; set; }

        bool EnableSsl { get; set; }

        bool UseDefaultCredentials { get; set; }

        NetworkCredential Credentials { get; set; }

        string PickupDirectoryLocation { get; set; }
    }
}
