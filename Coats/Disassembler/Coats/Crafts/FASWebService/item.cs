namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0")]
    public class item : INotifyPropertyChanged
    {
        private Coats.Crafts.FASWebService.attribute[] attributeField;
        private int currentrankField;
        private bool currentrankFieldSpecified;
        private string idField;
        private Coats.Crafts.FASWebService.link[] linkField;
        private int navindexField;
        private bool navindexFieldSpecified;
        private int quantityField;
        private bool quantityFieldSpecified;
        private string refdetailField;
        private Coats.Crafts.FASWebService.similaritemfields similaritemfieldsField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("attribute", Order=0)]
        public Coats.Crafts.FASWebService.attribute[] attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
                this.RaisePropertyChanged("attribute");
            }
        }

        [XmlAttribute("current-rank")]
        public int currentrank
        {
            get
            {
                return this.currentrankField;
            }
            set
            {
                this.currentrankField = value;
                this.RaisePropertyChanged("currentrank");
            }
        }

        [XmlIgnore]
        public bool currentrankSpecified
        {
            get
            {
                return this.currentrankFieldSpecified;
            }
            set
            {
                this.currentrankFieldSpecified = value;
                this.RaisePropertyChanged("currentrankSpecified");
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

        [XmlElement("link", Order=1)]
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

        [XmlElement("nav-index", Order=2)]
        public int navindex
        {
            get
            {
                return this.navindexField;
            }
            set
            {
                this.navindexField = value;
                this.RaisePropertyChanged("navindex");
            }
        }

        [XmlIgnore]
        public bool navindexSpecified
        {
            get
            {
                return this.navindexFieldSpecified;
            }
            set
            {
                this.navindexFieldSpecified = value;
                this.RaisePropertyChanged("navindexSpecified");
            }
        }

        [XmlElement(Order=4)]
        public int quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
                this.RaisePropertyChanged("quantity");
            }
        }

        [XmlIgnore]
        public bool quantitySpecified
        {
            get
            {
                return this.quantityFieldSpecified;
            }
            set
            {
                this.quantityFieldSpecified = value;
                this.RaisePropertyChanged("quantitySpecified");
            }
        }

        [XmlElement(Order=5)]
        public string refdetail
        {
            get
            {
                return this.refdetailField;
            }
            set
            {
                this.refdetailField = value;
                this.RaisePropertyChanged("refdetail");
            }
        }

        [XmlElement("similar-item-fields", Order=3)]
        public Coats.Crafts.FASWebService.similaritemfields similaritemfields
        {
            get
            {
                return this.similaritemfieldsField;
            }
            set
            {
                this.similaritemfieldsField = value;
                this.RaisePropertyChanged("similaritemfields");
            }
        }
    }
}

