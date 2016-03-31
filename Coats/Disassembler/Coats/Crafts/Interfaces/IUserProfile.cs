namespace Coats.Crafts.Interfaces
{
    using Coats.Crafts.Models;
    using System;

    public interface IUserProfile
    {
        Address AddressDetails { get; set; }

        Customer CustomerDetails { get; set; }
    }
}

