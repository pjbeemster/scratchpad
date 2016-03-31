namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code")]
    public class similaritemfields : INotifyPropertyChanged
    {
        private attributeExtended[] attributeField;
        private string locationparamprefixField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("attribute", Order=0)]
        public attributeExtended[] attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
                this.RaisePropertyChanged("attribute");
            }
        }

        [XmlAttribute("location-param-prefix")]
        public string locationparamprefix
        {
            get
            {
                return this.locationparamprefixField;
            }
            set
            {
                this.locationparamprefixField = value;
                this.RaisePropertyChanged("locationparamprefix");
            }
        }
    }
}

