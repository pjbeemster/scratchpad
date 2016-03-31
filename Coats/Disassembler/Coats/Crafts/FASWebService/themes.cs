namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough]
    public class themes : INotifyPropertyChanged
    {
        private attributetype[] attributetypesField;
        private string itemidField;
        private Coats.Crafts.FASWebService.theme[] themeField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlArrayItem("attribute-type", IsNullable=false), XmlArray("attribute-types", Order=1)]
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

        [XmlAttribute]
        public string itemid
        {
            get
            {
                return this.itemidField;
            }
            set
            {
                this.itemidField = value;
                this.RaisePropertyChanged("itemid");
            }
        }

        [XmlElement("theme", Order=0)]
        public Coats.Crafts.FASWebService.theme[] theme
        {
            get
            {
                return this.themeField;
            }
            set
            {
                this.themeField = value;
                this.RaisePropertyChanged("theme");
            }
        }
    }
}

