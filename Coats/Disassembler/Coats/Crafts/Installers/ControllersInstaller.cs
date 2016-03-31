namespace Coats.Crafts.Installers
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using System;
    using System.Web.Mvc;
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(new IRegistration[] { Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient() });
        }
    }
}

