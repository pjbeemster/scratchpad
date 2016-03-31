namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Services.Common;

    [DataServiceKey("Id")]
    public class CustomMeta
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.Component _Component;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime? _DateValue;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private decimal? _FloatValue;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Id;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemType;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _KeyName;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.Keyword _Keyword;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.Page _Page;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _StringValue;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static CustomMeta CreateCustomMeta(int ID, int itemId, int itemType, int publicationId, string keyName)
        {
            return new CustomMeta { 
                Id = ID,
                ItemId = itemId,
                ItemType = itemType,
                PublicationId = publicationId,
                KeyName = keyName
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
        public DateTime? DateValue
        {
            get
            {
                return this._DateValue;
            }
            set
            {
                this._DateValue = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public decimal? FloatValue
        {
            get
            {
                return this._FloatValue;
            }
            set
            {
                this._FloatValue = value;
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
        public string KeyName
        {
            get
            {
                return this._KeyName;
            }
            set
            {
                this._KeyName = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Coats.Crafts.CDS.Keyword Keyword
        {
            get
            {
                return this._Keyword;
            }
            set
            {
                this._Keyword = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Coats.Crafts.CDS.Page Page
        {
            get
            {
                return this._Page;
            }
            set
            {
                this._Page = value;
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
        public string StringValue
        {
            get
            {
                return this._StringValue;
            }
            set
            {
                this._StringValue = value;
            }
        }
    }
}

