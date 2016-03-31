namespace Coats.Crafts.HtmlHelpers
{
    using Castle.Windsor;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;

    public static class ComponentInfoHelper
    {
        public static IComponent ComponentInfo(this HtmlHelper helper, string tcm)
        {
            IComponent component = null;
            try
            {
                IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                return applicationInstance.Container.Resolve<IComponentFactory>().GetComponent(tcm);
            }
            catch (Exception)
            {
                return component;
            }
        }
    }
}

