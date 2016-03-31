namespace Coats.Crafts.Models
{
    using System;

    public class ProductExplorer : FacetedContentBase
    {
        private ExtComponentSearchSection _extComponentSection;

        public override ComponentSearchSection ComponentSection
        {
            get
            {
                return this.ExtComponentSection;
            }
            set
            {
                this.ExtComponentSection.ComponentTypes = value.ComponentTypes;
                this.ExtComponentSection.SearchTerm = value.SearchTerm;
            }
        }

        public ExtComponentSearchSection ExtComponentSection
        {
            get
            {
                if (this._extComponentSection == null)
                {
                    this._extComponentSection = new ExtComponentSearchSection();
                }
                return this._extComponentSection;
            }
            set
            {
                this._extComponentSection = value;
            }
        }
    }
}

