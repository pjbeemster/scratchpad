namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "PublicationId", "ComponentId", "TemplateId" })]
    public class ComponentPresentation
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.Component _Component;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ComponentId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _OutputFormat;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Page> _Pages = new Collection<Page>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _PresentationContent;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.Template _Template;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _TemplateId;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static ComponentPresentation CreateComponentPresentation(int publicationId, int componentId, int templateId, string presentationContent, string outputFormat)
        {
            return new ComponentPresentation { 
                PublicationId = publicationId,
                ComponentId = componentId,
                TemplateId = templateId,
                PresentationContent = presentationContent,
                OutputFormat = outputFormat
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Coats.Crafts.CDS.Component Component
        {
            get
            {
                return this._Component;
            }
            set
            {
                this._Component = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int ComponentId
        {
            get
            {
                return this._ComponentId;
            }
            set
            {
                this._ComponentId = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string OutputFormat
        {
            get
            {
                return this._OutputFormat;
            }
            set
            {
                this._OutputFormat = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<Page> Pages
        {
            get
            {
                return this._Pages;
            }
            set
            {
                if (value != null)
                {
                    this._Pages = value;
                }
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string PresentationContent
        {
            get
            {
                return this._PresentationContent;
            }
            set
            {
                this._PresentationContent = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int PublicationId
        {
            get
            {
                return this._PublicationId;
            }
            set
            {
                this._PublicationId = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Coats.Crafts.CDS.Template Template
        {
            get
            {
                return this._Template;
            }
            set
            {
                this._Template = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int TemplateId
        {
            get
            {
                return this._TemplateId;
            }
            set
            {
                this._TemplateId = value;
            }
        }
    }
}

