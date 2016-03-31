namespace Coats.Crafts.Resources
{
    using System.Xml.XPath;

    public interface IResourceDocument
    {
        XPathNavigator doc { get; }
    }
}

