using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Castle.Windsor;

namespace Coats.Crafts.Plumbing
{
    public class WindsorDependencyResolver : IDependencyResolver
    {
        IWindsorContainer container;
        public WindsorDependencyResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        #region IDependencyResolver Members

        public object GetService(Type serviceType)
        {
            return container.Kernel.HasComponent(serviceType) ? container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.Kernel.HasComponent(serviceType) ? container.ResolveAll(serviceType).Cast<object>() : new object[] { };
        }

        #endregion
    }
}
