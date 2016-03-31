namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough]
    public class querysuggestion : INotifyPropertyChanged
    {
        private int estimatedresultsField;
        private bool estimatedresultsFieldSpecified;
        private string originalField;
        private Coats.Crafts.FASWebService.searchterms[] searchtermsField;
        private Coats.Crafts.FASWebService.seourl seourlField;
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

        [XmlAttribute("estimated-results")]
        public int estimatedresults
        {
            get
            {
                return this.estimatedresultsField;
            }
            set
            {
                this.estimatedresultsField = value;
                this.RaisePropertyChanged("estimatedresults");
            }
        }

        [XmlIgnore]
        public bool estimatedresultsSpecified
        {
            get
            {
                return this.estimatedresultsFieldSpecified;
            }
            set
            {
                this.estimatedresultsFieldSpecified = value;
                this.RaisePropertyChanged("estimatedresultsSpecified");
            }
        }

        [XmlAttribute]
        public string original
        {
            get
            {
                return this.originalField;
            }
            set
            {
                this.originalField = value;
                this.RaisePropertyChanged("original");
            }
        }

        [XmlElement("searchterms", Order=3)]
        public Coats.Crafts.FASWebService.searchterms[] searchterms
        {
            get
            {
                return this.searchtermsField;
            }
            set
            {
                this.searchtermsField = value;
                this.RaisePropertyChanged("searchterms");
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

        [XmlElement(Order=0)]
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

