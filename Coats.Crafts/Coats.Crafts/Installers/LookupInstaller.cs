using System;
using System.Configuration;
using System.IO;
using System.Web.Compilation;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Factories;
using DD4T.Factories;
using DD4T.Factories.Caching;
using DD4T.Providers.WCFServices;
using DD4T.Mvc.Html;

using GeoIP;

namespace Coats.Crafts.Installers
{
    public class LookupInstaller : IWindsorInstaller 
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            string data = ConfigurationManager.AppSettings["GeoData"];
            string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            string dataFile = Path.Combine(dataDirectory, data);

            if (!File.Exists(dataFile))
                throw new FieldAccessException(
                    String.Format("The file {0} is not there!", dataFile));

            // Cache
            container.Register(
                Component.For<ILookupService>()
                .ImplementedBy<LookupService>()
                .DependsOn(
                    Dependency.OnValue("databaseFile", dataFile),
                    Dependency.OnValue("options", LookupService.GEOIP_MEMORY_CACHE)
                ));
        }
    }
}