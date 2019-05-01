using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RaceDay.ViewModels
{
	public class LogonViewModel : BaseViewModel
	{
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "*Invalid Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "*Required")]
        [StringLength(100)]
        public string LoginEmail { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "*Required")]
        [StringLength(20)]
        public string LoginPassword { get; set; }

        [Display(Name = "Remember Me")]
        public Boolean RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}