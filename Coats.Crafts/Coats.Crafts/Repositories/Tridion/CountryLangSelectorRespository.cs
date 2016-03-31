using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Core.Logging;
using Coats.Crafts.Data;
using Tridion.ContentDelivery.UGC.WebService;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Web.Mvc;
using Coats.Crafts.Gateway;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Coats.Crafts.Configuration;

namespace Coats.Crafts.Repositories.Tridion
{
    public class CountryLangSelectorRepository : ICountryLangSelectorRepository
    {
        public ILogger Logger { get; set; }
        private IAppSettings _settings;

        public CountryLangSelectorRepository(IAppSettings settings)
        {
            _settings = settings;
        }

    }
}