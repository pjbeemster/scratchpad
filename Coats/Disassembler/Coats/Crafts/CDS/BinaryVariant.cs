namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "BinaryId", "PublicationId", "VariantId" })]
    public class BinaryVariant
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.Binary _Binary;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _BinaryId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Description;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private bool _IsComponent;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Path;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Type;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _URLPath;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _VariantId;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static BinaryVariant CreateBinaryVariant(int binaryId, int publicationId, string description, bool isComponent, string path, string uRLPath, string variantId, string type)
        {
            return new BinaryVariant { 
                BinaryId = binaryId,
                PublicationId = publicationId,
                Description = description,
                IsComponent = isComponent,
                Path = path,
                URLPath = uRLPath,
                VariantId = variantId,
                Type = type
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Coats.Crafts.CDS.Binary Binary
        {
            get
            {
                return this._Binary;
            }
            set
            {
                this._Binary = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int BinaryId
        {
            get
            {
                return this._BinaryId;
            }
            set
            {
                this._BinaryId = value;
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
        public bool IsComponent
        {
            get
            {
                return this._IsComponent;
            }
            set
            {
                this._IsComponent = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Path
        {
            get
            {
                return this._Path;
            }
            set
            {
                this._Path = value;
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
        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string URLPath
        {
            get
            {
                return this._URLPath;
            }
            set
            {
                this._URLPath = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string VariantId
        {
            get
            {
                return this._VariantId;
            }
            set
            {
                this._VariantId = value;
            }
        }
    }
}

