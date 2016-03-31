namespace Coats.Crafts.Installers
{
    using Castle.Components.DictionaryAdapter;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using System;
    using System.Configuration;

    public class ConfigurationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(new IRegistration[] { Component.For<IAppSettings>().UsingFactoryMethod<IAppSettings>((Func<IAppSettings>) (() => new DictionaryAdapterFactory().GetAdapter<IAppSettings>(ConfigurationManager.AppSettings)), false) });
            WebConfiguration.InitSettings(container.Resolve<IAppSettings>());
        }
    }
}

