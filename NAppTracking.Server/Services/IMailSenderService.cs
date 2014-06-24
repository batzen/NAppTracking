namespace NAppTracking.Server.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NAppTracking.Server.Entities;

    public interface IMailSenderService
    {
        Task SendAsync(IEnumerable<ApplicationUser> users, string title, string message);

        Task SendAsync(ApplicationUser user, string title, string message);
    }
}