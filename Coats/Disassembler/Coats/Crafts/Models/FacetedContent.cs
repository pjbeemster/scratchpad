namespace Coats.Crafts.Models
{
    using System;

    public class FacetedContent : FacetedContentBase
    {
        private ComponentSearchSection _componentSection;

        public override ComponentSearchSection ComponentSection
        {
            get
            {
                if (this._componentSection == null)
                {
                    this._componentSection = new ComponentSearchSection();
                }
                return this._componentSection;
            }
            set
            {
                this._componentSection = value;
            }
        }
    }
}

