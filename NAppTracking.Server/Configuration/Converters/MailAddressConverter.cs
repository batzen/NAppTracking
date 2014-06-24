﻿namespace NAppTracking.Server.Configuration.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Net.Mail;

    public class MailAddressConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;
            return strValue == null ? null : new MailAddress(strValue);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var srcValue = value as MailAddress;
            if (srcValue != null 
                && destinationType == typeof(string))
            {
                return String.Format(CultureInfo.CurrentCulture, "{0} <{1}>", srcValue.DisplayName, srcValue.Address);
            }

            return null;
        }
    }
}