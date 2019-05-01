using RaceDay.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RaceDay.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "*Required")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "*Required")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "*Invalid Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "*Required")]
        [StringLength(100)]
        public string RegisterEmail { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "*Required")]
        [StringLength(20)]
        public string RegisterPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "*Required")]
        [StringLength(20)]
        [Compare("RegisterPassword", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Group Code")]
        [Required(ErrorMessage = "Required")]
        [StringLength(10)]
        public string GroupCode { get; set; }

        public string RecaptchaSiteKey { get; set; }
        public RegisterViewModel()
        {
            RecaptchaSiteKey = RecaptchaConfiguration.Instance.SiteKey;
        }
    }
}