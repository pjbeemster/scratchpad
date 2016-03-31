using Coats.Crafts.Models;

namespace Coats.Crafts.Interfaces
{
    public interface IUserProfile
    {
        Address AddressDetails { get; set; }
        //Administrators Administrator { get; set; }
        Customer CustomerDetails { get; set; }
    }
}