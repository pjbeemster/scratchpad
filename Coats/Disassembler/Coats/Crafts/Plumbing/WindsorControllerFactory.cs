namespace Coats.Crafts.Plumbing
{
    using Castle.MicroKernel;
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(0x194, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }
            Controller controller = this.kernel.Resolve(controllerType) as Controller;
            if (controller != null)
            {
                controller.ActionInvoker = this.kernel.Resolve<IActionInvoker>();
            }
            return controller;
        }

        public override void ReleaseController(IController controller)
        {
            this.kernel.ReleaseComponent(controller);
        }
    }
}

