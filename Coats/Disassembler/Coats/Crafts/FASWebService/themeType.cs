namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.Xml.Serialization;

    [Serializable, XmlType(Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233")]
    public enum themeType
    {
        facet = 3,
        [XmlEnum("facet-image")]
        facetimage = 0,
        [XmlEnum("facet-product")]
        facetproduct = 2,
        [XmlEnum("facet-text")]
        facettext = 1,
        item = 11,
        [XmlEnum("item-image")]
        itemimage = 8,
        [XmlEnum("item-product")]
        itemproduct = 10,
        [XmlEnum("item-text")]
        itemtext = 9,
        keyword = 7,
        [XmlEnum("keyword-image")]
        keywordimage = 4,
        [XmlEnum("keyword-product")]
        keywordproduct = 6,
        [XmlEnum("keyword-text")]
        keywordtext = 5
    }
}

