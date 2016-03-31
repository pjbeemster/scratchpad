using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Xml;
using System.Text;
using System.Web.Mvc.Html;
using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using Castle.Windsor;

namespace Coats.Crafts.HtmlHelpers
{
    public static class ComponentInfoHelper
    {
        public static IComponent ComponentInfo(this HtmlHelper helper, string tcm)
        {
            IComponent comp = null;

            try
            {
                var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                var factory = accessor.Container.Resolve<IComponentFactory>();
                comp = factory.GetComponent(tcm);
                return comp;

            } catch(Exception ex) {
                return comp;
            }

        }

    }
}