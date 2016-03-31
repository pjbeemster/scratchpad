namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "ItemId", "PublicationId" })]
    public class Page
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Author;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<ComponentPresentation> _ComponentPresentations = new Collection<ComponentPresentation>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _CreationDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<CustomMeta> _CustomMetas = new Collection<CustomMeta>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _InitialPublishDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Keyword> _Keywords = new Collection<Keyword>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _LastPublishDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _MajorVersion;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _MinorVersion;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _ModificationDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _OwningPublication;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.PageContent _PageContent;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _PagePath;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.StructureGroup _StructureGroup;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _TemplateId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Title;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Url;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Page CreatePage(int itemId, int publicationId, string author)
        {
            return new Page { 
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
        public Collection<CustomMeta> CustomMetas
        {
            get
            {
                return this._CustomMetas;
            }
            set
            {
                if (value != null)
                {
                    this._CustomMetas = value;
                }
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
        public Collection<Keyword> Keywords
        {
            get
            {
                return this._Keywords;
            }
            set
            {
                if (value != null)
                {
                    this._Keywords = value;
                }
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
        public Coats.Crafts.CDS.PageContent PageContent
        {
            get
            {
                return this._PageContent;
            }
            set
            {
                this._PageContent = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string PagePath
        {
            get
            {
                return this._PagePath;
            }
            set
            {
                this._PagePath = value;
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
        public Coats.Crafts.CDS.StructureGroup StructureGroup
        {
            get
            {
                return this._StructureGroup;
            }
            set
            {
                this._StructureGroup = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int? TemplateId
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

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
            }
        }
    }
}

