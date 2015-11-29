using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFXCore.Model.Data
{
    [Table("LogInfo")]
    public class LogInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 LogId { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }
        [MaxLength(-1)]
        public string ExtendedDescription { get; set; }
        [MaxLength(30)]
        public string ApplicationName { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public string UserIP { get; set; }
        [MaxLength(30)]
        public string SessionId { get; set; }
        [Required]
        public Consts.eLogLevel LogLevel { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public Consts.eLogOperationLevel OperationLevel { get; set; }
        [MaxLength(200)]
        public string OperationName { get; set; }
        public Consts.eLogOperationStatus? OperationStatus { get; set; }

        public LogInfo() { }
    }
}
