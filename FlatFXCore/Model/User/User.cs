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
        [MaxLength(100), Required]
        [Display(Name = "FirstName", ResourceType = typeof(FlatFXResources.Resources))]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name"), MaxLength(100)]
        public string MiddleName { get; set; }
        [MaxLength(100), Required]
        [Display(Name = "LastName", ResourceType = typeof(FlatFXResources.Resources))]
        public string LastName { get; set; }
        [Display(Name = "Is Active"), Required, DefaultValue(true), Index("IX_IsActive", IsUnique = false)]
        public bool IsActive { get; set; }
        [Required]
        public Consts.eUserStatus Status { get; set; }
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Role In Company"), MaxLength(100)]
        public string RoleInCompany { get; set; }
        [DisplayName("Language")]
        public Consts.eLanguage Language { get; set; }
        [MaxLength(16), MinLength(8), DisplayName("Signing Key")]
        public string SigningKey { get; set; }
        [DisplayName("Invoice Currency")]
        public Consts.eInvoiceCurrency InvoiceCurrency { get; set; }
        
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
            Language = Consts.eLanguage.English;
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
        [Column("Email1"), DisplayName("Email"), MaxLength(200)]
        public string Email { get; set; }
        [Column("Email2"), DisplayName("Additional Email"), MaxLength(200)]
        public string Email2 { get; set; }

        [Column("OfficePhone"), DisplayName("Office Phone"), MaxLength(30)]
        public string OfficePhone { get; set; }
        [Column("OfficePhone2"), DisplayName("Additional Office Phone"), MaxLength(30)]
        public string OfficePhone2 { get; set; }

        [Column("Fax"), DisplayName("Fax"), MaxLength(30)]
        public string Fax { get; set; }
        [Column("HomePhone"), DisplayName("Home Phone"), MaxLength(30)]
        public string HomePhone { get; set; }

        [Column("MobilePhone"), DisplayName("Mobile Phone"), MaxLength(30)]
        public string MobilePhone { get; set; }
        [Column("MobilePhone2"), DisplayName("Additional Mobile Phone"), MaxLength(30)]
        public string MobilePhone2 { get; set; }

        [Column("CarPhone"), DisplayName("Car Phone"), MaxLength(30)]
        public string CarPhone { get; set; }

        [Column("Address"), DisplayName("Address"), MaxLength(400)]
        public string Address { get; set; }
        [Column("Country"), DisplayName("Country")]
        public Consts.eCountries? Country { get; set; }
        [Column("WebSite"), DisplayName("WebSite"), MaxLength(400)]
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
