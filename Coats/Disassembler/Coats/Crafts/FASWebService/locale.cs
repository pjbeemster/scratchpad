namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public class locale : INotifyPropertyChanged
    {
        private string mlvalueField;
        private bool selectedField = false;
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
        public string mlvalue
        {
            get
            {
                return this.mlvalueField;
            }
            set
            {
                this.mlvalueField = value;
                this.RaisePropertyChanged("mlvalue");
            }
        }

        [XmlAttribute, DefaultValue(false)]
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

