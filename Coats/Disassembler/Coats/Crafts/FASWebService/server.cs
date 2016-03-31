namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), DesignerCategory("code"), DebuggerStepThrough]
    public class server : INotifyPropertyChanged
    {
        private string configdirField;
        private string contextrootField;
        private string defaultuniverseField;
        private string encodingdetectstringField;
        private string hostField;
        private locale[] localesField;
        private int portField;
        private Coats.Crafts.FASWebService.role roleField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("config-dir", Order=5)]
        public string configdir
        {
            get
            {
                return this.configdirField;
            }
            set
            {
                this.configdirField = value;
                this.RaisePropertyChanged("configdir");
            }
        }

        [XmlElement("context-root", Order=3)]
        public string contextroot
        {
            get
            {
                return this.contextrootField;
            }
            set
            {
                this.contextrootField = value;
                this.RaisePropertyChanged("contextroot");
            }
        }

        [XmlElement("default-universe", Order=7)]
        public string defaultuniverse
        {
            get
            {
                return this.defaultuniverseField;
            }
            set
            {
                this.defaultuniverseField = value;
                this.RaisePropertyChanged("defaultuniverse");
            }
        }

        [XmlElement("encoding-detect-string", Order=0)]
        public string encodingdetectstring
        {
            get
            {
                return this.encodingdetectstringField;
            }
            set
            {
                this.encodingdetectstringField = value;
                this.RaisePropertyChanged("encodingdetectstring");
            }
        }

        [XmlElement(Order=1)]
        public string host
        {
            get
            {
                return this.hostField;
            }
            set
            {
                this.hostField = value;
                this.RaisePropertyChanged("host");
            }
        }

        [XmlArrayItem("locale", IsNullable=false), XmlArray(Order=6)]
        public locale[] locales
        {
            get
            {
                return this.localesField;
            }
            set
            {
                this.localesField = value;
                this.RaisePropertyChanged("locales");
            }
        }

        [XmlElement(Order=2)]
        public int port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
                this.RaisePropertyChanged("port");
            }
        }

        [XmlElement(Order=4)]
        public Coats.Crafts.FASWebService.role role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
                this.RaisePropertyChanged("role");
            }
        }
    }
}

