namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DesignerCategory("code"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public class universe : INotifyPropertyChanged
    {
        private attributetype[] attributetypesField;
        private Coats.Crafts.FASWebService.breadcrumbs breadcrumbsField;
        private field[] displayfieldsField;
        private Coats.Crafts.FASWebService.error errorField;
        private Coats.Crafts.FASWebService.facetmap[] facetmapField;
        private Coats.Crafts.FASWebService.itemssection itemssectionField;
        private Coats.Crafts.FASWebService.link linkField;
        private string nameField;
        private Coats.Crafts.FASWebService.queryalternatives queryalternativesField;
        private Coats.Crafts.FASWebService.themes[] themesField;
        private universeType typeField = universeType.deselected;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlArray("attribute-types", Order=8), XmlArrayItem("attribute-type", IsNullable=false)]
        public attributetype[] attributetypes
        {
            get
            {
                return this.attributetypesField;
            }
            set
            {
                this.attributetypesField = value;
                this.RaisePropertyChanged("attributetypes");
            }
        }

        [XmlElement(Order=3)]
        public Coats.Crafts.FASWebService.breadcrumbs breadcrumbs
        {
            get
            {
                return this.breadcrumbsField;
            }
            set
            {
                this.breadcrumbsField = value;
                this.RaisePropertyChanged("breadcrumbs");
            }
        }

        [XmlArray("display-fields", Order=7), XmlArrayItem("field", IsNullable=false)]
        public field[] displayfields
        {
            get
            {
                return this.displayfieldsField;
            }
            set
            {
                this.displayfieldsField = value;
                this.RaisePropertyChanged("displayfields");
            }
        }

        [XmlElement(Order=0)]
        public Coats.Crafts.FASWebService.error error
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

        [XmlElement("facetmap", Order=2)]
        public Coats.Crafts.FASWebService.facetmap[] facetmap
        {
            get
            {
                return this.facetmapField;
            }
            set
            {
                this.facetmapField = value;
                this.RaisePropertyChanged("facetmap");
            }
        }

        [XmlElement("items-section", Order=4)]
        public Coats.Crafts.FASWebService.itemssection itemssection
        {
            get
            {
                return this.itemssectionField;
            }
            set
            {
                this.itemssectionField = value;
                this.RaisePropertyChanged("itemssection");
            }
        }

        [XmlElement(Order=1)]
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

        [XmlElement("query-alternatives", Order=6)]
        public Coats.Crafts.FASWebService.queryalternatives queryalternatives
        {
            get
            {
                return this.queryalternativesField;
            }
            set
            {
                this.queryalternativesField = value;
                this.RaisePropertyChanged("queryalternatives");
            }
        }

        [XmlElement("themes", Order=5)]
        public Coats.Crafts.FASWebService.themes[] themes
        {
            get
            {
                return this.themesField;
            }
            set
            {
                this.themesField = value;
                this.RaisePropertyChanged("themes");
            }
        }

        [XmlAttribute, DefaultValue(1)]
        public universeType type
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
    }
}

