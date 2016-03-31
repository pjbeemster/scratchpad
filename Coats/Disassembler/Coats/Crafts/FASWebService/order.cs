namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DebuggerStepThrough, DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233")]
    public class order : INotifyPropertyChanged
    {
        private orderError errorField;
        private bool errorFieldSpecified;
        private string[] itemField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement(Order=1)]
        public orderError error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
                this.RaisePropertyChanged("error");
            }
        }

        [XmlIgnore]
        public bool errorSpecified
        {
            get
            {
                return this.errorFieldSpecified;
            }
            set
            {
                this.errorFieldSpecified = value;
                this.RaisePropertyChanged("errorSpecified");
            }
        }

        [XmlElement("item", Order=0)]
        public string[] item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
                this.RaisePropertyChanged("item");
            }
        }
    }
}

