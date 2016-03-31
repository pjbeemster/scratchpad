using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coats.Crafts.Data;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Web.Mvc;
using DD4T.ContentModel;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface IRegistrationRepository
    {
        bool checkDisplayNameExists(string displayname);
        //Added by Ajaya for double opt
        bool SaveRegisterData(string RegisteredEmailAddress,string IPAdressofRegister,string IPAdressofConfirmer);
        bool checkEmailAddressExists(string RegisteredEmailAddress);
    }
}
	