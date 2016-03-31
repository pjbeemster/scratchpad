namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DesignerCategory("code"), DebuggerStepThrough, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233")]
    public class breadcrumbs : INotifyPropertyChanged
    {
        private attributetype[] attributetypesField;
        private Coats.Crafts.FASWebService.crumb[] crumbField;
        private int nrofitemsinselectionField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlArray("attribute-types", Order=1), XmlArrayItem("attribute-type", IsNullable=false)]
        public attributetype[] attributetypes
        {
            get
            {
                return this.attributetypesField;
            }
            set
            {
                this.attributetypesField = value;
                this.RaisePropertyChanged("attributetypes");
            }
        }

        [XmlElement("crumb", Order=0)]
        public Coats.Crafts.FASWebService.crumb[] crumb
        {
            get
            {
                return this.crumbField;
            }
            set
            {
                this.crumbField = value;
                this.RaisePropertyChanged("crumb");
            }
        }

        [XmlAttribute("nr-of-items-in-selection")]
        public int nrofitemsinselection
        {
            get
            {
                return this.nrofitemsinselectionField;
            }
            set
            {
                this.nrofitemsinselectionField = value;
                this.RaisePropertyChanged("nrofitemsinselection");
            }
        }
    }
}

