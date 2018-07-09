using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CrimeWatch.Models;
using System.Net.Mail;

namespace CrimeWatch.Controllers
{
    /// <summary>
    /// Handles user requests related to login, registration and account-related functions
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }
        /// <summary>
        /// Auto-generated code from Asp.NET identify framework
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
       

        /// <summary>
        /// Auto-generated code from Asp.NET identify framework.
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        /// <summary>
        /// Auto-generated code from Asp.NET identify framework.
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Handles user request to navigate to login page
        /// </summary>
        /// <returns>Login View.</returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        
        /// <summary>
        /// Receives login credentials provided by the user, validates input 
        ///     and performs corresponding actions.
        /// </summary>
        /// <param name="model"> User credentials.</param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, String returnUrl)
        {
            //Validates user input.
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Checks the result of user details.
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                //If the user is successfully signed redirect to User Portal
                case SignInStatus.Success:
                    return RedirectToAction("MyPortal", "Home");
                //Otherwise, outpput appropriate error
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        /// <summary>
        /// Redirects to Register View
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Sends a confirmation email to the provided email
        /// </summary>
        /// <returns>Redirection to User Portal View</returns>
        public async Task<ActionResult> SendConfirmationEmail()
        {
            //Gets the logged in user
            var user = _db.AspNetUsers.Find(User.Identity.GetUserId());
            //Generates unique Id to authenitcate the account
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //Forms the link sent to the email address
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
            //Sends the email
            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Dear user, <br><br>Please confirm your CrimeWatch account by clicking <a href=\"" + callbackUrl + "\">here</a>.<br><br>If you are not fammiliar with this message please ignore it. <br><br> Thanks,<br>The CrimeWatch team");
            return RedirectToAction("MyPortal", "Home");
        }

        /// <summary>
        /// Called when registration form is submitted. If details are valid sends email to the user. Otherwise outputs error.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>User data passed via registration list</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Email = model.Email,  FullName = model.FullName, UserName = model.Email, PhoneNumber = String.IsNullOrEmpty(model.PhoneNumber)?"":model.PhoneNumber};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Register user to the system.
                    await SignInManager.SignInAsync(user, false, false);
                    //If provided data are valid, send confirmation email.
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Dear user, <br><br>Please confirm your CrimeWatch account by clicking <a href=\"" + callbackUrl + "\">here</a>.<br><br>If you are not fammiliar with this message please ignore it. <br><br> Thanks,<br>The CrimeWatch team");
                    return RedirectToAction("MyPortal", "Home");
                }
                //If data are not valid outputs error.
                AddErrors(result);
            }
            // If we got this far, something failed, redisplay form.
            return View(model);
        }


        /// <summary>
        /// Called when confirmation link in the email is pressed.
        /// </summary>
        /// <param name="userId">The Id of the logged in user</param>
        /// <param name="code">The unique token to confirm email</param>
        /// <returns>Redirection to User Portal method</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            //If user Id or code is null return error
            if (userId == null || code == null)
            {
                return View("Error");
            }
            //Confirms email using the unique token
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (!result.Succeeded)
            {
                return View("Error");
            }
            return RedirectToAction("MyPortal", "Home");
        }

        
     
        /// <summary>
        /// Logs the user off the system.
        /// </summary>
        /// <returns>Redirects to home page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //Signs the user out
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }     

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Deletes the account of the user.
        /// </summary>
        /// <param name="email">Email of the user to be deleted.</param>
        /// <returns></returns>
        public ActionResult DeleteUser(String email)
        {
            try
            {
                foreach (var user in _db.AspNetUsers)
                {
                    if (user.Email == email)
                    {
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        _db.AspNetUsers.Remove(user);
                    }
                }
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("Error");
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Redirects to ChangeLoginDetails view
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeLoginDetails()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeLoginDetails(ChangeLoginDetailsViewModel model)
        {
            var formValidationResult = ValidateFormInput(model);
            if (formValidationResult == "ok")
            {
                if (!String.IsNullOrEmpty(model.OldPassword))
                {
                    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        if (!String.IsNullOrEmpty(model.OldEmail))
                        {
                            var user = _db.AspNetUsers.Find(User.Identity.GetUserId());
                            if (user.Email == model.OldEmail)
                            {
                                if (IsValid(model.NewEmail))
                                {
                                    user.Email = model.NewEmail;
                                    user.UserName = model.NewEmail;
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    ModelState.AddModelError(String.Empty, "The new email does not have the correct format.");
                                    return View();
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, "The email you provided is incorrect.");
                                return View();
                            }
                        }
                        return RedirectToAction("MyPortal", "Home", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                        return View(model);
                    }
                }
                else
                {
                    var user = _db.AspNetUsers.Find(User.Identity.GetUserId());
                    if (user.Email == model.OldEmail)
                    {
                        if (IsValid(model.NewEmail))
                        {
                            user.Email = model.NewEmail;
                            user.UserName = model.NewEmail;
                            _db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "The new email does not have the correct format.");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "The email you provided is incorrect.");
                        return View();
                    }
                    return RedirectToAction("MyPortal", "Home", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, formValidationResult);
                return View();
            }
        }

        public String ValidateFormInput(ChangeLoginDetailsViewModel model)
        {
            if (!ModelState.IsValid || (String.IsNullOrEmpty(model.OldEmail) && String.IsNullOrEmpty(model.OldPassword)))
            {
                return "Either current password or email must be provided.";
            }
            if (!String.IsNullOrEmpty(model.OldPassword))
            {
                if (String.IsNullOrEmpty(model.NewPassword) || String.IsNullOrEmpty(model.ConfirmPassword))
                {
                    return "Both new and confirmed passwords must be provided.";
                }
            }
            if (!String.IsNullOrEmpty(model.OldEmail))
            {
                if (String.IsNullOrEmpty(model.NewEmail) || String.IsNullOrEmpty(model.ConfirmEmail))
                {
                    return "Both new and confirmed emails must be provided.";
                }
            }
            return "ok";
        }

        public bool IsValid(string emailaddress)
        {
            try
            {
                var m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public ActionResult ChangeContactDetails()
        {
            var user = _db.AspNetUsers.Find(User.Identity.GetUserId());
            ViewBag.currentName = user.FullName;
            ViewBag.currentPhone = user.PhoneNumber;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeContactDetails(ChangeContactDetailsViewModel model)
        {
            if (String.IsNullOrEmpty(model.NewFullName) && String.IsNullOrEmpty(model.NewPhoneNumber))
            {
                ModelState.AddModelError(String.Empty, "No details provided.");
                return View();
            }

            if (!String.IsNullOrEmpty(model.NewFullName))
            {
                _db.AspNetUsers.Find(User.Identity.GetUserId()).FullName = model.NewFullName;
            }

            if (!String.IsNullOrEmpty(model.NewPhoneNumber))
            {
                _db.AspNetUsers.Find(User.Identity.GetUserId()).PhoneNumber = model.NewPhoneNumber;
            }

            _db.SaveChanges();
            return RedirectToAction("MyPortal", "Home");

        }
 
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}