using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Castle.Windsor;

namespace Coats.Crafts.Plumbing
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Castle.MicroKernel;

    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            var controller = kernel.Resolve(controllerType) as Controller;

            // Resolve new action invoker
            if (controller != null)
            {
                controller.ActionInvoker = kernel.Resolve<IActionInvoker>();
            }

            return controller;
        }
    }
}
