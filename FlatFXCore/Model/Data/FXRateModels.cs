using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFXCore.Model.Data
{
    [Table("Currency")]
    public class Currency
    {
        [Key, Column(TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public Currency() { }
    }

    [Table("FXRates")]
    public class FXRate
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

        public FXRate() { }
    }
    [Table("HistoricalFXRates")]
    public class HistoricalFXRate
    {
        [Key, Column(Order = 1, TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }
        [ForeignKey("Key")]
        public virtual FXRate FXRate { get; set; }

        [Key, Column(Order = 2), Required]
        public DateTime Time { get; set; }

        [Required]
        public double Bid { get; set; }
        [Required]
        public double Ask { get; set; }
        [Required]
        public double Mid { get; set; }

        

        public HistoricalFXRate() { }
    }
    [Table("DailyFXRates")]
    public class DailyFXRate
    {
        [Key, Column(Order = 1, TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }
        [ForeignKey("Key")]
        public virtual FXRate FXRate { get; set; }

        [Key, Column(Order = 2), Required]
        public DateTime Time { get; set; }

        [Required]
        public double Bid { get; set; }
        [Required]
        public double Ask { get; set; }
        [Required]
        public double Mid { get; set; }

        public DailyFXRate() { }
    }
}
