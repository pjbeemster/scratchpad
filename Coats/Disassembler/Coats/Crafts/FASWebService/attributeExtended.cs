namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Serialization;

    [Serializable, DesignerCategory("code"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, XmlType(Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public class attributeExtended : attribute
    {
        private string locationparamField;

        [XmlAttribute("location-param")]
        public string locationparam
        {
            get
            {
                return this.locationparamField;
            }
            set
            {
                this.locationparamField = value;
                base.RaisePropertyChanged("locationparam");
            }
        }
    }
}

