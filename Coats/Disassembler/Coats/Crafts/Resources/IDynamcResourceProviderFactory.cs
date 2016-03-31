namespace Coats.Crafts.Resources
{
    using System;
    using System.Web.Compilation;

    public interface IDynamcResourceProviderFactory : IDisposable
    {
        IResourceProvider Create(string resourceName);
        void Release(IResourceProvider provider);
    }
}

