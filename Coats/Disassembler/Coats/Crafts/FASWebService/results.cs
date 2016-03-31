namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, DesignerCategory("code"), XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough]
    public class results : INotifyPropertyChanged
    {
        private int current_setField;
        private Coats.Crafts.FASWebService.seourl seourlField;
        private string start_index_paramField;
        private int start_indexField;
        private int total_itemsField;
        private string urlparamsField;
        private string view_set_sizeField;
        private string view_size_paramField;
        private int view_sizeField;

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
        public int current_set
        {
            get
            {
                return this.current_setField;
            }
            set
            {
                this.current_setField = value;
                this.RaisePropertyChanged("current_set");
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

        [XmlAttribute]
        public int start_index
        {
            get
            {
                return this.start_indexField;
            }
            set
            {
                this.start_indexField = value;
                this.RaisePropertyChanged("start_index");
            }
        }

        [XmlAttribute]
        public string start_index_param
        {
            get
            {
                return this.start_index_paramField;
            }
            set
            {
                this.start_index_paramField = value;
                this.RaisePropertyChanged("start_index_param");
            }
        }

        [XmlAttribute]
        public int total_items
        {
            get
            {
                return this.total_itemsField;
            }
            set
            {
                this.total_itemsField = value;
                this.RaisePropertyChanged("total_items");
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

        [XmlAttribute]
        public string view_set_size
        {
            get
            {
                return this.view_set_sizeField;
            }
            set
            {
                this.view_set_sizeField = value;
                this.RaisePropertyChanged("view_set_size");
            }
        }

        [XmlAttribute]
        public int view_size
        {
            get
            {
                return this.view_sizeField;
            }
            set
            {
                this.view_sizeField = value;
                this.RaisePropertyChanged("view_size");
            }
        }

        [XmlAttribute]
        public string view_size_param
        {
            get
            {
                return this.view_size_paramField;
            }
            set
            {
                this.view_size_paramField = value;
                this.RaisePropertyChanged("view_size_param");
            }
        }
    }
}

