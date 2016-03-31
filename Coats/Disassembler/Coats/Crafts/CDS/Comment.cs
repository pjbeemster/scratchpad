namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Services.Common;

    [DataServiceKey("Id")]
    public class Comment
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Content;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private DateTime _CreationDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private long _Id;
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
        private DateTime? _ModeratedDate;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Moderator;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _Score;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Status;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.User _User;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static Comment CreateComment(long ID, int itemPublicationId, int itemId, int itemType, DateTime creationDate, DateTime lastModifiedDate, int status)
        {
            return new Comment { 
                Id = ID,
                ItemPublicationId = itemPublicationId,
                ItemId = itemId,
                ItemType = itemType,
                CreationDate = creationDate,
                LastModifiedDate = lastModifiedDate,
                Status = status
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Content
        {
            get
            {
                return this._Content;
            }
            set
            {
                this._Content = value;
            }
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
        public long Id
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
        public DateTime? ModeratedDate
        {
            get
            {
                return this._ModeratedDate;
            }
            set
            {
                this._ModeratedDate = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string Moderator
        {
            get
            {
                return this._Moderator;
            }
            set
            {
                this._Moderator = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int? Score
        {
            get
            {
                return this._Score;
            }
            set
            {
                this._Score = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
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

