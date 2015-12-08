using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatFXCore.BussinessLayer;
using System.Threading;
using System.Globalization;
using FlatFXCore.Model.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatFXCore.Model.Core
{
    /// <summary>
    /// GenericDictionaryItem
    /// </summary>
    public class GenericDictionaryItem
    {
        [Key, Column(Order = 1, TypeName = "NVARCHAR"), MaxLength(300)]
        public string Key { get; set; }
        
        [Key, Column(Order = 2)]
        public string Category { get; set; }

        [StringLength(200)]
        public string Info1 { get; set; }
        [StringLength(200)]
        public string Info2 { get; set; }

        public GenericDictionaryItem() 
        {
        }
    }
}
