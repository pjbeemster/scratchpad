namespace Coats.Crafts.Controllers
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    public class EmbeddedResourceResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(Path.GetFileName(absoluteUri.AbsolutePath));
        }
    }
}

