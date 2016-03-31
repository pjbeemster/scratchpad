namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class PasswordReminder
    {
        [DataType(DataType.EmailAddress), CustomEmail("ReminderEmailAddress"), CustomResourceRequired("ReminderEmailAddressRequired")]
        public string ReminderEmailAddress { get; set; }
    }
}

