namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough]
    public class theme : INotifyPropertyChanged
    {
        private customfield[] customfieldsField;
        private string idField;
        private item[] itemsField;
        private string itemsqueryField;
        private Coats.Crafts.FASWebService.link[] linkField;
        private string nameField;
        private string sloganField;
        private Coats.Crafts.FASWebService.staticcontent staticcontentField;
        private string titleField;
        private themeType typeField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlArrayItem("custom-field", IsNullable=false), XmlArray("custom-fields", Order=2)]
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
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
                this.RaisePropertyChanged("id");
            }
        }

        [XmlArray(Order=5), XmlArrayItem("item", IsNullable=false)]
        public item[] items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
                this.RaisePropertyChanged("items");
            }
        }

        [XmlAttribute("items-query")]
        public string itemsquery
        {
            get
            {
                return this.itemsqueryField;
            }
            set
            {
                this.itemsqueryField = value;
                this.RaisePropertyChanged("itemsquery");
            }
        }

        [XmlElement("link", Order=4)]
        public Coats.Crafts.FASWebService.link[] link
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

        [XmlElement(Order=1)]
        public string slogan
        {
            get
            {
                return this.sloganField;
            }
            set
            {
                this.sloganField = value;
                this.RaisePropertyChanged("slogan");
            }
        }

        [XmlElement("static-content", Order=3)]
        public Coats.Crafts.FASWebService.staticcontent staticcontent
        {
            get
            {
                return this.staticcontentField;
            }
            set
            {
                this.staticcontentField = value;
                this.RaisePropertyChanged("staticcontent");
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
        public themeType type
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

