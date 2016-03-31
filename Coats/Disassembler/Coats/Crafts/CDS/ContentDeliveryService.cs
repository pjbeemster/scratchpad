namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Services.Client;

    public class ContentDeliveryService : DataServiceContext
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Binary> _Binaries;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<BinaryVariant> _BinaryVariants;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Comment> _Comments;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<ComponentPresentation> _ComponentPresentations;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Component> _Components;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<CustomMeta> _CustomMetas;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<ItemStat> _ItemStats;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Keyword> _Keywords;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<PageContent> _PageContents;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Page> _Pages;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Publication> _Publications;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Rating> _Ratings;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Schema> _Schemas;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<StructureGroup> _StructureGroups;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<Template> _Templates;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DataServiceQuery<User> _Users;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public ContentDeliveryService(Uri serviceRoot) : base(serviceRoot)
        {
            base.ResolveName = new Func<Type, string>(this.ResolveNameFromType);
            base.ResolveType = new Func<string, Type>(this.ResolveTypeFromName);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToBinaries(Binary binary)
        {
            base.AddObject("Binaries", binary);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToBinaryVariants(BinaryVariant binaryVariant)
        {
            base.AddObject("BinaryVariants", binaryVariant);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToComments(Comment comment)
        {
            base.AddObject("Comments", comment);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToComponentPresentations(ComponentPresentation componentPresentation)
        {
            base.AddObject("ComponentPresentations", componentPresentation);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToComponents(Component component)
        {
            base.AddObject("Components", component);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToCustomMetas(CustomMeta customMeta)
        {
            base.AddObject("CustomMetas", customMeta);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToItemStats(ItemStat itemStat)
        {
            base.AddObject("ItemStats", itemStat);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToKeywords(Keyword keyword)
        {
            base.AddObject("Keywords", keyword);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToPageContents(PageContent pageContent)
        {
            base.AddObject("PageContents", pageContent);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToPages(Page page)
        {
            base.AddObject("Pages", page);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToPublications(Publication publication)
        {
            base.AddObject("Publications", publication);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToRatings(Rating rating)
        {
            base.AddObject("Ratings", rating);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToSchemas(Schema schema)
        {
            base.AddObject("Schemas", schema);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToStructureGroups(StructureGroup structureGroup)
        {
            base.AddObject("StructureGroups", structureGroup);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToTemplates(Template template)
        {
            base.AddObject("Templates", template);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public void AddToUsers(User user)
        {
            base.AddObject("Users", user);
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        protected string ResolveNameFromType(Type clientType)
        {
            if (clientType.Namespace.Equals("Coats.Crafts.CDS", StringComparison.Ordinal))
            {
                return ("Tridion.ContentDelivery." + clientType.Name);
            }
            return null;
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        protected Type ResolveTypeFromName(string typeName)
        {
            if (typeName.StartsWith("Tridion.ContentDelivery", StringComparison.Ordinal))
            {
                return base.GetType().Assembly.GetType("Coats.Crafts.CDS" + typeName.Substring(0x17), false);
            }
            return null;
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Binary> Binaries
        {
            get
            {
                if (this._Binaries == null)
                {
                    this._Binaries = base.CreateQuery<Binary>("Binaries");
                }
                return this._Binaries;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<BinaryVariant> BinaryVariants
        {
            get
            {
                if (this._BinaryVariants == null)
                {
                    this._BinaryVariants = base.CreateQuery<BinaryVariant>("BinaryVariants");
                }
                return this._BinaryVariants;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Comment> Comments
        {
            get
            {
                if (this._Comments == null)
                {
                    this._Comments = base.CreateQuery<Comment>("Comments");
                }
                return this._Comments;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<ComponentPresentation> ComponentPresentations
        {
            get
            {
                if (this._ComponentPresentations == null)
                {
                    this._ComponentPresentations = base.CreateQuery<ComponentPresentation>("ComponentPresentations");
                }
                return this._ComponentPresentations;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Component> Components
        {
            get
            {
                if (this._Components == null)
                {
                    this._Components = base.CreateQuery<Component>("Components");
                }
                return this._Components;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<CustomMeta> CustomMetas
        {
            get
            {
                if (this._CustomMetas == null)
                {
                    this._CustomMetas = base.CreateQuery<CustomMeta>("CustomMetas");
                }
                return this._CustomMetas;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<ItemStat> ItemStats
        {
            get
            {
                if (this._ItemStats == null)
                {
                    this._ItemStats = base.CreateQuery<ItemStat>("ItemStats");
                }
                return this._ItemStats;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Keyword> Keywords
        {
            get
            {
                if (this._Keywords == null)
                {
                    this._Keywords = base.CreateQuery<Keyword>("Keywords");
                }
                return this._Keywords;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<PageContent> PageContents
        {
            get
            {
                if (this._PageContents == null)
                {
                    this._PageContents = base.CreateQuery<PageContent>("PageContents");
                }
                return this._PageContents;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Page> Pages
        {
            get
            {
                if (this._Pages == null)
                {
                    this._Pages = base.CreateQuery<Page>("Pages");
                }
                return this._Pages;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Publication> Publications
        {
            get
            {
                if (this._Publications == null)
                {
                    this._Publications = base.CreateQuery<Publication>("Publications");
                }
                return this._Publications;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Rating> Ratings
        {
            get
            {
                if (this._Ratings == null)
                {
                    this._Ratings = base.CreateQuery<Rating>("Ratings");
                }
                return this._Ratings;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Schema> Schemas
        {
            get
            {
                if (this._Schemas == null)
                {
                    this._Schemas = base.CreateQuery<Schema>("Schemas");
                }
                return this._Schemas;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<StructureGroup> StructureGroups
        {
            get
            {
                if (this._StructureGroups == null)
                {
                    this._StructureGroups = base.CreateQuery<StructureGroup>("StructureGroups");
                }
                return this._StructureGroups;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<Template> Templates
        {
            get
            {
                if (this._Templates == null)
                {
                    this._Templates = base.CreateQuery<Template>("Templates");
                }
                return this._Templates;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DataServiceQuery<User> Users
        {
            get
            {
                if (this._Users == null)
                {
                    this._Users = base.CreateQuery<User>("Users");
                }
                return this._Users;
            }
        }
    }
}

