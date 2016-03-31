using System;
using System.Web.Compilation;
using System.Web.Mvc;

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
using Coats.Crafts.Plumbing;

namespace Coats.Crafts.Installers
{
    public class ActionInvokerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Action filters
            container.Register(
                Component.For<IActionInvoker>()
                .ImplementedBy<WindsorActionInvoker>()
                );
        }
    }
}