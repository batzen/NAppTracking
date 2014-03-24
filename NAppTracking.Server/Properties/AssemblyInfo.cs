using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Owin;
using NAppTracking.Server;
using WebActivatorEx;

[assembly: AssemblyTitle("NAppTracking.Server")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("NAppTracking.Server")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Owin config
[assembly: OwinStartup(typeof(Startup))]

// Ninject config
[assembly: PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof(NinjectWebCommon), "Stop")]