using System.ComponentModel.DataAnnotations;
using Coats.Crafts.Validators;
using DataAnnotationsExtensions;
using System.Web.Mvc;
using Coats.Crafts.Attributes;

namespace Coats.Crafts.Models
{
    public class PasswordReminder
    {
        // [Remote("IsValidUser", "RemoteValidation", HttpMethod = "POST")]
        [CustomResourceRequired("ReminderEmailAddressRequired")]
        [DataType(DataType.EmailAddress)]
        [CustomEmail("ReminderEmailAddress")]
        public string ReminderEmailAddress { get; set; }
    }
}
