namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DesignerCategory("code"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public class facetmap : INotifyPropertyChanged
    {
        private Coats.Crafts.FASWebService.filter[] filterField;
        private string universeField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("filter", Order=0)]
        public Coats.Crafts.FASWebService.filter[] filter
        {
            get
            {
                return this.filterField;
            }
            set
            {
                this.filterField = value;
                this.RaisePropertyChanged("filter");
            }
        }

        [XmlAttribute]
        public string universe
        {
            get
            {
                return this.universeField;
            }
            set
            {
                this.universeField = value;
                this.RaisePropertyChanged("universe");
            }
        }
    }
}

