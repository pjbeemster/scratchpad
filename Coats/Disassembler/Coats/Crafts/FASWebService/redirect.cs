namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DebuggerStepThrough, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DesignerCategory("code"), GeneratedCode("System.Xml", "4.0.30319.233")]
    public class redirect : INotifyPropertyChanged
    {
        private string redirecthostField;
        private int redirectportField;
        private string redirecturlField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("redirect-host", Order=0)]
        public string redirecthost
        {
            get
            {
                return this.redirecthostField;
            }
            set
            {
                this.redirecthostField = value;
                this.RaisePropertyChanged("redirecthost");
            }
        }

        [XmlElement("redirect-port", Order=1)]
        public int redirectport
        {
            get
            {
                return this.redirectportField;
            }
            set
            {
                this.redirectportField = value;
                this.RaisePropertyChanged("redirectport");
            }
        }

        [XmlElement("redirect-url", DataType="anyURI", Order=2)]
        public string redirecturl
        {
            get
            {
                return this.redirecturlField;
            }
            set
            {
                this.redirecturlField = value;
                this.RaisePropertyChanged("redirecturl");
            }
        }
    }
}

