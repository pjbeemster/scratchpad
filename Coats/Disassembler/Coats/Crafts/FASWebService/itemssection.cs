namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), DesignerCategory("code"), DebuggerStepThrough, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public class itemssection : INotifyPropertyChanged
    {
        private link[] headingField;
        private item[] itemsField;
        private Coats.Crafts.FASWebService.results resultsField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlArray(Order=1), XmlArrayItem("link", IsNullable=false)]
        public link[] heading
        {
            get
            {
                return this.headingField;
            }
            set
            {
                this.headingField = value;
                this.RaisePropertyChanged("heading");
            }
        }

        [XmlArrayItem("item", IsNullable=false), XmlArray(Order=2)]
        public item[] items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
                this.RaisePropertyChanged("items");
            }
        }

        [XmlElement(Order=0)]
        public Coats.Crafts.FASWebService.results results
        {
            get
            {
                return this.resultsField;
            }
            set
            {
                this.resultsField = value;
                this.RaisePropertyChanged("results");
            }
        }
    }
}

