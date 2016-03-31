namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), DesignerCategory("code"), XmlType(Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), XmlInclude(typeof(attributeExtended)), DebuggerStepThrough]
    public class attribute : INotifyPropertyChanged
    {
        private attributeTypeFormat basetypeField;
        private bool isnullField;
        private bool isnullFieldSpecified;
        private string mimetypeField;
        private string nameField;
        private string refsecondidField;
        private string skeyField;
        private Coats.Crafts.FASWebService.value[] valueField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlAttribute]
        public attributeTypeFormat basetype
        {
            get
            {
                return this.basetypeField;
            }
            set
            {
                this.basetypeField = value;
                this.RaisePropertyChanged("basetype");
            }
        }

        [XmlAttribute]
        public bool isnull
        {
            get
            {
                return this.isnullField;
            }
            set
            {
                this.isnullField = value;
                this.RaisePropertyChanged("isnull");
            }
        }

        [XmlIgnore]
        public bool isnullSpecified
        {
            get
            {
                return this.isnullFieldSpecified;
            }
            set
            {
                this.isnullFieldSpecified = value;
                this.RaisePropertyChanged("isnullSpecified");
            }
        }

        [XmlAttribute]
        public string mimetype
        {
            get
            {
                return this.mimetypeField;
            }
            set
            {
                this.mimetypeField = value;
                this.RaisePropertyChanged("mimetype");
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

        [XmlAttribute("ref-secondid")]
        public string refsecondid
        {
            get
            {
                return this.refsecondidField;
            }
            set
            {
                this.refsecondidField = value;
                this.RaisePropertyChanged("refsecondid");
            }
        }

        [XmlAttribute]
        public string skey
        {
            get
            {
                return this.skeyField;
            }
            set
            {
                this.skeyField = value;
                this.RaisePropertyChanged("skey");
            }
        }

        [XmlElement("value", Order=0)]
        public Coats.Crafts.FASWebService.value[] value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
                this.RaisePropertyChanged("value");
            }
        }
    }
}

