namespace NAppTracking.Server.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using NAppTracking.Server.Configuration;
    using NAppTracking.Server.Entities;
    using WebGrease.Css.Extensions;

    public class MailSenderService : IMailSenderService
    {
        private readonly IAppConfiguration appConfiguration;
        private readonly IMailSenderConfiguration mailSenderConfiguration;                
        private readonly IFileSystemService fileSystemService;

        public MailSenderService(IAppConfiguration appConfiguration, IMailSenderConfiguration mailSenderConfiguration, IFileSystemService fileSystemService)
        {
            this.appConfiguration = appConfiguration;
            this.mailSenderConfiguration = mailSenderConfiguration;
            this.fileSystemService = fileSystemService;
        }

        public async Task SendAsync(IEnumerable<ApplicationUser> users, string title, string message)
        {
            foreach (var user in users)
            {
                await this.SendAsync(user, title, message);
            }
        }

        public async Task SendAsync(ApplicationUser user, string title, string message)
        {
            if (user.EmailConfirmed == false
                || string.IsNullOrEmpty(user.Email))
            {
                return;
            }

            var smtpClient = CreateNewSmtpClient();

            var mailMessage = new MailMessage(this.appConfiguration.Owner, new MailAddress(user.Email, user.UserName)) 
                {
                    Subject = title, 
                    Body = message
                };

            await smtpClient.SendMailAsync(mailMessage);
        }

        private SmtpClient CreateNewSmtpClient()
        {
            if (this.mailSenderConfiguration.Host == null)
            {
                var smtpClient = new SmtpClient
                {
                    DeliveryMethod = this.mailSenderConfiguration.DeliveryMethod,
                    PickupDirectoryLocation = this.mailSenderConfiguration.PickupDirectoryLocation
                };

                if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory
                    && !this.fileSystemService.DirectoryExists(smtpClient.PickupDirectoryLocation))
                {
                    this.fileSystemService.CreateDirectory(smtpClient.PickupDirectoryLocation);
                }

                return smtpClient;
            }
            else
            {
                var smtpClient = new SmtpClient
                {
                    DeliveryMethod = this.mailSenderConfiguration.DeliveryMethod,
                    Credentials = this.mailSenderConfiguration.Credentials,
                    EnableSsl = this.mailSenderConfiguration.EnableSsl,
                    Host = this.mailSenderConfiguration.Host,
                    Port = this.mailSenderConfiguration.Port,
                    UseDefaultCredentials = this.mailSenderConfiguration.UseDefaultCredentials
                };
                return smtpClient;
            }
        }
    }
}