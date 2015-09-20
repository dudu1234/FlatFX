//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel.DataAnnotations.Schema;  
//using System.Data.Entity.ModelConfiguration;
//using FlatFX.Model.Data;  

//namespace FlatFX.Model.Mapping  
//{  
//    public class UserMap : EntityTypeConfiguration<User>  
//    {  
//        public UserMap()  
//        {  
//            //Key  
//            HasKey(t => t.ID);  
  
//            //Fields  
//            Property(t => t.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);  
//            Property(t => t.UserName).IsRequired().HasMaxLength(25);
//            Property(t => t.FirstName);
//            Property(t => t.LastName);
//            Property(t => t.AddedDate).IsRequired();  
//            Property(t => t.ModifiedDate).IsRequired();  
//            Property(t => t.IP);  
  
//            //table  
//            ToTable("Users");  
//        }  
//    }
//}