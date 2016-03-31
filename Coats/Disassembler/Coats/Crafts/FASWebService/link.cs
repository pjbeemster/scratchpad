namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough]
    public class link : INotifyPropertyChanged
    {
        private string fh_secondidField;
        private string nameField;
        private int nrField;
        private bool nrFieldSpecified;
        private Coats.Crafts.FASWebService.seourl seourlField;
        private int sortedField;
        private bool sortedFieldSpecified;
        private linkSortorder sortorderField;
        private bool sortorderFieldSpecified;
        private linkType typeField;
        private bool typeFieldSpecified;
        private string urlparamsField;
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

        [XmlAttribute]
        public string fh_secondid
        {
            get
            {
                return this.fh_secondidField;
            }
            set
            {
                this.fh_secondidField = value;
                this.RaisePropertyChanged("fh_secondid");
            }
        }

        [XmlElement(Order=0)]
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

        [XmlIgnore]
        public bool nrSpecified
        {
            get
            {
                return this.nrFieldSpecified;
            }
            set
            {
                this.nrFieldSpecified = value;
                this.RaisePropertyChanged("nrSpecified");
            }
        }

        [XmlElement("seo-url", Order=2)]
        public Coats.Crafts.FASWebService.seourl seourl
        {
            get
            {
                return this.seourlField;
            }
            set
            {
                this.seourlField = value;
                this.RaisePropertyChanged("seourl");
            }
        }

        [XmlAttribute]
        public int sorted
        {
            get
            {
                return this.sortedField;
            }
            set
            {
                this.sortedField = value;
                this.RaisePropertyChanged("sorted");
            }
        }

        [XmlIgnore]
        public bool sortedSpecified
        {
            get
            {
                return this.sortedFieldSpecified;
            }
            set
            {
                this.sortedFieldSpecified = value;
                this.RaisePropertyChanged("sortedSpecified");
            }
        }

        [XmlAttribute("sort-order")]
        public linkSortorder sortorder
        {
            get
            {
                return this.sortorderField;
            }
            set
            {
                this.sortorderField = value;
                this.RaisePropertyChanged("sortorder");
            }
        }

        [XmlIgnore]
        public bool sortorderSpecified
        {
            get
            {
                return this.sortorderFieldSpecified;
            }
            set
            {
                this.sortorderFieldSpecified = value;
                this.RaisePropertyChanged("sortorderSpecified");
            }
        }

        [XmlAttribute]
        public linkType type
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

        [XmlIgnore]
        public bool typeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
                this.RaisePropertyChanged("typeSpecified");
            }
        }

        [XmlElement("url-params", Order=1)]
        public string urlparams
        {
            get
            {
                return this.urlparamsField;
            }
            set
            {
                this.urlparamsField = value;
                this.RaisePropertyChanged("urlparams");
            }
        }

        [XmlElement(Order=3)]
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

