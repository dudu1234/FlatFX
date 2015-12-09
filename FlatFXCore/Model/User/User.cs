using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlatFXCore.Model.User
{
    public class ApplicationUser : IdentityUser
    {
        /*/override IdentityUser members
        [Display(Name = "UserName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(256, MinimumLength = 3, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public virtual string UserName { get; set; }
        [Display(Name = "Email", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(256, MinimumLength = 3, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [EmailAddress]
        public virtual string Email { get; set; }
        [Display(Name = "PhoneNumber", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [Phone]
        public virtual string PhoneNumber { get; set; }
        */
        [Description("This is the attribute for FirstName")]
        [Display(Name = "FirstName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string FirstName { get; set; }
        [MaxLength(100)]
        [Display(Name = "MiddleName", ResourceType = typeof(FlatFXResources.Resources))]
        public string MiddleName { get; set; }
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [Display(Name = "LastName", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string LastName { get; set; }
        [Display(Name = "Is Active"), Required, DefaultValue(true), Index("IX_IsActive", IsUnique = false)]
        public bool IsActive { get; set; }
        [Required]
        public Consts.eUserStatus Status { get; set; }
        [Display(Name = "CreatedAt", ResourceType = typeof(FlatFXResources.Resources))]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Role", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string RoleInCompany { get; set; }
        [Display(Name = "Language", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eLanguage Language { get; set; }
        [MaxLength(16), MinLength(8)]
        [Display(Name = "SigningKey", ResourceType = typeof(FlatFXResources.Resources))]
        public string SigningKey { get; set; }
        [Display(Name = "InvoiceCurrency", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eInvoiceCurrency InvoiceCurrency { get; set; }

        [Display(Name = "Is approved by FlatFX"), DefaultValue(false)]
        public bool IsApprovedByFlatFX { get; set; }

        public ContactDetails ContactDetails { get; set; }

        // NAVIGATION PROPERTIES GUY ??? if I uncomment this line a new index is created in table UserMessages
        //public virtual ICollection<UserMessageData> ToMessageData { get; set; }
        //public virtual ICollection<UserMessageData> FromMessageData { get; set; }
        public virtual ICollection<UserActionData> Actions { get; set; }
        public virtual ICollection<UserFavoriteData> Favorites { get; set; }
        public virtual ICollection<Provider> Providers { get; set; }
        public virtual ICollection<Company> Companies { get; set; }

        public ApplicationUser()
        {
            //set default new user values
            this.Id = Guid.NewGuid().ToString();
            Language = Consts.eLanguage.English;
            CreatedAt = DateTime.Now;
            IsActive = true;
            Status = FlatFXCore.BussinessLayer.Consts.eUserStatus.Active;
            SigningKey = Guid.NewGuid().ToString().Substring(0, 8);
                
            ContactDetails = new ContactDetails(); 
            Companies = new List<Company>();
            Providers = new List<Provider>(); 
            Actions = new List<UserActionData>();
            Favorites = new List<UserFavoriteData>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + ((MiddleName != "")? (MiddleName + " ") : "") + LastName;
            }
        }
    }
    [ComplexType]
    public class ContactDetails
    {
        [Display(Name = "Email", ResourceType = typeof(FlatFXResources.Resources))]        
        [Column("Email1"), MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; }
        [Column("Email2"), MaxLength(200)]
        [EmailAddress]
        [Display(Name = "Email2", ResourceType = typeof(FlatFXResources.Resources))]
        public string Email2 { get; set; }

        [Column("OfficePhone"), MaxLength(30)]
        [Display(Name = "OfficePhone", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string OfficePhone { get; set; }
        [Column("OfficePhone2"), MaxLength(30)]
        [Phone]
        [Display(Name = "OfficePhone2", ResourceType = typeof(FlatFXResources.Resources))]
        public string OfficePhone2 { get; set; }

        [Column("Fax"), MaxLength(30)]
        [Display(Name = "Fax", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string Fax { get; set; }
        [Column("HomePhone"), MaxLength(30)]
        [Phone]
        [Display(Name = "HomePhone", ResourceType = typeof(FlatFXResources.Resources))]
        public string HomePhone { get; set; }

        [Column("MobilePhone"), MaxLength(30)]
        [Display(Name = "MobilePhone", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string MobilePhone { get; set; }
        [Column("MobilePhone2"), MaxLength(30)]
        [Phone]
        [Display(Name = "MobilePhone2", ResourceType = typeof(FlatFXResources.Resources))]
        public string MobilePhone2 { get; set; }

        [Column("CarPhone"), MaxLength(30)]
        [Phone]
        [Display(Name = "CarPhone", ResourceType = typeof(FlatFXResources.Resources))]
        public string CarPhone { get; set; }

        [Column("Address"), MaxLength(400)]
        [Display(Name = "Address", ResourceType = typeof(FlatFXResources.Resources))]
        public string Address { get; set; }
        [Column("Country")]
        [Display(Name = "Country", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCountries? Country { get; set; }
        [Column("WebSite"), MaxLength(400)]
        [Display(Name = "WebSite", ResourceType = typeof(FlatFXResources.Resources))]
        public string WebSite { get; set; }
    }
    [Table("UserMessages")]
    public class UserMessageData
    {
        [Key]
        public Int64 MessageId { get; set; }

        public string FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public virtual ApplicationUser FromUser { get; set; }
        public string ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public virtual ApplicationUser ToUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }
        [MaxLength(2000)]
        public string Text { get; set; }
        [Required]
        public bool IsRemoved { get; set; }
        public Int16 Priority { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public bool IsImportant { get; set; }
    }
    [Table("UserFavorites")]
    public class UserFavoriteData
    {
        [Key]
        public int FavoriteId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required, MaxLength(50)]
        public string Category { get; set; }
        [Required, MaxLength(400)]
        public string Text { get; set; }
        [Required]
        public int Priority { get; set; }
    }
    [Table("UserActions")]
    public class UserActionData
    {
        [Key]
        public Int64 ActionId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DateTime Time { get; set; }
        [MaxLength(500)]
        public string Text { get; set; }
        [Required]
        public Int16 Priority { get; set; }
        [Required]
        public bool IsSucceded { get; set; }
        [Required]
        public bool IsRemoved { get; set; }
    }
}
