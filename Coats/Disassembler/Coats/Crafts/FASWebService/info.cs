namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code")]
    public class info : INotifyPropertyChanged
    {
        private string countryField;
        private string currentuniverseField;
        private string editurlField;
        private string langField;
        private Coats.Crafts.FASWebService.locale localeField;
        private Coats.Crafts.FASWebService.meta metaField;
        private modeType modeField;
        private string pathField;
        private string queryField;
        private string querystringhttpencodedField;
        private Coats.Crafts.FASWebService.server serverField;
        private string sessionField;
        private string sourcexmlField;
        private pageType typeField;
        private string urlField;
        private string usertypeField;
        private viewType viewField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement(DataType="language", Order=3)]
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
                this.RaisePropertyChanged("country");
            }
        }

        [XmlElement("current-universe", Order=5)]
        public string currentuniverse
        {
            get
            {
                return this.currentuniverseField;
            }
            set
            {
                this.currentuniverseField = value;
                this.RaisePropertyChanged("currentuniverse");
            }
        }

        [XmlElement("edit-url", Order=13)]
        public string editurl
        {
            get
            {
                return this.editurlField;
            }
            set
            {
                this.editurlField = value;
                this.RaisePropertyChanged("editurl");
            }
        }

        [XmlElement(DataType="language", Order=2)]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
                this.RaisePropertyChanged("lang");
            }
        }

        [XmlElement(Order=4)]
        public Coats.Crafts.FASWebService.locale locale
        {
            get
            {
                return this.localeField;
            }
            set
            {
                this.localeField = value;
                this.RaisePropertyChanged("locale");
            }
        }

        [XmlElement(Order=0x10)]
        public Coats.Crafts.FASWebService.meta meta
        {
            get
            {
                return this.metaField;
            }
            set
            {
                this.metaField = value;
                this.RaisePropertyChanged("meta");
            }
        }

        [XmlElement(Order=7)]
        public modeType mode
        {
            get
            {
                return this.modeField;
            }
            set
            {
                this.modeField = value;
                this.RaisePropertyChanged("mode");
            }
        }

        [XmlElement(Order=9)]
        public string path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
                this.RaisePropertyChanged("path");
            }
        }

        [XmlElement(Order=8)]
        public string query
        {
            get
            {
                return this.queryField;
            }
            set
            {
                this.queryField = value;
                this.RaisePropertyChanged("query");
            }
        }

        [XmlElement("query-string-httpencoded", Order=11)]
        public string querystringhttpencoded
        {
            get
            {
                return this.querystringhttpencodedField;
            }
            set
            {
                this.querystringhttpencodedField = value;
                this.RaisePropertyChanged("querystringhttpencoded");
            }
        }

        [XmlElement(Order=15)]
        public Coats.Crafts.FASWebService.server server
        {
            get
            {
                return this.serverField;
            }
            set
            {
                this.serverField = value;
                this.RaisePropertyChanged("server");
            }
        }

        [XmlElement(Order=0)]
        public string session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
                this.RaisePropertyChanged("session");
            }
        }

        [XmlElement("source-xml", Order=14)]
        public string sourcexml
        {
            get
            {
                return this.sourcexmlField;
            }
            set
            {
                this.sourcexmlField = value;
                this.RaisePropertyChanged("sourcexml");
            }
        }

        [XmlElement(Order=10)]
        public pageType type
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

        [XmlElement(Order=12)]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
                this.RaisePropertyChanged("url");
            }
        }

        [XmlElement("user-type", Order=1)]
        public string usertype
        {
            get
            {
                return this.usertypeField;
            }
            set
            {
                this.usertypeField = value;
                this.RaisePropertyChanged("usertype");
            }
        }

        [XmlElement(Order=6)]
        public viewType view
        {
            get
            {
                return this.viewField;
            }
            set
            {
                this.viewField = value;
                this.RaisePropertyChanged("view");
            }
        }
    }
}

