namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), XmlType(Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public enum linkType
    {
        detail = 5,
        [XmlEnum("filtered-all")]
        filteredall = 12,
        first = 7,
        index = 3,
        keeplist = 6,
        last = 8,
        less = 10,
        lister = 2,
        [XmlEnum("lowest-category")]
        lowestcategory = 13,
        more = 9,
        next = 0,
        previous = 1,
        search = 4,
        [XmlEnum("see-all")]
        seeall = 11
    }
}

