namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "PublicationId", "SchemaId" })]
    public class Schema
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Component> _Components = new Collection<Component>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _SchemaId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Title;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Schema CreateSchema(int publicationId, int schemaId, string title)
        {
            return new Schema { 
                PublicationId = publicationId,
                SchemaId = schemaId,
                Title = title
            };
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
        public int SchemaId
        {
            get
            {
                return this._SchemaId;
            }
            set
            {
                this._SchemaId = value;
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

