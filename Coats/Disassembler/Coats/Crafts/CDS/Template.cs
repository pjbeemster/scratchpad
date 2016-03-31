namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "ItemId", "PublicationId" })]
    public class Template
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Author;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<ComponentPresentation> _ComponentPresentations = new Collection<ComponentPresentation>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _CreationDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _InitialPublishDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _LastPublishDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _MajorVersion;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _MinorVersion;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _ModificationDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _OutputFormat;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _OwningPublication;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _TemplatePriority;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Title;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Template CreateTemplate(int templatePriority, string outputFormat, int itemId, int publicationId, string author)
        {
            return new Template { 
                TemplatePriority = templatePriority,
                OutputFormat = outputFormat,
                ItemId = itemId,
                PublicationId = publicationId,
                Author = author
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Author
        {
            get
            {
                return this._Author;
            }
            set
            {
                this._Author = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<ComponentPresentation> ComponentPresentations
        {
            get
            {
                return this._ComponentPresentations;
            }
            set
            {
                if (value != null)
                {
                    this._ComponentPresentations = value;
                }
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DateTime? CreationDate
        {
            get
            {
                return this._CreationDate;
            }
            set
            {
                this._CreationDate = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DateTime? InitialPublishDate
        {
            get
            {
                return this._InitialPublishDate;
            }
            set
            {
                this._InitialPublishDate = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int ItemId
        {
            get
            {
                return this._ItemId;
            }
            set
            {
                this._ItemId = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DateTime? LastPublishDate
        {
            get
            {
                return this._LastPublishDate;
            }
            set
            {
                this._LastPublishDate = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int? MajorVersion
        {
            get
            {
                return this._MajorVersion;
            }
            set
            {
                this._MajorVersion = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int? MinorVersion
        {
            get
            {
                return this._MinorVersion;
            }
            set
            {
                this._MinorVersion = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DateTime? ModificationDate
        {
            get
            {
                return this._ModificationDate;
            }
            set
            {
                this._ModificationDate = value;
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
        public int? OwningPublication
        {
            get
            {
                return this._OwningPublication;
            }
            set
            {
                this._OwningPublication = value;
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
        public int TemplatePriority
        {
            get
            {
                return this._TemplatePriority;
            }
            set
            {
                this._TemplatePriority = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }
    }
}

