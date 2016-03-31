namespace Coats.Crafts.Plumbing
{
    using Castle.Windsor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class WindsorDependencyResolver : IDependencyResolver
    {
        private IWindsorContainer container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return (this.container.Kernel.HasComponent(serviceType) ? this.container.Resolve(serviceType) : null);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return (this.container.Kernel.HasComponent(serviceType) ? this.container.ResolveAll(serviceType).Cast<object>() : ((IEnumerable<object>) new object[0]));
        }
    }
}

