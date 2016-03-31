namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Services.Common;

    [DataServiceKey("Id")]
    public class Rating
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime _CreationDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Id;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemPublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.ItemStat _ItemStat;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _ItemType;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime _LastModifiedDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _RatingValue;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.User _User;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Rating CreateRating(string ID, string ratingValue, int itemPublicationId, int itemId, int itemType, DateTime creationDate, DateTime lastModifiedDate)
        {
            return new Rating { 
                Id = ID,
                RatingValue = ratingValue,
                ItemPublicationId = itemPublicationId,
                ItemId = itemId,
                ItemType = itemType,
                CreationDate = creationDate,
                LastModifiedDate = lastModifiedDate
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public DateTime CreationDate
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
        public string Id
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
        public int ItemPublicationId
        {
            get
            {
                return this._ItemPublicationId;
            }
            set
            {
                this._ItemPublicationId = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Coats.Crafts.CDS.ItemStat ItemStat
        {
            get
            {
                return this._ItemStat;
            }
            set
            {
                this._ItemStat = value;
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
        public DateTime LastModifiedDate
        {
            get
            {
                return this._LastModifiedDate;
            }
            set
            {
                this._LastModifiedDate = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string RatingValue
        {
            get
            {
                return this._RatingValue;
            }
            set
            {
                this._RatingValue = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Coats.Crafts.CDS.User User
        {
            get
            {
                return this._User;
            }
            set
            {
                this._User = value;
            }
        }
    }
}

