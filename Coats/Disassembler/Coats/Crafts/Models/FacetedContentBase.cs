namespace Coats.Crafts.Models
{
    using Coats.Crafts.FASWebService;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public abstract class FacetedContentBase
    {
        private Field _brand = new Field();

        protected FacetedContentBase()
        {
        }

        public Field Brand
        {
            get
            {
                return this._brand;
            }
            set
            {
                this._brand = value;
            }
        }

        public PaginatedComponentList ComponentList { get; set; }

        public abstract ComponentSearchSection ComponentSection { get; set; }

        public List<IComponent> FabricComponents { get; set; }

        public FacetCollection FacetMap { get; set; }

        public string FredHopperLocation { get; set; }

        public filter ReturnedSchemas { get; set; }

        public FacetSort Sort { get; set; }
    }
}
