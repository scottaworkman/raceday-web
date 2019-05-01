using RaceDay.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RaceDay.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "*Invalid Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "*Required")]
        [StringLength(100)]
        public string RegisterEmail { get; set; }

        public string RecaptchaSiteKey { get; set; }
        public ForgotPasswordViewModel()
        {
            RecaptchaSiteKey = RecaptchaConfiguration.Instance.SiteKey;
        }
    }
}