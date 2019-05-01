using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RaceDay.Services.Models
{
    public class JsonUser
    {
        [Display(Name="User ID")]
        public string UserId { get; set; }

        [Display(Name="Name")]
        public string Name { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime LastUpdate { get; set; }
    }
}