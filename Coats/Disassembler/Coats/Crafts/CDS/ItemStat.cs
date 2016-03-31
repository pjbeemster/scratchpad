namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "PublicationId", "Id", "Type" })]
    public class ItemStat
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private double? _AverageRating;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Comment> _Comments = new Collection<Comment>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Id;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _NumberOfComments;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int? _NumberOfRatings;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Rating> _Ratings = new Collection<Rating>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _Type;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static ItemStat CreateItemStat(int publicationId, int ID, int type)
        {
            return new ItemStat { 
                PublicationId = publicationId,
                Id = ID,
                Type = type
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public double? AverageRating
        {
            get
            {
                return this._AverageRating;
            }
            set
            {
                this._AverageRating = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public Collection<Comment> Comments
        {
            get
            {
                return this._Comments;
            }
            set
            {
                if (value != null)
                {
                    this._Comments = value;
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
        public int? NumberOfComments
        {
            get
            {
                return this._NumberOfComments;
            }
            set
            {
                this._NumberOfComments = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int? NumberOfRatings
        {
            get
            {
                return this._NumberOfRatings;
            }
            set
            {
                this._NumberOfRatings = value;
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
        public Collection<Rating> Ratings
        {
            get
            {
                return this._Ratings;
            }
            set
            {
                if (value != null)
                {
                    this._Ratings = value;
                }
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public int Type
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

