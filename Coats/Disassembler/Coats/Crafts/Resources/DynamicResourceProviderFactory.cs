namespace Coats.Crafts.Resources
{
    using Castle.MicroKernel;
    using Castle.Windsor;
    using DD4T.Utils;
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.Compilation;

    public class DynamicResourceProviderFactory : ResourceProviderFactory
    {
        public override IResourceProvider CreateGlobalResourceProvider(string resourceName)
        {
            LoggerService.Debug(">>CreateGlobalResourceProvider({0})", new object[] { resourceName });
            return this.GetResourceProvider(resourceName);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            LoggerService.Debug(">>CreateLocalResourceProvider({0})", new object[] { virtualPath });
            string resourceName = virtualPath;
            if (!string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = virtualPath.Remove(0, 1);
                resourceName = virtualPath.Remove(0, virtualPath.IndexOf('/') + 1);
            }
            return this.GetResourceProvider(resourceName);
        }

        protected virtual IResourceProvider GetResourceProvider(string resourceName)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            IResourceProvider instance = applicationInstance.Container.Resolve<IResourceProvider>((IDictionary) new Arguments(new { resourceName = resourceName }, new IArgumentsComparer[0]));
            LoggerService.Information(string.Format("GetResourceProvider > Tracking? {0}", applicationInstance.Container.Kernel.ReleasePolicy.HasTrack(instance)), new object[0]);
            return instance;
        }
    }
}

