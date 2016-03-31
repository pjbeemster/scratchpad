namespace Coats.Crafts.CDS
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Services.Common;

    [DataServiceKey(new string[] { "PageId", "PublicationId" })]
    public class PageContent
    {
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _CharSet;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private string _Content;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private Coats.Crafts.CDS.Page _Page;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PageId;
        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        private int _PublicationId;

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public static PageContent CreatePageContent(int publicationId, int pageId)
        {
            return new PageContent { 
                PublicationId = publicationId,
                PageId = pageId
            };
        }

        [GeneratedCode("System.Data.Services.Design", "1.0.0")]
        public string CharSet
        {
            get
            {
                return this._CharSet;
            }
            set
            {
                this._CharSet = value;
            }
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
        public int PageId
        {
            get
            {
                return this._PageId;
            }
            set
            {
                this._PageId = value;
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
    }
}

