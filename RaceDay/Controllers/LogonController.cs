using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using RaceDay.Services;
using RaceDay.ViewModels;
using RaceDay.Utilities;

namespace RaceDay.Controllers
{
    [HandleError(View = "Error")]
    public partial class LogonController : BaseController

    {
        // GET: Logon
        //
        // Display Logon form
        //
        public virtual ActionResult Index(string returnUrl)
        {
            LogonViewModel model = new LogonViewModel();
            model.ReturnUrl = returnUrl;

            // If user is already authenticated (either through navigation or by Remember Me), then direct to a content page
            //
            if (Request.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // POST: Logon
        //
        // Attempt to log on the user
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(LogonViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt a login
                //
                var login = await RaceDayClient.Login(model.LoginEmail.Trim(), model.LoginPassword.Trim());

                if (login == null)
                {
                    model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.error, "Invalid Email/Password.");
                }
                else
                {
                    RaceDayUser.LoginUser(login, model.RememberMe);
                    return RedirectToAction(MVC.Home.Index());
                }
            }
            return View(model);
        }

        // GET: /Logon/Register
        //
        // Display registration form
        //
        public virtual ActionResult Register()
        {
            var model = new RegisterViewModel();

            return View(model);
        }

        // POST: /Logon/Register
        //
        // Process a new registration
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string recapatchaResponse = Request.Form["g-recaptcha-response"];
                var result = await RecaptchaClient.Verify(RecaptchaConfiguration.Instance.SecretKey, recapatchaResponse);
                if (result.success)
                {
                    var resultCode = await RaceDayClient.UserRegister(model.GroupCode, model.FirstName, model.LastName, model.RegisterEmail, model.RegisterPassword);
                    if (resultCode == HttpStatusCode.Created)
                    {
                        var login = await RaceDayClient.Login(model.RegisterEmail.Trim(), model.RegisterPassword.Trim());
                        RaceDayUser.LoginUser(login, false);

                        return RedirectToAction(MVC.Home.Index());
                    }
                    else if (resultCode == HttpStatusCode.BadRequest)
                    {
                        model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.error, "Invalid registration request.");
                    }
                    else if (resultCode == HttpStatusCode.Conflict)
                    {
                        model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.error, "Account already exists.");
                    }
                    else
                    {
                        model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.error, "API Server error.");
                    }
                }
                else
                {
                    model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.error, "Invalid captcha response.");
                }
            }

            return View(model);
        }

        public virtual ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string recapatchaResponse = Request.Form["g-recaptcha-response"];
                var result = await RecaptchaClient.Verify(RecaptchaConfiguration.Instance.SecretKey, recapatchaResponse);
                if (result.success)
                {
                    var statusCode = await RaceDayClient.ForgotPassword(model.RegisterEmail);
                    if (statusCode == HttpStatusCode.OK)
                    {
                        model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.success, "Check your email for your account password.<br />You may change your password after you have <a href=\"/raceday\">logged on</a>.");
                    }
                    else
                    { 
                        model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.error, "Could not identify your account.<br />Use the <a href=\"/Logon/Register\">Register Account</a> link to create an account if you have not yet registered.");
                    }
                }
                else
                {
                    model.PageMessage = new PageMessageModel(MessageDismissEnum.none, CssMessageClassEnum.error, "Invalid captcha response.");
                }
            }

            return View(model);
        }
        public virtual ActionResult Logoff()
        {
            RaceDayUser.LogoffUser();
            return RedirectToAction("Index", "Home", null);
        }

    }
}