namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;

    [DataServiceKey("Id")]
    public class User
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Comment> _Comments = new Collection<Comment>();
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _EmailAddress;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _ExternalId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Id;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Name;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Collection<Rating> _Ratings = new Collection<Rating>();

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static User CreateUser(string ID)
        {
            return new User { Id = ID };
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
        public string EmailAddress
        {
            get
            {
                return this._EmailAddress;
            }
            set
            {
                this._EmailAddress = value;
            }
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string ExternalId
        {
            get
            {
                return this._ExternalId;
            }
            set
            {
                this._ExternalId = value;
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
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
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
    }
}

