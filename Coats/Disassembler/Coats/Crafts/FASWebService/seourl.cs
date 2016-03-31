namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code")]
    public class seourl : INotifyPropertyChanged
    {
        private bool isnullField;
        private bool isnullFieldSpecified;
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

