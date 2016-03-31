using System;
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

using Coats.Crafts.Resources;

namespace Coats.Crafts.Installers
{
    public class DD4TInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Cache - ignoring public properties to stop Windsor trying to set them!
            container.Register(
                Component.For<ICacheAgent>()
                .ImplementedBy<DefaultCacheAgent>()
                .Properties(Castle.Core.PropertyFilter.IgnoreAll)
                .LifestyleTransient()
                );   
            
            // ComponentPresentationRenderer
            container.Register(
                Component.For<IComponentPresentationRenderer>()
                .ImplementedBy<DefaultComponentPresentationRenderer>()
                );

            // PageProvider
            container.Register(
                Component.For<IPageProvider>()
                .ImplementedBy<TridionPageProvider>()
            );
                //.DependsOn(Dependency.OnAppSettingsValue("PublicationId", "DD4T.PublicationId"))
            //);

            // LinkProvider
            container.Register(
                Component.For<ILinkProvider>()
                .ImplementedBy<TridionLinkProvider>()
            );

            // ComponentProvider
            container.Register(
                Component.For<IComponentProvider>()
                .ImplementedBy<TridionComponentProvider>()
            );

            // PageFactory
            container.Register(
                Component.For<IPageFactory>()
                .ImplementedBy<PageFactory>()
                //.DependsOn(
                //    Dependency.OnComponent<IPageProvider, TridionPageProvider>(),
                //    Dependency.OnComponent<IComponentFactory, ComponentFactory>(),
                //    Dependency.OnComponent<ILinkFactory, LinkFactory>()
                //)
            );

            // ComponentFactory
            container.Register(
                Component.For<IComponentFactory>()
                .ImplementedBy<ComponentFactory>()
                //.DependsOn(
                //    Dependency.OnComponent<IComponentProvider, TridionComponentProvider>()
                //)
            );

            // LinkFactory
            container.Register(
                Component.For<ILinkFactory>()
                .ImplementedBy<LinkFactory>()
                //.DependsOn(
                //    Dependency.OnComponent<ILinkProvider, TridionLinkProvider>()
                //)
            );
        }
    }
}