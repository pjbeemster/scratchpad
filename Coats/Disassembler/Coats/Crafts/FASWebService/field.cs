namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DebuggerStepThrough, DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233")]
    public class field : INotifyPropertyChanged
    {
        private attributeTypeFormat basetypeField;
        private string nameField;
        private string refuniverseField;
        private bool reverseField = false;
        private string selectioncriteriaField;
        private string skeyField;
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

        [XmlAttribute("ref-universe")]
        public string refuniverse
        {
            get
            {
                return this.refuniverseField;
            }
            set
            {
                this.refuniverseField = value;
                this.RaisePropertyChanged("refuniverse");
            }
        }

        [DefaultValue(false), XmlAttribute]
        public bool reverse
        {
            get
            {
                return this.reverseField;
            }
            set
            {
                this.reverseField = value;
                this.RaisePropertyChanged("reverse");
            }
        }

        [XmlAttribute("selection-criteria")]
        public string selectioncriteria
        {
            get
            {
                return this.selectioncriteriaField;
            }
            set
            {
                this.selectioncriteriaField = value;
                this.RaisePropertyChanged("selectioncriteria");
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

