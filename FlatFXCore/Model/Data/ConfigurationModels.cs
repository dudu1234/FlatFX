using FlatFXCore.BussinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFXCore.Model.Data
{
    [Table("Configuration")]
    public class ConfigurationRow
    {
        [Key, Column(Order = 1), MaxLength(100)]
        public string Key { get; set; }
        
        [Key, Column(Order = 2)]
        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        //public virtual UserData user { get; set; }

        [Required, MaxLength(2000)]
        public string Value { get; set; }
        [Required, MaxLength(2000)]
        public string Description { get; set; }
        [Required]
        public bool AutomaticDisplay { get; set; }
        [Required, MaxLength(100)]
        public string TabName { get; set; }
        [Required]
        public bool UserOption { get; set; }
        [MaxLength(100)]
        public string DisplayTag { get; set; }
        public Consts.eConfigurationFieldType Type { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        [Required]
        public int TabOrder { get; set; }
        [Required, MaxLength(2000)]
        public string DefaultValue { get; set; }

        public ConfigurationRow() { }
    }
}
