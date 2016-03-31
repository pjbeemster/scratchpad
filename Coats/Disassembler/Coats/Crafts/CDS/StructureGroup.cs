namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "Id", "PublicationId" })]
    public class StructureGroup
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<StructureGroup> _Children = new Collection<StructureGroup>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Depth;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Directory;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Id;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Page> _Pages = new Collection<Page>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private StructureGroup _Parent;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Title;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static StructureGroup CreateStructureGroup(int ID, int publicationId, string title, string directory, int depth)
        {
            return new StructureGroup { 
                Id = ID,
                PublicationId = publicationId,
                Title = title,
                Directory = directory,
                Depth = depth
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<StructureGroup> Children
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
        public string Directory
        {
            get
            {
                return this._Directory;
            }
            set
            {
                this._Directory = value;
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
        public StructureGroup Parent
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

