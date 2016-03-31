namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough, DesignerCategory("code"), GeneratedCode("System.Xml", "4.0.30319.233")]
    public class attributetype : INotifyPropertyChanged
    {
        private attributeTypeFormat basetypeField;
        private bool basetypeFieldSpecified;
        private yesNo differsField;
        private bool differsFieldSpecified;
        private string nameField;
        private string typeField;
        private string valueField;

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

        [XmlIgnore]
        public bool basetypeSpecified
        {
            get
            {
                return this.basetypeFieldSpecified;
            }
            set
            {
                this.basetypeFieldSpecified = value;
                this.RaisePropertyChanged("basetypeSpecified");
            }
        }

        [XmlAttribute]
        public yesNo differs
        {
            get
            {
                return this.differsField;
            }
            set
            {
                this.differsField = value;
                this.RaisePropertyChanged("differs");
            }
        }

        [XmlIgnore]
        public bool differsSpecified
        {
            get
            {
                return this.differsFieldSpecified;
            }
            set
            {
                this.differsFieldSpecified = value;
                this.RaisePropertyChanged("differsSpecified");
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
        public string type
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

        [XmlText]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
                this.RaisePropertyChanged("Value");
            }
        }
    }
}

