namespace Coats.Crafts.Installers
{
    using Castle.Core;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using DD4T.ContentModel.Contracts.Caching;
    using DD4T.ContentModel.Contracts.Providers;
    using DD4T.ContentModel.Factories;
    using DD4T.Factories;
    using DD4T.Factories.Caching;
    using DD4T.Mvc.Html;
    using DD4T.Providers.WCFServices;
    using System;

    public class DD4TInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(new IRegistration[] { Component.For<ICacheAgent>().ImplementedBy<DefaultCacheAgent>().Properties(PropertyFilter.IgnoreAll).LifestyleTransient() });
            container.Register(new IRegistration[] { Component.For<IComponentPresentationRenderer>().ImplementedBy<DefaultComponentPresentationRenderer>() });
            container.Register(new IRegistration[] { Component.For<IPageProvider>().ImplementedBy<TridionPageProvider>() });
            container.Register(new IRegistration[] { Component.For<ILinkProvider>().ImplementedBy<TridionLinkProvider>() });
            container.Register(new IRegistration[] { Component.For<IComponentProvider>().ImplementedBy<TridionComponentProvider>() });
            container.Register(new IRegistration[] { Component.For<IPageFactory>().ImplementedBy<PageFactory>() });
            container.Register(new IRegistration[] { Component.For<IComponentFactory>().ImplementedBy<ComponentFactory>() });
            container.Register(new IRegistration[] { Component.For<ILinkFactory>().ImplementedBy<LinkFactory>() });
        }
    }
}

