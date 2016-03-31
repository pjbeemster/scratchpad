namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough, DesignerCategory("code")]
    public class filter : INotifyPropertyChanged
    {
        private attributeTypeFormat basetypeField;
        private bool basetypeFieldSpecified;
        private customfield[] customfieldsField;
        private string facetidField;
        private Coats.Crafts.FASWebService.filtersection[] filtersectionField;
        private Coats.Crafts.FASWebService.link linkField;
        private string onField;
        private bool selectedField;
        private bool selectedFieldSpecified;
        private int show_number_valuesField;
        private bool show_number_valuesFieldSpecified;
        private string titleField;
        private string typeField;
        private string urlparamsbaseField;

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

        [XmlArrayItem("custom-field", IsNullable=false), XmlArray("custom-fields", Order=3)]
        public customfield[] customfields
        {
            get
            {
                return this.customfieldsField;
            }
            set
            {
                this.customfieldsField = value;
                this.RaisePropertyChanged("customfields");
            }
        }

        [XmlAttribute]
        public string facetid
        {
            get
            {
                return this.facetidField;
            }
            set
            {
                this.facetidField = value;
                this.RaisePropertyChanged("facetid");
            }
        }

        [XmlElement("filtersection", Order=1)]
        public Coats.Crafts.FASWebService.filtersection[] filtersection
        {
            get
            {
                return this.filtersectionField;
            }
            set
            {
                this.filtersectionField = value;
                this.RaisePropertyChanged("filtersection");
            }
        }

        [XmlElement(Order=4)]
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
        public string on
        {
            get
            {
                return this.onField;
            }
            set
            {
                this.onField = value;
                this.RaisePropertyChanged("on");
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

        [XmlAttribute]
        public int show_number_values
        {
            get
            {
                return this.show_number_valuesField;
            }
            set
            {
                this.show_number_valuesField = value;
                this.RaisePropertyChanged("show_number_values");
            }
        }

        [XmlIgnore]
        public bool show_number_valuesSpecified
        {
            get
            {
                return this.show_number_valuesFieldSpecified;
            }
            set
            {
                this.show_number_valuesFieldSpecified = value;
                this.RaisePropertyChanged("show_number_valuesSpecified");
            }
        }

        [XmlElement(Order=0)]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
                this.RaisePropertyChanged("title");
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

        [XmlElement("url-params-base", Order=2)]
        public string urlparamsbase
        {
            get
            {
                return this.urlparamsbaseField;
            }
            set
            {
                this.urlparamsbaseField = value;
                this.RaisePropertyChanged("urlparamsbase");
            }
        }
    }
}

