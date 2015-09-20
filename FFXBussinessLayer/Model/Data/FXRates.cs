using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFX.Model.Data
{
    [Table("FXRates")]
    public class FXRateData
    {
        [Key, Column(Order = 1, TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }

        [Required]
        public double Bid { get; set; }
        [Required]
        public double Ask { get; set; }
        [Required]
        public double Mid { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public FXRateData() { }
    }
    [Table("HistoricalFXRates")]
    public class HistoricalFXRateData
    {
        [Key, Column(Order = 1, TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }
        [ForeignKey("Key")]
        public virtual FXRateData FXRate { get; set; }

        [Key, Column(Order = 2), Required]
        public DateTime Time { get; set; }

        [Required]
        public double Bid { get; set; }
        [Required]
        public double Ask { get; set; }
        [Required]
        public double Mid { get; set; }

        

        public HistoricalFXRateData() { }
    }
    [Table("DailyFXRates")]
    public class DailyFXRateData
    {
        [Key, Column(Order = 1, TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }
        [ForeignKey("Key")]
        public virtual FXRateData FXRate { get; set; }

        [Key, Column(Order = 2), Required]
        public DateTime Time { get; set; }

        [Required]
        public double Bid { get; set; }
        [Required]
        public double Ask { get; set; }
        [Required]
        public double Mid { get; set; }

        public DailyFXRateData() { }
    }
}
