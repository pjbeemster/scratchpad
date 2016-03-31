namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough, DesignerCategory("code")]
    public class crumb : INotifyPropertyChanged
    {
        private Coats.Crafts.FASWebService.name nameField;
        private Coats.Crafts.FASWebService.searchterms searchtermsField;
        private Coats.Crafts.FASWebService.seourl seourlField;
        private string urlparamsField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement(Order=2)]
        public Coats.Crafts.FASWebService.name name
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

        [XmlElement(Order=3)]
        public Coats.Crafts.FASWebService.searchterms searchterms
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

        [XmlElement("seo-url", Order=1)]
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

        [XmlElement("url-params", Order=0)]
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
    }
}

