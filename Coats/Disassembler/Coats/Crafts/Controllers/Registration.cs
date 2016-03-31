namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Models;
    using System;
    using System.Runtime.CompilerServices;

    public class Registration
    {
        public Coats.Crafts.Controllers.LoginForm LoginForm { get; set; }

        public static string NewsLetterHeader { get; set; }
        public Coats.Crafts.Models.PasswordReminder PasswordReminder { get; set; }

        public Coats.Crafts.Controllers.RegistrationForm RegistrationForm { get; set; }
    }
}

