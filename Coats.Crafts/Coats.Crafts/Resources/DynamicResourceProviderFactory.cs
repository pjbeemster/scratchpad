using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Xml;
using System.Xml.XPath;

using Castle.Windsor;
using Castle.MicroKernel;

using DD4T.Factories;
using DD4T.ContentModel.Factories;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.Utils;
using DD4T.Factories.Caching;

using Coats.Crafts.Configuration;
using System.Diagnostics;

namespace Coats.Crafts.Resources
{
    public interface IDynamcResourceProviderFactory: IDisposable
    {
        IResourceProvider Create(string resourceName);
        void Release(IResourceProvider provider);
    }

    public static class Helper
    {
        public static string GetResource(string key)
        {
            return HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, key) as string;
        }

        public static string GetResource(string bundle, string key)
        {
            return HttpContext.GetGlobalResourceObject(bundle, key) as string;
        }
    }

    public class DynamicResourceProviderFactory : ResourceProviderFactory
    {
        protected virtual IResourceProvider GetResourceProvider(string resourceName)
        {
            IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;            
            
            //return new DynamicResourceProvider(resourceName);
            IResourceProvider provider = accessor.Container.Resolve<IResourceProvider>(new Arguments(new { resourceName = resourceName }));
            LoggerService.Information(String.Format("GetResourceProvider > Tracking? {0}", accessor.Container.Kernel.ReleasePolicy.HasTrack(provider)));

            return provider;
        }

        public override IResourceProvider CreateGlobalResourceProvider(string resourceName)
        {
            LoggerService.Debug(">>CreateGlobalResourceProvider({0})", resourceName);
            return GetResourceProvider(resourceName);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            LoggerService.Debug(">>CreateLocalResourceProvider({0})", virtualPath);
            string resourceName = virtualPath;
            if (!string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = virtualPath.Remove(0, 1);
                resourceName = virtualPath.Remove(0, virtualPath.IndexOf('/') + 1);
            }
            return GetResourceProvider(resourceName);
        }       
    }

    public class DynamicResourceProvider : IResourceProvider
    {
        public DynamicResourceProvider(string resourceName)
        {
            _resourceName = resourceName;
        }
        public readonly static string ResourcePath = WebConfiguration.Current.ResourcePath;

        //public IPageFactory PageFactory { get; set; }
        //public ICacheAgent CacheAgent { get; set; }

        protected virtual string GetPathToResource(string resourceName)
        {
            return string.Format(ResourcePath, resourceName);
        }

        private string _resourceName;
        private object lock1 = new object();
        public XPathNavigator ResourceDocument
        {
            get
            {
                string path = GetPathToResource(_resourceName);
                LoggerService.Debug(">>DynamicResourceProvider load document ({0})", path);

                IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                //IResourceDocument resourceDocument = accessor.Container.Resolve<IResourceDocument>(new Arguments(new { resourcePath = path }));
                
                IResourceDocument resourceDocument;                
                if ( _resourceName.Contains("seo_resources"))
                {
                    resourceDocument = accessor.Container.Resolve<IResourceDocument>("seolabels");
                }
                else{
                    resourceDocument = accessor.Container.Resolve<IResourceDocument>("labels");
                }

                LoggerService.Information(String.Format("ResourceDocument > Tracking? {0}", accessor.Container.Kernel.ReleasePolicy.HasTrack(resourceDocument)));
                LoggerService.Information(String.Format("ResourceDocument > doc: {0}", resourceDocument.GetHashCode()));

                return resourceDocument.doc;


                //string xml = PageFactory.FindPageContent(resourcePath);
                //XPathDocument xpath =  new XPathDocument(new StringReader(xml));
                //return xpath.CreateNavigator();

                //string resourcePath = GetPathToResource(_resourceName);
                //string cacheKey = string.Format("Resource_{0}", resourcePath);
                //XmlDocument xmlDoc = CacheAgent.Load(cacheKey) as XmlDocument;
                //if (xmlDoc != null)
                //    return xmlDoc;

                //lock (lock1)
                //{
                //    xmlDoc = new XmlDocument();
                //    xmlDoc.LoadXml(PageFactory.FindPageContent(resourcePath));
                //    CacheAgent.Store(cacheKey, "System", xmlDoc);
                //    return xmlDoc;
                //}
            }
        }



        public object GetObject(string resourceKey, System.Globalization.CultureInfo culture)
        {
            string result = String.Format("[{0}] not found", resourceKey);

            LoggerService.Debug(">>DynamicResourceProvider({0})", resourceKey);

            string key = string.Format("/root/data[@name='{0}']/value", resourceKey);
            XPathNavigator xpath_result = ResourceDocument.SelectSingleNode(key) as XPathNavigator;
            if (xpath_result != null)
            {
                //throw new ArgumentException(string.Format("Resource {0} does not exist in bundle {1}", resourceKey, _resourceName));
                result = xpath_result.Value;
            }

            LoggerService.Debug("<<DynamicResourceProvider({0})", resourceKey);
            return result;
        }

        public System.Resources.IResourceReader ResourceReader
        {
            get { throw new NotImplementedException(); }
        }
    }

    public interface IResourceDocument
    {
        XPathNavigator doc { get; }
    }
    public class ResourceDocument : IResourceDocument
    {
        public string ResourcePath { get; private set; }

        private XPathDocument xpath;

        public ResourceDocument(string resourcePath, IPageFactory pageFactory)
        {
            ResourcePath = resourcePath;

            string xml = pageFactory.FindPageContent(resourcePath);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            xpath = new XPathDocument(new StringReader(xml));

            stopWatch.Stop();

            LoggerService.Debug(">>ResourceDocument ({0})", stopWatch.Elapsed.TotalMilliseconds);
        }

        public XPathNavigator doc
        {
            get
            {
                return xpath.CreateNavigator();
            }
        }
    }
}