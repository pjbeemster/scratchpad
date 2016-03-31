namespace Coats.Crafts.Resources
{
    using DD4T.ContentModel.Factories;
    using DD4T.Utils;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Xml.XPath;

    public class ResourceDocument : IResourceDocument
    {
        private XPathDocument xpath;

        public ResourceDocument(string resourcePath, IPageFactory pageFactory)
        {
            this.ResourcePath = resourcePath;
            string s = pageFactory.FindPageContent(resourcePath);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.xpath = new XPathDocument(new StringReader(s));
            stopwatch.Stop();
            LoggerService.Debug(">>ResourceDocument ({0})", new object[] { stopwatch.Elapsed.TotalMilliseconds });
        }

        public XPathNavigator doc
        {
            get
            {
                return this.xpath.CreateNavigator();
            }
        }

        public string ResourcePath { get; private set; }
    }
}

