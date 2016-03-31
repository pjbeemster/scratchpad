namespace Coats.Crafts.Installers
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using System;

    public class ResourceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(new IRegistration[] { Component.For<IDynamcResourceProviderFactory>().AsFactory<IDynamcResourceProviderFactory>() });
            container.Register(new IRegistration[] { Component.For<IResourceProvider>().ImplementedBy<DynamicResourceProvider>().LifestyleTransient() });
            container.Register(new IRegistration[] { Component.For<IResourceDocument>().ImplementedBy<ResourceDocument>().Named("labels").DependsOn(new { resourcePath = "/labels/resources.resx" }).LifestylePerWebRequest() });
            container.Register(new IRegistration[] { Component.For<IResourceDocument>().ImplementedBy<ResourceDocument>().Named("seolabels").DependsOn(new { resourcePath = "/labels/seo_resources.resx" }).LifestylePerWebRequest() });
        }
    }
}

