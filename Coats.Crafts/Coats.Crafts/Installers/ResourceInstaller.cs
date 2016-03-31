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

using Castle.Facilities.TypedFactory;

using Coats.Crafts.Resources;

namespace Coats.Crafts.Installers
{
    public class ResourceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IDynamcResourceProviderFactory>()
                        .AsFactory()
            );

            // Resource provider
            container.Register(
                Component.For<IResourceProvider>()
                .ImplementedBy<DynamicResourceProvider>()
                .LifestyleTransient() // MUST be LifeStyleTransient, otherwise all cached resx files will point to the same instance instead of unique.
                //.DependsOn(
                //    Dependency.OnComponent<IPageProvider, TridionPageProvider>(),
                //    Dependency.OnComponent<ICacheAgent, DefaultCacheAgent>())
                 );


            // This allows yuo to resolve a dependency on the same service - but means the paths have to be included.
            container.Register(
                Component.For<Coats.Crafts.Resources.IResourceDocument>()
                .ImplementedBy<Coats.Crafts.Resources.ResourceDocument>()
                .Named("labels")
                .DependsOn(new { resourcePath ="/labels/resources.resx" })
                .LifestylePerWebRequest()
                 );

            container.Register(
                Component.For<Coats.Crafts.Resources.IResourceDocument>()
                .ImplementedBy<Coats.Crafts.Resources.ResourceDocument>()
                .Named("seolabels")
                .DependsOn(new { resourcePath = "/labels/seo_resources.resx" })
                .LifestylePerWebRequest()
                 );
        }
    }
}