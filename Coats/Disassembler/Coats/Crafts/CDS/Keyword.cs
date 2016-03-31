namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "Id", "PublicationId", "TaxonomyId" })]
    public class Keyword
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private bool _Abstract;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Keyword> _Children = new Collection<Keyword>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Component> _Components = new Collection<Component>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<CustomMeta> _CustomMetas = new Collection<CustomMeta>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Depth;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Description;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private bool _HasChildren;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Id;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemType;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Key;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private bool _Navigable;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Page> _Pages = new Collection<Page>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Keyword _Parent;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _TaxonomyId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Title;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _TotalRelatedItems;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Keyword CreateKeyword(int ID, int publicationId, int taxonomyId, string title, bool hasChildren, bool @abstract, bool navigable, int depth, int itemType)
        {
            return new Keyword { 
                Id = ID,
                PublicationId = publicationId,
                TaxonomyId = taxonomyId,
                Title = title,
                HasChildren = hasChildren,
                Abstract = @abstract,
                Navigable = navigable,
                Depth = depth,
                ItemType = itemType
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public bool Abstract
        {
            get
            {
                return this._Abstract;
            }
            set
            {
                this._Abstract = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<Keyword> Children
        {
            get
            {
                return this._Children;
            }
            set
            {
                if (value != null)
                {
                    this._Children = value;
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
        public int Depth
        {
            get
            {
                return this._Depth;
            }
            set
            {
                this._Depth = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public bool HasChildren
        {
            get
            {
                return this._HasChildren;
            }
            set
            {
                this._HasChildren = value;
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
        public int ItemType
        {
            get
            {
                return this._ItemType;
            }
            set
            {
                this._ItemType = value;
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
        public bool Navigable
        {
            get
            {
                return this._Navigable;
            }
            set
            {
                this._Navigable = value;
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
        public Keyword Parent
        {
            get
            {
                return this._Parent;
            }
            set
            {
                this._Parent = value;
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
        public int TaxonomyId
        {
            get
            {
                return this._TaxonomyId;
            }
            set
            {
                this._TaxonomyId = value;
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
        public int? TotalRelatedItems
        {
            get
            {
                return this._TotalRelatedItems;
            }
            set
            {
                this._TotalRelatedItems = value;
            }
        }
    }
}

