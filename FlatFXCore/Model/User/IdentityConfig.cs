using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using FlatFXCore.Model.Core;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Data;

namespace FlatFXCore.Model.User
{
    public class EmailService : IIdentityMessageService
    {
        public EmailService()
        {
        }

        public string DefaultFromEmail
        {
            get
            {
                return From_DefaultEmailAddress.Address;
            }
        }

        public System.Net.Mail.MailAddress From_DefaultEmailAddress
        {
            get
            {
                if (ApplicationInformation.Instance.IsDevelopmetMachine)
                    return new System.Net.Mail.MailAddress("noreply.FlatFX@gmail.com", "No Reply FlatFX (Gmail)");
                else
                    return new System.Net.Mail.MailAddress("noreply@flatfx.com", "No Reply FlatFX");
            }
        }

        public async Task SendAsync(IdentityMessage message)
        {
            await configSMTPasync(message);
        }

        // send email via smtp service
        private async Task configSMTPasync(IdentityMessage message)
        {
            try
            {
                // Create the message:
                var mail = new System.Net.Mail.MailMessage(From_DefaultEmailAddress, new System.Net.Mail.MailAddress(message.Destination));
                mail.Subject = message.Subject;
                mail.IsBodyHtml = true;
                mail.Body = message.Body;

                await FFXSmtpClient.SendMailAsync(mail);

                mail = null;
            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError("Failed to send email to " + message.Destination + ". Subject: " + message.Subject, ex);
            }
        }

        public bool SendMail(EmailNotification emailNotification)
        {
            try
            {
                // Create the message:
                var mail = new System.Net.Mail.MailMessage();
                mail.From = From_DefaultEmailAddress;
                
                if (emailNotification.To == null || emailNotification.To.Trim() == "")
                    emailNotification.To = "info@FlatFX.com";
                foreach(string mailAddr in emailNotification.To.Split(';'))
                    mail.To.Add(mailAddr.Trim());

                if (emailNotification.Cc != null && emailNotification.Cc.Trim() != "")
                {
                    foreach (string mailAddr in emailNotification.Cc.Split(';'))
                        mail.CC.Add(mailAddr.Trim());
                }

                if (emailNotification.Bcc != null && emailNotification.Bcc.Trim() != "")
                {
                    foreach (string mailAddr in emailNotification.Bcc.Split(';'))
                        mail.Bcc.Add(mailAddr.Trim());
                }

                mail.Subject = emailNotification.Subject;
                mail.IsBodyHtml = true;
                mail.Body = emailNotification.Body;

                FFXSmtpClient.Send(mail);

                mail = null;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed to send email to " + emailNotification.To + ". Subject: " + emailNotification.Subject, ex);
                return false;
            }
        }

        private System.Net.Mail.SmtpClient FFXSmtpClient
        {
            get
            {
                System.Net.Mail.SmtpClient client = null;

                if (!ApplicationInformation.Instance.IsDevelopmetMachine)
                {
                    /* Contabo */
                    client = new System.Net.Mail.SmtpClient("mail.flatfx.com", 25);
                    client.Timeout = 30000;
                    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = false;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(From_DefaultEmailAddress.Address, "tr#tmQ26");
                    client.Credentials = credentials;
                }
                else
                {
                    var credentialUserName = "noreply.FlatFX@gmail.com";
                    var pwd = "Rt54!ty76"; // "FlatFx1!";

                    client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                    client.Timeout = 30000;
                    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
                    client.EnableSsl = true;
                    client.Credentials = credentials;
                }

                return client;
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDBContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
