namespace Coats.Crafts.Installers
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using System;

    public class ActionInvokerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(new IRegistration[] { Component.For<IActionInvoker>().ImplementedBy<WindsorActionInvoker>() });
        }
    }
}

