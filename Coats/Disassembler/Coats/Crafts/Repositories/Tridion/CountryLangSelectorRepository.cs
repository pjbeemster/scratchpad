namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.Runtime.CompilerServices;

    public class CountryLangSelectorRepository : ICountryLangSelectorRepository
    {
        private IAppSettings _settings;

        public CountryLangSelectorRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        public ILogger Logger { get; set; }
    }
}

