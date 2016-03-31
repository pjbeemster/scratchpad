namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Xml.Serialization;

    [Serializable, XmlType(AnonymousType=true, Namespace="http://ns.fredhopper.com/XML/output/6.1.0"), GeneratedCode("System.Xml", "4.0.30319.233"), DebuggerStepThrough, DesignerCategory("code")]
    public class footer : INotifyPropertyChanged
    {
        private string logargsField;
        private Coats.Crafts.FASWebService.processtime processtimeField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("log-args", Order=1)]
        public string logargs
        {
            get
            {
                return this.logargsField;
            }
            set
            {
                this.logargsField = value;
                this.RaisePropertyChanged("logargs");
            }
        }

        [XmlElement("process-time", Order=0)]
        public Coats.Crafts.FASWebService.processtime processtime
        {
            get
            {
                return this.processtimeField;
            }
            set
            {
                this.processtimeField = value;
                this.RaisePropertyChanged("processtime");
            }
        }
    }
}

