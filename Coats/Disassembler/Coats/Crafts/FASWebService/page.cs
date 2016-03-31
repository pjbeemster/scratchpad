namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DebuggerStepThrough, DesignerCategory("code"), GeneratedCode("System.Xml", "4.0.30319.233")]
    public class page : INotifyPropertyChanged
    {
        private Coats.Crafts.FASWebService.footer footerField;
        private Coats.Crafts.FASWebService.info infoField;
        private Coats.Crafts.FASWebService.order orderField;
        private string qidField;
        private Coats.Crafts.FASWebService.redirect redirectField;
        private string searchpassField;
        private Coats.Crafts.FASWebService.searchterms searchtermsField;
        private universe[] universesField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement(Order=6)]
        public Coats.Crafts.FASWebService.footer footer
        {
            get
            {
                return this.footerField;
            }
            set
            {
                this.footerField = value;
                this.RaisePropertyChanged("footer");
            }
        }

        [XmlElement(Order=0)]
        public Coats.Crafts.FASWebService.info info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
                this.RaisePropertyChanged("info");
            }
        }

        [XmlElement(Order=5)]
        public Coats.Crafts.FASWebService.order order
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
                this.RaisePropertyChanged("order");
            }
        }

        [XmlAttribute]
        public string qid
        {
            get
            {
                return this.qidField;
            }
            set
            {
                this.qidField = value;
                this.RaisePropertyChanged("qid");
            }
        }

        [XmlElement(Order=1)]
        public Coats.Crafts.FASWebService.redirect redirect
        {
            get
            {
                return this.redirectField;
            }
            set
            {
                this.redirectField = value;
                this.RaisePropertyChanged("redirect");
            }
        }

        [XmlElement(Order=3)]
        public string searchpass
        {
            get
            {
                return this.searchpassField;
            }
            set
            {
                this.searchpassField = value;
                this.RaisePropertyChanged("searchpass");
            }
        }

        [XmlElement(Order=2)]
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

        [XmlArray(Order=4), XmlArrayItem("universe", IsNullable=false)]
        public universe[] universes
        {
            get
            {
                return this.universesField;
            }
            set
            {
                this.universesField = value;
                this.RaisePropertyChanged("universes");
            }
        }
    }
}

