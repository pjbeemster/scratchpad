namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough, DesignerCategory("code")]
    public class content : INotifyPropertyChanged
    {
        private Coats.Crafts.FASWebService.contentlink contentlinkField;
        private string contentvalueField;
        private string nameField;
        private staticContentType typeField;
        private bool typeFieldSpecified;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("content-link", Order=1)]
        public Coats.Crafts.FASWebService.contentlink contentlink
        {
            get
            {
                return this.contentlinkField;
            }
            set
            {
                this.contentlinkField = value;
                this.RaisePropertyChanged("contentlink");
            }
        }

        [XmlElement("content-value", Order=0)]
        public string contentvalue
        {
            get
            {
                return this.contentvalueField;
            }
            set
            {
                this.contentvalueField = value;
                this.RaisePropertyChanged("contentvalue");
            }
        }

        [XmlAttribute]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
                this.RaisePropertyChanged("name");
            }
        }

        [XmlAttribute]
        public staticContentType type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
                this.RaisePropertyChanged("type");
            }
        }

        [XmlIgnore]
        public bool typeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
                this.RaisePropertyChanged("typeSpecified");
            }
        }
    }
}

