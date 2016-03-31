namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey("Id")]
    public class Publication
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Binary> _Binaries = new Collection<Binary>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<BinaryVariant> _BinaryVariants = new Collection<BinaryVariant>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<ComponentPresentation> _ComponentPresentations = new Collection<ComponentPresentation>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Component> _Components = new Collection<Component>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<CustomMeta> _CustomMetas = new Collection<CustomMeta>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Id;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Key;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Keyword> _Keywords = new Collection<Keyword>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _MultimediaPath;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _MultimediaUrl;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<PageContent> _PageContents = new Collection<PageContent>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Page> _Pages = new Collection<Page>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _PublicationPath;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _PublicationUrl;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Schema> _Schemas = new Collection<Schema>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<StructureGroup> _StructureGroups = new Collection<StructureGroup>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Template> _Templates = new Collection<Template>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Title;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Publication CreatePublication(int ID)
        {
            return new Publication { Id = ID };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<Binary> Binaries
        {
            get
            {
                return this._Binaries;
            }
            set
            {
                if (value != null)
                {
                    this._Binaries = value;
                }
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<BinaryVariant> BinaryVariants
        {
            get
            {
                return this._BinaryVariants;
            }
            set
            {
                if (value != null)
                {
                    this._BinaryVariants = value;
                }
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
        public Collection<Component> Components
        {
            get
            {
                return this._Components;
            }
            set
            {
                if (value != null)
                {
                    this._Components = value;
                }
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
        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this._Id = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Key
        {
            get
            {
                return this._Key;
            }
            set
            {
                this._Key = value;
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
        public string MultimediaPath
        {
            get
            {
                return this._MultimediaPath;
            }
            set
            {
                this._MultimediaPath = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string MultimediaUrl
        {
            get
            {
                return this._MultimediaUrl;
            }
            set
            {
                this._MultimediaUrl = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<PageContent> PageContents
        {
            get
            {
                return this._PageContents;
            }
            set
            {
                if (value != null)
                {
                    this._PageContents = value;
                }
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
        public string PublicationPath
        {
            get
            {
                return this._PublicationPath;
            }
            set
            {
                this._PublicationPath = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string PublicationUrl
        {
            get
            {
                return this._PublicationUrl;
            }
            set
            {
                this._PublicationUrl = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<Schema> Schemas
        {
            get
            {
                return this._Schemas;
            }
            set
            {
                if (value != null)
                {
                    this._Schemas = value;
                }
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<StructureGroup> StructureGroups
        {
            get
            {
                return this._StructureGroups;
            }
            set
            {
                if (value != null)
                {
                    this._StructureGroups = value;
                }
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<Template> Templates
        {
            get
            {
                return this._Templates;
            }
            set
            {
                if (value != null)
                {
                    this._Templates = value;
                }
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

