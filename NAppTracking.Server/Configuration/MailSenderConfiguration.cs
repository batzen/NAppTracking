namespace NAppTracking.Server.Configuration
{
    using System.Net;
    using System.Net.Mail;
    using System.Web.Hosting;

    public class MailSenderConfiguration : IMailSenderConfiguration
    {
        public MailSenderConfiguration(IAppConfiguration appConfiguration)
        {
            if (appConfiguration.SmtpUri != null
                && appConfiguration.SmtpUri.IsAbsoluteUri)
            {
                var smtpUri = new SmtpUri(appConfiguration.SmtpUri);

                this.DeliveryMethod = SmtpDeliveryMethod.Network;
                this.Host = smtpUri.Host;
                this.Port = smtpUri.Port;
                this.EnableSsl = smtpUri.IsSecure;

                if (!string.IsNullOrWhiteSpace(smtpUri.UserName))
                {
                    this.UseDefaultCredentials = false;
                    this.Credentials = new NetworkCredential(smtpUri.UserName, smtpUri.Password);
                }
            }
            else
            {
                this.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                this.PickupDirectoryLocation = HostingEnvironment.MapPath("~/App_Data/Mail");
            }
        }

        public SmtpDeliveryMethod DeliveryMethod { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public NetworkCredential Credentials { get; set; }

        public string PickupDirectoryLocation { get; set; }
    }
}