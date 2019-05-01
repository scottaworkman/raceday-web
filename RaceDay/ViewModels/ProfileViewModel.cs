using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RaceDay.ViewModels
{
	public class ProfileViewModel : BaseViewModel
	{
		[Display(Name="User ID")]
		public String UserId { get; set; }

		[Required(ErrorMessage="Name is required")]
		[StringLength(100, ErrorMessage = "Maximum length of 100 characters")]
		[Display(Name="Name")]
		public string Name { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "Maximum length of 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Maximum length of 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length of 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "Maximum length of 50 characters")]
        [MinLength(6, ErrorMessage = "Passwords must be at least 6 characters")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}