using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Castle.Windsor;

using Coats.Crafts.Extensions;
using Castle.MicroKernel;

namespace Coats.Crafts.Plumbing
{
    public class WindsorActionInvoker : ControllerActionInvoker
    {
        private readonly IKernel kernel;

        public WindsorActionInvoker(IKernel kernel)
        {
            this.kernel = kernel;
        }

        protected override ActionExecutedContext InvokeActionMethodWithFilters(
                ControllerContext controllerContext,
                IList<IActionFilter> filters,
                ActionDescriptor actionDescriptor,
                IDictionary<string, object> parameters)
        {
            foreach (IActionFilter actionFilter in filters)
            {
                kernel.InjectProperties(actionFilter);
            }
            return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
        }
    }
}