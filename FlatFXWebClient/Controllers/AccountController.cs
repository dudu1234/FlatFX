using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FlatFXCore.Model.User;
using FlatFXCore.Model.Core;
using System.Web.Security;
using FlatFXWebClient.ViewModels;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Data;
using System.Collections.Generic;

namespace FlatFXWebClient.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        /// <summary>
        /// Change the current culture.
        /// </summary>
        /// <param name="culture">The current selected culture.</param>
        /// <returns>The action result.</returns>
        [AllowAnonymous]
        public ActionResult ChangeCurrentCulture(string culture)
        {
            FlatFXCookie.SetCookie("lang", culture);
            Session["lang"] = culture;

            // Redirect to the same page from where the request was made! 
            return Redirect(Request.UrlReferrer.ToString());
        }

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        // User logined succesfully ==> create a new site session!
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        ApplicationUser user = _db.Users.FirstOrDefault(u => u.UserName == model.UserName);
                        FlatFXSession siteSession = new FlatFXSession(_db, user);
                        Session["SiteSession"] = siteSession; // Cache the user login data!

                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session["SiteSession"] = null;
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
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

        [HttpPost]
        [AllowAnonymous]
        public JsonResult isFieldUnique(string UserName)
        {
            // GUY how to analize the POST request parameters ?
            string paramNames = "";
            foreach (string key in Request.Form.Keys)
                paramNames += key + ";";

            if (Request["UserName"] != null)
                return Json(IsUnique("UserName", Request["UserName"]));
            else if (Request["CompanyShortName"] != null)
                return Json(IsUnique("CompanyShortName", Request["CompanyShortName"]));
            else if (Request["CompanyFullName"] != null)
                return Json(IsUnique("CompanyFullName", Request["CompanyFullName"]));
            else if (Request["Email"] != null)
                return Json(IsUnique("UserEmail", Request["Email"]));
            else if (Request["AccountName"] != null)
                return Json(IsUnique("AccountName", Request["AccountName"]));
            else if (Request["AccountFullName"] != null)
                return Json(IsUnique("AccountFullName", Request["AccountFullName"]));
            else
                return Json(false);
        }
        private bool IsUnique(string fieldName, string fieldValue)
        {
            if (fieldName == "UserName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                var user = Membership.GetUser(fieldValue);
                return (user == null);
            }
            else if (fieldName == "UserEmail")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !_db.Users.Where(u => u.Email.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "CompanyEmail")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !_db.Companies.Where(c => c.ContactDetails.Email.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "CompanyShortName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !_db.Companies.Where(c => c.CompanyShortName.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "CompanyFullName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !_db.Companies.Where(c => c.CompanyFullName.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "AccountName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !_db.CompanyAccounts.Where(a => a.AccountName.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "AccountFullName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !_db.CompanyAccounts.Where(a => a.AccountFullName.ToLower() == fieldValue.ToLower()).Any();
            }

            return false;
        }
        private bool IsUnique(string fieldName, object viewModel)
        {
            if (fieldName == "ProviderAccountName" && viewModel != null)
            {
                if (viewModel == null || !(viewModel is RegisterProviderAccountViewModel))
                    return false;
                return !_db.ProviderAccounts.Where(pa => pa.ProviderId == (viewModel as RegisterProviderAccountViewModel).ProviderId &&
                    pa.BankAccountName.Trim().ToLower() == (viewModel as RegisterProviderAccountViewModel).AccountName.Trim().ToLower()).Any();
            }
            else if (fieldName == "ProviderAccountNumber" && viewModel != null)
            {
                if (viewModel == null || !(viewModel is RegisterProviderAccountViewModel))
                    return false;
                return !_db.ProviderAccounts.Where(pa => pa.ProviderId ==
                    (viewModel as RegisterProviderAccountViewModel).ProviderId &&
                    pa.BankAccountNumber.Trim().ToLower() == (viewModel as RegisterProviderAccountViewModel).AccountNumber.Trim().ToLower() &&
                    pa.BankBranchNumber.Trim().ToLower() == (viewModel as RegisterProviderAccountViewModel).BranchNumber.Trim().ToLower()).Any();
            }

            return false;
        }

        #region Register
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (ViewBag.ProviderList == null)
            {
                Dictionary<string, string> Providers = new Dictionary<string, string>();
                if (_db.Providers.Where(p => p.IsActive).Any())
                    Providers = _db.Providers.Where(p => p.IsActive).ToDictionary(p1 => p1.ProviderId, p2 => p2.FullName);
                ViewBag.ProviderList = Providers;
            }

            RegisterCompanyEntitiesModelView registerCompanyEntitiesModelView = new RegisterCompanyEntitiesModelView();
            return View(registerCompanyEntitiesModelView);
            
            //return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterCompanyEntitiesModelView model)
        {
            // GUY : why does the model is empty (all members are null)
            IdentityResult result = null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                #region chack if model is valid
                if (!IsUnique("UserName", model.UserVM.UserName.Trim()))
                    ModelState.AddModelError("", "UserName is not unique.");
                if (!IsUnique("UserEmail", model.UserVM.userContactDetails.Email.Trim()))
                    ModelState.AddModelError("", "User email is not unique.");
                if (!IsUnique("CompanyShortName", model.companyVM.CompanyShortName.Trim()))
                    ModelState.AddModelError("", "Company short name is not unique.");
                if (!IsUnique("CompanyFullName", model.companyVM.CompanyFullName.Trim()))
                    ModelState.AddModelError("", "Company full name is not unique.");
                if (!IsUnique("AccountName", model.companyVM.AccountName.Trim()))
                    ModelState.AddModelError("", "Account name is not unique.");
                if (!IsUnique("AccountFullName", model.companyVM.AccountFullName.Trim()))
                    ModelState.AddModelError("", "Account full name is not unique.");
                if (!IsUnique("ProviderAccountName", model.providerAccountVM))
                    ModelState.AddModelError("", "Provider account name is not unique.");
                if (!IsUnique("ProviderAccountNumber", model.providerAccountVM))
                    ModelState.AddModelError("", "Provider account number is not unique.");
                #endregion

                //Create User contact details
                ContactDetails contactDetails = new ContactDetails();
                contactDetails.OfficePhone = model.UserVM.userContactDetails.OfficePhone.Trim();
                contactDetails.Fax = model.UserVM.userContactDetails.Fax.Trim();
                contactDetails.Country = model.UserVM.userContactDetails.Country;
                contactDetails.WebSite = model.UserVM.userContactDetails.WebSite.Trim();
                contactDetails.MobilePhone = model.UserVM.userContactDetails.MobilePhone.Trim();

                //Create User
                var user = new ApplicationUser();
                user.UserName = model.UserVM.UserName.Trim();
                user.FirstName = model.UserVM.FirstName.Trim();
                user.LastName = model.UserVM.LastName.Trim();
                user.Email = model.UserVM.userContactDetails.Email.Trim();
                user.RoleInCompany = model.UserVM.Role;
                user.PhoneNumber = model.UserVM.userContactDetails.MobilePhone.Trim();
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.Status = FlatFXCore.BussinessLayer.Consts.eUserStatus.Active;
                user.UserRole = FlatFXCore.BussinessLayer.Consts.UserRoles.Administrator;
                user.Language = Consts.eLanguage.English;
                user.SigningKey = Guid.NewGuid().ToString().Substring(0, 8);
                user.ContactDetails = contactDetails;
                
                result = await UserManager.CreateAsync(user, model.UserVM.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    //Create company contact details
                    ContactDetails companyDetails = new ContactDetails();
                    companyDetails.Email = model.companyVM.companyContactDetails.Email.Trim();
                    companyDetails.MobilePhone = model.companyVM.companyContactDetails.MobilePhone.Trim();
                    companyDetails.OfficePhone = model.companyVM.companyContactDetails.OfficePhone.Trim();
                    companyDetails.Fax = model.companyVM.companyContactDetails.Fax.Trim();
                    companyDetails.Country = model.companyVM.companyContactDetails.Country;
                    companyDetails.WebSite = model.companyVM.companyContactDetails.WebSite.Trim();
                    companyDetails.Address = model.companyVM.companyContactDetails.contactDetailsEx.Address.Trim();
                    companyDetails.Email2 = model.companyVM.companyContactDetails.contactDetailsEx.Email2.Trim();
                    companyDetails.MobilePhone2 = model.companyVM.companyContactDetails.contactDetailsEx.MobilePhone2.Trim();
                    companyDetails.OfficePhone2 = model.companyVM.companyContactDetails.contactDetailsEx.OfficePhone2.Trim();
                    companyDetails.HomePhone = model.companyVM.companyContactDetails.contactDetailsEx.HomePhone.Trim();
                    companyDetails.CarPhone = model.companyVM.companyContactDetails.contactDetailsEx.CarPhone.Trim();
                    
                    //Add company
                    var company = new Company();
                    company.ContactDetails = companyDetails;
                    company.CreatedAt = DateTime.Now;
                    company.IsActive = true;
                    company.Status = Consts.eCompanyStatus.Active;
                    company.CompanyFullName = model.companyVM.CompanyFullName.Trim();
                    company.CompanyId = Guid.NewGuid().ToString();
                    company.CompanyShortName = model.companyVM.CompanyShortName.Trim();
                    company.CompanyVolumePerYearUSD = model.companyVM.CompanyVolumePerYearUSD;
                    company.CustomerType = model.companyVM.CustomerType;
                    company.IsDepositValid = false;
                    company.IsSignOnRegistrationAgreement = false;
                    company.LastUpdate = DateTime.Now;
                    company.ValidIP = model.companyVM.ValidIP;
                    _db.Companies.Add(company);

                    //Add company account
                    var companyAccount = new CompanyAccount();
                    companyAccount.AccountFullName = model.companyVM.AccountFullName.Trim();
                    companyAccount.AccountName = model.companyVM.AccountName.Trim();
                    companyAccount.CompanyAccountId = Guid.NewGuid().ToString();
                    companyAccount.CompanyId = company.CompanyId;
                    companyAccount.IsActive = true;
                    companyAccount.IsDefaultAccount = true;
                    _db.CompanyAccounts.Add(companyAccount);

                    //Add provider account
                    var providerAccount = new ProviderAccount();
                    providerAccount.ApprovedBYFlatFX = false;
                    providerAccount.ApprovedBYProvider = false;
                    providerAccount.BankAccountFullName = model.providerAccountVM.AccountFullName.Trim();
                    providerAccount.BankAccountName = model.providerAccountVM.AccountName.Trim();
                    providerAccount.BankAccountNumber = model.providerAccountVM.AccountNumber.Trim();
                    providerAccount.BankAddress = model.providerAccountVM.Address.Trim();
                    providerAccount.BankBranchNumber = model.providerAccountVM.BranchNumber.Trim();
                    providerAccount.CompanyAccountId = companyAccount.CompanyAccountId;
                    providerAccount.CreatedAt = DateTime.Now;
                    providerAccount.IBAN = model.providerAccountVM.IBAN;
                    providerAccount.IsActive = true;
                    //providerAccount.IsDemoAccount = true;
                    providerAccount.LastUpdate = DateTime.Now;
                    providerAccount.LastUpdateBy = User.Identity.Name;
                    providerAccount.ProviderId = model.providerAccountVM.ProviderId;
                    providerAccount.QuoteResponse_CustomerPromil = null;
                    providerAccount.QuoteResponse_IsBlocked = false;
                    providerAccount.SWIFT = model.providerAccountVM.SWIFT;
                    _db.ProviderAccounts.Add(providerAccount);

                    _db.SaveChanges();

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            if (result != null)
                AddErrors(result);

            return View(model);
        }

        #endregion

        #region Register Demo
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult RegisterDemo()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterDemo(RegisterDemoUserModelView model)
        {
            IdentityResult result = null;
            ContactDetails contactDetails = null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                #region chack if model is valid
                if (!IsUnique("UserName", model.Email.Trim()))
                    ModelState.AddModelError("", "User Name is not unique.");
                if (!IsUnique("UserEmail", model.Email.Trim()))
                    ModelState.AddModelError("", "User Email is not unique.");
                #endregion

                //Create User
                var user = new ApplicationUser();
                user.UserName = model.Email.Trim();
                user.FirstName = model.Email.Trim().Substring(0, model.Email.Trim().IndexOf('@'));
                user.LastName = "Demo";
                user.Email = model.Email.Trim();
                user.RoleInCompany = "";
                user.PhoneNumber = (model.MobilePhone == null)? "" : model.MobilePhone.Trim();
                contactDetails = new ContactDetails();
                user.ContactDetails = contactDetails;
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.Status = FlatFXCore.BussinessLayer.Consts.eUserStatus.Active;
                user.UserRole = FlatFXCore.BussinessLayer.Consts.UserRoles.CompanyDemoUser;
                user.Language = Consts.eLanguage.English;
                user.SigningKey = Guid.NewGuid().ToString().Substring(0, 8);

                result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    //Add the demo user to the Demo company
                    Company demoCompany = _db.Companies.Where(c => c.CompanyShortName == "Demo").SingleOrDefault();
                    if (demoCompany != null)
                    {
                        var u = _db.Users.First(p => p.Email == user.Email);
                        demoCompany.Users.Add(u);
                        _db.SaveChanges();
                    }

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            if (result != null)
                AddErrors(result);

            return View(model);
        }
        #endregion
    }
}