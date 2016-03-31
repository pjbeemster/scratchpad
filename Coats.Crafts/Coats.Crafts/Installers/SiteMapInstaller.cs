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

using Coats.Crafts.Sitemap;

namespace Coats.Crafts.Installers
{
    public class SiteMapInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {          
            container.Register(
                Component.For<ISiteMap>()
                .ImplementedBy<TridionSiteMap>()
                );
        }
    }
}