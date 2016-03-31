namespace Coats.Crafts.Resources
{
    using Castle.Windsor;
    using Configuration;
    using DD4T.Utils;
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Web;
    using System.Web.Compilation;
    using System.Xml.XPath;

    public class DynamicResourceProvider : IResourceProvider
    {
        private string _resourceName;
        private object lock1 = new object();
        public static readonly string ResourcePath = WebConfiguration.Current.ResourcePath;

        public DynamicResourceProvider(string resourceName)
        {
            this._resourceName = resourceName;
        }

        public object GetObject(string resourceKey, CultureInfo culture)
        {
            string str = string.Format("[{0}] not found", resourceKey);
            LoggerService.Debug(">>DynamicResourceProvider({0})", new object[] { resourceKey });
            string xpath = string.Format("/root/data[@name='{0}']/value", resourceKey);
            XPathNavigator navigator = this.ResourceDocument.SelectSingleNode(xpath);
            if (navigator != null)
            {
                str = navigator.Value;
            }
            LoggerService.Debug("<<DynamicResourceProvider({0})", new object[] { resourceKey });
            return str;
        }

        protected virtual string GetPathToResource(string resourceName)
        {
            return string.Format(ResourcePath, resourceName);
        }

        public XPathNavigator ResourceDocument
        {
            get
            {
                IResourceDocument document;
                string pathToResource = this.GetPathToResource(this._resourceName);
                LoggerService.Debug(">>DynamicResourceProvider load document ({0})", new object[] { pathToResource });
                IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                if (this._resourceName.Contains("seo_resources"))
                {
                    document = applicationInstance.Container.Resolve<IResourceDocument>("seolabels");
                }
                else
                {
                    document = applicationInstance.Container.Resolve<IResourceDocument>("labels");
                }
                LoggerService.Information(string.Format("ResourceDocument > Tracking? {0}", applicationInstance.Container.Kernel.ReleasePolicy.HasTrack(document)), new object[0]);
                LoggerService.Information(string.Format("ResourceDocument > doc: {0}", document.GetHashCode()), new object[0]);
                return document.doc;
            }
        }

        public IResourceReader ResourceReader
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

