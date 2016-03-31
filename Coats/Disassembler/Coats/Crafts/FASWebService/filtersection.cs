namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code")]
    public class filtersection : INotifyPropertyChanged
    {
        private Coats.Crafts.FASWebService.link linkField;
        private int nrField;
        private bool selectedField;
        private bool selectedFieldSpecified;
        private Coats.Crafts.FASWebService.value valueField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement(Order=0)]
        public Coats.Crafts.FASWebService.link link
        {
            get
            {
                return this.linkField;
            }
            set
            {
                this.linkField = value;
                this.RaisePropertyChanged("link");
            }
        }

        [XmlAttribute]
        public int nr
        {
            get
            {
                return this.nrField;
            }
            set
            {
                this.nrField = value;
                this.RaisePropertyChanged("nr");
            }
        }

        [XmlAttribute]
        public bool selected
        {
            get
            {
                return this.selectedField;
            }
            set
            {
                this.selectedField = value;
                this.RaisePropertyChanged("selected");
            }
        }

        [XmlIgnore]
        public bool selectedSpecified
        {
            get
            {
                return this.selectedFieldSpecified;
            }
            set
            {
                this.selectedFieldSpecified = value;
                this.RaisePropertyChanged("selectedSpecified");
            }
        }

        [XmlElement(Order=1)]
        public Coats.Crafts.FASWebService.value value
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

