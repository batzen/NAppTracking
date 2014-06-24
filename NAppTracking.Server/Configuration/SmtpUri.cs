namespace NAppTracking.Server.Configuration
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    public class SmtpUri
    {
        private static readonly Regex UserInfoParser = new Regex("^(?<username>[^:]*):(?<password>.*)$");

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public bool IsSecure { get; private set; }

        public SmtpUri(Uri uri)
        {
            this.IsSecure = uri.Scheme.Equals("smtps", StringComparison.OrdinalIgnoreCase);

            if (!this.IsSecure 
                && !uri.Scheme.Equals("smtp", StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException("Invalid SMTP URL: " + uri);
            }

            var m = UserInfoParser.Match(uri.UserInfo);

            if (m.Success)
            {
                this.UserName = WebUtility.UrlDecode(m.Groups["username"].Value);
                this.Password = WebUtility.UrlDecode(m.Groups["password"].Value);
            }
            else
            {
                this.UserName = WebUtility.UrlDecode(uri.UserInfo);
            }

            this.Host = uri.Host;
            this.Port = uri.IsDefaultPort 
                ? 25 
                : uri.Port;
        }
    }
}