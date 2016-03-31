namespace Coats.Crafts.Repositories.Interfaces
{
    using System;

    public interface IRegistrationRepository
    {
        bool checkDisplayNameExists(string displayname);
        bool checkEmailAddressExists(string RegisteredEmailAddress);
        bool SaveRegisterData(string RegisteredEmailAddress, string IPAdressofRegister, string IPAdressofConfirmer);
    }
}

