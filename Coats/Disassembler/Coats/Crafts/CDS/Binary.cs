namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "BinaryId", "PublicationId" })]
    public class Binary
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _BinaryId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<BinaryVariant> _BinaryVariants = new Collection<BinaryVariant>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Type;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Binary CreateBinary(int binaryId, int publicationId, string type)
        {
            return new Binary { 
                BinaryId = binaryId,
                PublicationId = publicationId,
                Type = type
            };
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
    }
}

