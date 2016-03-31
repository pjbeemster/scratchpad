namespace Coats.Crafts.Plumbing
{
    using Castle.MicroKernel;
    using Coats.Crafts.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class WindsorActionInvoker : ControllerActionInvoker
    {
        private readonly IKernel kernel;

        public WindsorActionInvoker(IKernel kernel)
        {
            this.kernel = kernel;
        }

        protected override ActionExecutedContext InvokeActionMethodWithFilters(ControllerContext controllerContext, IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
        {
            foreach (IActionFilter filter in filters)
            {
                this.kernel.InjectProperties(filter);
            }
            return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
        }
    }
}

