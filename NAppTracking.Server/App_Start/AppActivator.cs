namespace NAppTracking.Server
{
    using System.Security.Claims;
    using System.Web.Helpers;
    using AutoMapper;
    using NAppTracking.Dto;
    using NAppTracking.Server.Configuration;
    using NAppTracking.Server.Entities;
    using NAppTracking.Server.Helpers;
    using Ninject;

    public class AppActivator
    {
        public static void PreStart()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            NinjectWebCommon.Start();

            Mapper.CreateMap<ExceptionReportDto, ExceptionReport>();
            Mapper.CreateMap<ExceptionReportCustomDataSetDto, ExceptionReportCustomDataSet>();
            Mapper.CreateMap<ExceptionReportCustomDataDto, ExceptionReportCustomData>();
        }

        public static void PostStart()
        {
            if (NinjectWebCommon.Kernel.Get<IAppConfiguration>().IsDemoEnabled)
            {
                DemoDatabaseHelper.CreateDemoDatabase();
            }
        }

        public static void Stop()
        {
            NinjectWebCommon.Stop();
        }
    }
}