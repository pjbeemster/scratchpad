namespace Coats.Crafts.Installers
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Sitemap;
    using System;

    public class SiteMapInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(new IRegistration[] { Component.For<ISiteMap>().ImplementedBy<TridionSiteMap>() });
        }
    }
}

