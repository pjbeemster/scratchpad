namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), XmlType(Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public enum errorType
    {
        emptykeeplist = 5,
        itemnotfound = 2,
        itemnotfoundlocale = 3,
        itemnotfoundlocation = 1,
        itemnotfounduniverse = 4,
        loading = 8,
        [XmlEnum("missing-arguments")]
        missingarguments = 6,
        [XmlEnum("no location and no secondid")]
        nolocationandnosecondid = 0,
        [XmlEnum("search-noresults")]
        searchnoresults = 7
    }
}

