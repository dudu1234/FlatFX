﻿using FlatFX.BussinessLayer;
using FlatFX.Model.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFX.Model.Data
{
    //Attributes Examples:
    //[MaxLength(10),MinLength(5)] 
    //[MaxLength(10, ErrorMessage="BloggerName must be 10 characters or less"),MinLength(5)]
    //[ConcurrencyCheck, MaxLength(10, ErrorMessage = "BloggerName must be 10 characters or less"), MinLength(5)]
    //[Column(“BlogDescription", TypeName="ntext")]
    //[ForeignKey("BlogId")]



    [Table("Users")]
    public class UserData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Index("IX_UserName", IsUnique = true), MaxLength(100), Required]
        public string UserName { get; set; }
        [MaxLength(100), Required]
        public string FirstName {
            get
            {
                return "aaa";
            }
            set
            {

            }

        }
        /// <summary>
        /// MiddleName 
        /// </summary>
        [MaxLength(100)]
        public string MiddleName { get; set; }
        [MaxLength(100), Required]
        public string LastName { get; set; }
        [MaxLength(200), Required]
        public string Password { get; set; }
        [Required, DefaultValue(true), Index("IX_IsActive", IsUnique = false)]
        public bool IsActive { get; set; }
        
        [Required]
        public Consts.eUserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        [MaxLength(100)]
        public string Role { get; set; }

        public int ContactDetailsId { get; set; }
        [ForeignKey("ContactDetailsId")]
        public ContactDetails ContactDetails { get; set; }

        // NAVIGATION PROPERTIES GUY ??? if I uncomment this line a new index is created in table UserMessages
        //public virtual ICollection<UserMessageData> ToMessageData { get; set; }
        //public virtual ICollection<UserMessageData> FromMessageData { get; set; }
        public virtual ICollection<UserActionData> Actions { get; set; }
        public virtual ICollection<UserFavoriteData> Favorites { get; set; }
        public virtual ICollection<ProviderData> Providers { get; set; }
        public virtual ICollection<CompanyData> Companies { get; set; }

        public UserData() { }


        public string FullName
        {
            get
            {
                return FirstName + " " + ((MiddleName != "")? (MiddleName + " ") : "") + LastName;
            }
            
        }
    }
    [Table("ContactDetails")]
    public class ContactDetails
    {
        [Key]
        public int ContactDetailsId { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
        [MaxLength(200)]
        public string Email2 { get; set; }
        
        [MaxLength(30)]
        public string OfficePhone { get; set; }
        [MaxLength(30)]
        public string OfficePhone2 { get; set; }

        [MaxLength(30)]
        public string Fax { get; set; }
        [MaxLength(30)]
        public string HomePhone { get; set; }

        [MaxLength(30)]
        public string MobilePhone { get; set; }
        [MaxLength(30)]
        public string MobilePhone2 { get; set; }

        [MaxLength(30)]
        public string CarPhone { get; set; }

        [MaxLength(400)]
        public string Address { get; set; }
        [MaxLength(200)]
        public string Country { get; set; }
        [MaxLength(400)]
        public string WebSite { get; set; }
    }
    [Table("UserMessages")]
    public class UserMessageData
    {
        [Key]
        public Int64 MessageId { get; set; }

        public int FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public virtual UserData FromUser { get; set; }
        public int ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public virtual UserData ToUser { get; set; }

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

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserData User { get; set; }

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

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserData User { get; set; }

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