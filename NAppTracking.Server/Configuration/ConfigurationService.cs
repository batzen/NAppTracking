namespace NAppTracking.Server.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Web.Configuration;

    public class ConfigurationService
    {
        private const string SettingPrefix = "Gallery.";

        private IAppConfiguration current;

        public virtual IAppConfiguration Current
        {
            get { return this.current ?? (this.current = this.ResolveSettings()); }
            set { this.current = value; }
        }

        public virtual IAppConfiguration ResolveSettings()
        {
            return this.ResolveConfigObject(new AppConfiguration(), SettingPrefix);
        }

        public virtual T ResolveConfigObject<T>(T instance, string prefix)
        {
            // Iterate over the properties
            foreach (var property in GetConfigProperties(instance))
            {
                // Try to get a config setting value
                var baseName = string.IsNullOrEmpty(property.DisplayName) 
                    ? property.Name 
                    : property.DisplayName;

                var settingName = prefix + baseName;

                var value = this.ReadSetting(settingName);

                if (string.IsNullOrEmpty(value))
                {
                    var defaultValue = property.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();

                    if (defaultValue != null 
                        && defaultValue.Value != null)
                    {
                        if (defaultValue.Value.GetType() == property.PropertyType)
                        {
                            property.SetValue(instance, defaultValue.Value);
                            continue;
                        }
                        
                        value = defaultValue.Value as string;
                    }
                }

                if (!string.IsNullOrEmpty(value))
                {
                    if (property.PropertyType.IsAssignableFrom(typeof(string)))
                    {
                        property.SetValue(instance, value);
                    }
                    else if (property.Converter != null 
                        && property.Converter.CanConvertFrom(typeof(string)))
                    {
                        // Convert the value
                        property.SetValue(instance, property.Converter.ConvertFromString(value));
                    }
                }
                else if (property.Attributes.OfType<RequiredAttribute>().Any())
                {
                    throw new ConfigurationErrorsException(String.Format(CultureInfo.InvariantCulture, "Missing required configuration setting: '{0}'", settingName));
                }
            }

            return instance;
        }

        internal static IEnumerable<PropertyDescriptor> GetConfigProperties<T>(T instance)
        {
            return TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>()
                .Where(p => !p.IsReadOnly);
        }

        public virtual string ReadSetting(string settingName)
        {
            var cstr = this.GetConnectionString(settingName);

            return cstr != null 
                ? cstr.ConnectionString 
                : this.GetAppSetting(settingName);
        }

        public virtual string GetAppSetting(string settingName)
        {
            return WebConfigurationManager.AppSettings[settingName];
        }

        public virtual ConnectionStringSettings GetConnectionString(string settingName)
        {
            return WebConfigurationManager.ConnectionStrings[settingName];
        }
    }
}