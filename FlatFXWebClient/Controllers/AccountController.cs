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
using System.Data.Entity;
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
                        ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName == model.UserName);
                        FlatFXSession siteSession = new FlatFXSession(db, user);
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
                return RedirectToAction("UserIndex", "Manage");
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
            string paramNames = "";
            foreach (string key in Request.Form.Keys)
                paramNames += key + ";";

            if (Request["UserVM.UserName"] != null)
                return Json(IsUnique("UserName", Request["UserVM.UserName"]));
            else if (Request["CompanyVM.CompanyShortName"] != null)
                return Json(IsUnique("CompanyShortName", Request["CompanyVM.CompanyShortName"]));
            else if (Request["CompanyVM.CompanyFullName"] != null)
                return Json(IsUnique("CompanyFullName", Request["CompanyVM.CompanyFullName"]));
            else if (Request["UserVM.userContactDetails.Email"] != null)
                return Json(IsUnique("UserEmail", Request["UserVM.userContactDetails.Email"]));
            else if (Request["companyVM.companyContactDetails.Email"] != null)
                return Json(IsUnique("CompanyEmail", Request["companyVM.companyContactDetails.Email"]));
            else if (Request["companyVM.AccountName"] != null)
                return Json(IsUnique("AccountName", Request["companyVM.AccountName"]));
            else if (Request["companyVM.AccountFullName"] != null)
                return Json(IsUnique("AccountFullName", Request["companyVM.AccountFullName"]));
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
                return !db.Users.Where(u => u.Email.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "CompanyEmail")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !db.Companies.Where(c => c.ContactDetails.Email.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "CompanyShortName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !db.Companies.Where(c => c.CompanyShortName.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "CompanyFullName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !db.Companies.Where(c => c.CompanyFullName.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "AccountName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !db.CompanyAccounts.Where(a => a.AccountName.ToLower() == fieldValue.ToLower()).Any();
            }
            else if (fieldName == "AccountFullName")
            {
                if (fieldValue == null || fieldValue == "")
                    return false;
                return !db.CompanyAccounts.Where(a => a.AccountFullName.ToLower() == fieldValue.ToLower()).Any();
            }

            return false;
        }
        private bool IsUnique(string fieldName, object viewModel)
        {
            if (fieldName == "ProviderAccountName" && viewModel != null)
            {
                if (viewModel == null || !(viewModel is ProviderAccountDetailsViewModel))
                    return false;
                string provId = (viewModel as ProviderAccountDetailsViewModel).ProviderId;
                string accName = (viewModel as ProviderAccountDetailsViewModel).AccountName.TrimString().ToLower();
                return !db.ProviderAccounts.Where(pa => pa.ProviderId == provId && pa.BankAccountName.ToLower() == accName).Any();
            }
            else if (fieldName == "ProviderAccountNumber" && viewModel != null)
            {
                if (viewModel == null || !(viewModel is ProviderAccountDetailsViewModel))
                    return false;

                string provId = (viewModel as ProviderAccountDetailsViewModel).ProviderId;
                string AccountNumber = (viewModel as ProviderAccountDetailsViewModel).AccountNumber.TrimString().ToLower();
                string BranchNumber = (viewModel as ProviderAccountDetailsViewModel).BranchNumber.TrimString().ToLower();

                return !db.ProviderAccounts.Where(pa => pa.ProviderId == provId && pa.BankAccountNumber.ToLower() == AccountNumber &&
                    pa.BankBranchNumber.ToLower() == BranchNumber).Any();
            }

            return false;
        }

        #region Register
        //
        // GET: /Account/RegisterCompany
        public ActionResult RegisterCompany()
        {
            CompanyUserAllEntitiesModelView companyUserAllEntitiesModelView = new CompanyUserAllEntitiesModelView();
            companyUserAllEntitiesModelView.UserVM = null;
            return View("RegisterAll", companyUserAllEntitiesModelView);
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult RegisterAll()
        {
            CompanyUserAllEntitiesModelView companyUserAllEntitiesModelView = new CompanyUserAllEntitiesModelView();
            return View(companyUserAllEntitiesModelView);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAll(CompanyUserAllEntitiesModelView model)
        {
            IdentityResult result = null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UserVM.UserName == null && model.companyVM.AccountName == null && model.providerAccountVM.AccountName == null)
            {
                return View(model);
            }

            try
            {
                bool IsUserRegister = true;
                if (model.UserVM.UserName == null)
                    IsUserRegister = false;

                bool IsCompanyRegister = true;
                if (model.companyVM.AccountName == null)
                    IsCompanyRegister = false;

                bool IsProviderAccountRegister = true;
                if (model.providerAccountVM.AccountName == null)
                    IsProviderAccountRegister = false;

                #region chack if model is valid
                bool errorFlag = false;
                if (IsUserRegister && !IsUnique("UserName", model.UserVM.UserName.TrimString()))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "UserName is not unique.");
                }
                if (IsUserRegister && !IsUnique("UserEmail", model.UserVM.userContactDetails.Email.TrimString()))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "User email is not unique.");
                }
                if (IsCompanyRegister && !IsUnique("CompanyShortName", model.companyVM.CompanyShortName.TrimString()))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "Company short name is not unique.");
                }
                if (IsCompanyRegister && !IsUnique("CompanyFullName", model.companyVM.CompanyFullName.TrimString()))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "Company full name is not unique.");
                }
                if (IsCompanyRegister && !IsUnique("AccountName", model.companyVM.AccountName.TrimString()))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "Account name is not unique.");
                }
                if (IsCompanyRegister && !IsUnique("AccountFullName", model.companyVM.AccountFullName.TrimString()))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "Account full name is not unique.");
                }
                if (IsProviderAccountRegister && !IsUnique("ProviderAccountName", model.providerAccountVM))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "Provider account name is not unique.");
                }
                if (IsProviderAccountRegister && !IsUnique("ProviderAccountNumber", model.providerAccountVM))
                {
                    errorFlag = true;
                    ModelState.AddModelError("", "Provider account number is not unique.");
                }

                if (errorFlag)
                {
                    return View(model);
                }
                #endregion

                ApplicationUser user = null;
                bool succeeded = true;
                
                if (IsUserRegister)
                {
                    //Create User
                    user = new ApplicationUser();
                    user.UserName = model.UserVM.UserName.TrimString();
                    user.FirstName = model.UserVM.FirstName.TrimString();
                    user.LastName = model.UserVM.LastName.TrimString();
                    user.Email = model.UserVM.userContactDetails.Email.TrimString();
                    user.RoleInCompany = model.UserVM.Role;
                    user.PhoneNumber = model.UserVM.userContactDetails.MobilePhone.TrimString();
                    user.ContactDetails.OfficePhone = model.UserVM.userContactDetails.OfficePhone.TrimString();
                    user.ContactDetails.Fax = model.UserVM.userContactDetails.Fax.TrimString();
                    user.ContactDetails.Country = model.UserVM.userContactDetails.Country;
                    user.ContactDetails.WebSite = model.UserVM.userContactDetails.WebSite.TrimString();
                    user.ContactDetails.MobilePhone = model.UserVM.userContactDetails.MobilePhone.TrimString();

                    result = await UserManager.CreateAsync(user, model.UserVM.Password);
                    succeeded = result.Succeeded;
                }
                else
                {
                    string userId = User.Identity.GetUserId();
                    user = db.Users.Include(u => u.Companies).Where(u => u.Id == userId).FirstOrDefault();
                }

                if (succeeded)
                {
                    if (IsUserRegister)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        UserManager.AddToRole(user.Id, Consts.Role_CompanyUser);

                        user = db.Users.Include(u => u.Companies).Where(u => u.Id == user.Id).FirstOrDefault();

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    }

                    if (IsCompanyRegister)
                    {
                        //Add company
                        Company company = new Company();
                        company.CompanyFullName = model.companyVM.CompanyFullName.TrimString();
                        company.CompanyShortName = model.companyVM.CompanyShortName.TrimString();
                        company.CompanyVolumePerYearUSD = model.companyVM.CompanyVolumePerYearUSD;
                        company.CustomerType = model.companyVM.CustomerType;
                        company.ValidIP = model.companyVM.ValidIP;
                        company.ContactDetails.Email = model.companyVM.companyContactDetails.Email.TrimString();
                        company.ContactDetails.MobilePhone = model.companyVM.companyContactDetails.MobilePhone.TrimString();
                        company.ContactDetails.OfficePhone = model.companyVM.companyContactDetails.OfficePhone.TrimString();
                        company.ContactDetails.Fax = model.companyVM.companyContactDetails.Fax.TrimString();
                        company.ContactDetails.Country = model.companyVM.companyContactDetails.Country;
                        company.ContactDetails.WebSite = model.companyVM.companyContactDetails.WebSite.TrimString();
                        company.ContactDetails.Address = model.companyVM.companyContactDetails.contactDetailsEx.Address.TrimString();
                        company.ContactDetails.Email2 = model.companyVM.companyContactDetails.contactDetailsEx.Email2.TrimString();
                        company.ContactDetails.MobilePhone2 = model.companyVM.companyContactDetails.contactDetailsEx.MobilePhone2.TrimString();
                        company.ContactDetails.OfficePhone2 = model.companyVM.companyContactDetails.contactDetailsEx.OfficePhone2.TrimString();
                        company.ContactDetails.HomePhone = model.companyVM.companyContactDetails.contactDetailsEx.HomePhone.TrimString();
                        company.ContactDetails.CarPhone = model.companyVM.companyContactDetails.contactDetailsEx.CarPhone.TrimString();

                        //Attach user to company
                        company.Users.Add(user);

                        db.Companies.Add(company);

                        //Add company account
                        CompanyAccount companyAccount = new CompanyAccount();
                        companyAccount.AccountFullName = model.companyVM.AccountFullName.TrimString();
                        companyAccount.AccountName = model.companyVM.AccountName.TrimString();
                        companyAccount.CompanyId = company.CompanyId;
                        db.CompanyAccounts.Add(companyAccount);

                        model.companyAccountParent = companyAccount.CompanyAccountId;
                    }

                    if (IsProviderAccountRegister)
                    {
                        //Add provider account
                        var providerAccount = new ProviderAccount();
                        providerAccount.BankAccountFullName = model.providerAccountVM.AccountFullName.TrimString();
                        providerAccount.BankAccountName = model.providerAccountVM.AccountName.TrimString();
                        providerAccount.BankAccountNumber = model.providerAccountVM.AccountNumber.TrimString();
                        providerAccount.BankAddress = model.providerAccountVM.Address.TrimString();
                        providerAccount.BankBranchNumber = model.providerAccountVM.BranchNumber.TrimString();
                        providerAccount.CompanyAccountId = model.companyAccountParent;
                        providerAccount.IBAN = model.providerAccountVM.IBAN;
                        //providerAccount.IsDemoAccount = true;
                        providerAccount.LastUpdateBy = User.Identity.Name;
                        providerAccount.ProviderId = model.providerAccountVM.ProviderId;
                        providerAccount.SWIFT = model.providerAccountVM.SWIFT;
                        providerAccount.AllowToTradeDirectlly = model.providerAccountVM.AllowToTradeDirectlly;
                        db.ProviderAccounts.Add(providerAccount);
                    }

                    db.SaveChanges();

                    if (IsCompanyRegister && IsUserRegister && IsProviderAccountRegister)
                        return RedirectToAction("Index", "Home");
                    else if (IsCompanyRegister)
                        return RedirectToAction("IndexUser", "Companies");
                    else
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
        public async Task<ActionResult> RegisterDemo(DemoUserDetailsModelView model)
        {
            IdentityResult result = null;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                #region chack if model is valid
                if (!IsUnique("UserName", model.Email.TrimString()))
                    ModelState.AddModelError("", "User Name is not unique.");
                if (!IsUnique("UserEmail", model.Email.TrimString()))
                    ModelState.AddModelError("", "User Email is not unique.");
                #endregion

                //Create User
                var user = new ApplicationUser();
                user.UserName = model.Email.TrimString();
                user.FirstName = model.Email.TrimString().Substring(0, model.Email.TrimString().IndexOf('@'));
                user.LastName = "Demo";
                user.Email = model.Email.TrimString();
                user.RoleInCompany = "";
                user.PhoneNumber = (model.MobilePhone == null)? "" : model.MobilePhone.TrimString();
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.Status = FlatFXCore.BussinessLayer.Consts.eUserStatus.Active;
                user.Language = Consts.eLanguage.English;

                result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    UserManager.AddToRole(user.Id, Consts.Role_CompanyDemoUser);

                    //Add the demo user to the Demo company
                    Company demoCompany = db.Companies.Where(c => c.CompanyShortName == "Demo").SingleOrDefault();
                    if (demoCompany != null)
                    {
                        var u = db.Users.First(p => p.Email == user.Email);
                        demoCompany.Users.Add(u);
                        db.SaveChanges();
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