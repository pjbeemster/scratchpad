namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code")]
    public class value : INotifyPropertyChanged
    {
        private bool islocaleField;
        private bool islocaleFieldSpecified;
        private string nonmlField;
        private string universerefField;
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
        public bool islocale
        {
            get
            {
                return this.islocaleField;
            }
            set
            {
                this.islocaleField = value;
                this.RaisePropertyChanged("islocale");
            }
        }

        [XmlIgnore]
        public bool islocaleSpecified
        {
            get
            {
                return this.islocaleFieldSpecified;
            }
            set
            {
                this.islocaleFieldSpecified = value;
                this.RaisePropertyChanged("islocaleSpecified");
            }
        }

        [XmlAttribute("non-ml")]
        public string nonml
        {
            get
            {
                return this.nonmlField;
            }
            set
            {
                this.nonmlField = value;
                this.RaisePropertyChanged("nonml");
            }
        }

        [XmlAttribute("universe-ref")]
        public string universeref
        {
            get
            {
                return this.universerefField;
            }
            set
            {
                this.universerefField = value;
                this.RaisePropertyChanged("universeref");
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

