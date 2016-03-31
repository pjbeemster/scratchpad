using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

using Castle.Components.DictionaryAdapter;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Coats.Crafts.Configuration;

namespace Coats.Crafts.Installers
{
    public class ConfigurationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAppSettings>().UsingFactoryMethod(
                    () => new DictionaryAdapterFactory()
                         .GetAdapter<IAppSettings>(ConfigurationManager.AppSettings)));

            WebConfiguration.InitSettings(container.Resolve<IAppSettings>());
        }
    }
}
