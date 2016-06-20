using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Security.Principal;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.User;
using System.Linq.Expressions;

namespace FlatFXCore.Model.Data
{
    [Table("EmailNotifications")]
    public class EmailNotification
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 EmailNotificationId { get; set; }
        [Required]
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        
        [Required]
        public string Subject { get; set; }
        public string Body { get; set; }
        [Required]
        public Consts.eEmailStatus EmailStatus { get; set; }

        public EmailNotification()
        {
            //Set the default values
            EmailStatus = Consts.eEmailStatus.None;
            EmailService es = new EmailService();
            From = es.DefaultFromEmail;
        }
        public EmailNotification(string To, string Bcc)
        {
            //Set the default values
            EmailStatus = Consts.eEmailStatus.None;
            EmailService es = new EmailService();
            From = es.DefaultFromEmail;
            if (To == null)
                this.To = "info@FlatFX.com";
            else
                this.To = To;
            if (Bcc != null)
                this.Bcc = Bcc;
        }

    }
}