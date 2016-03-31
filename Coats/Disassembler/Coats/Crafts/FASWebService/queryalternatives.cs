namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code")]
    public class queryalternatives : INotifyPropertyChanged
    {
        private Coats.Crafts.FASWebService.querysuggestion[] querysuggestionField;
        private bool wasexecutedField = true;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("query-suggestion", Order=0)]
        public Coats.Crafts.FASWebService.querysuggestion[] querysuggestion
        {
            get
            {
                return this.querysuggestionField;
            }
            set
            {
                this.querysuggestionField = value;
                this.RaisePropertyChanged("querysuggestion");
            }
        }

        [DefaultValue(true), XmlAttribute("was-executed")]
        public bool wasexecuted
        {
            get
            {
                return this.wasexecutedField;
            }
            set
            {
                this.wasexecutedField = value;
                this.RaisePropertyChanged("wasexecuted");
            }
        }
    }
}

