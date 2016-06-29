using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlatFXCore.Model.Data;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using FlatFXCore.Model.User;
using MySql.Data.Entity;

namespace FlatFXCore.Model.Core
{
    /// <summary>
    /// 
    /// Update-Database -Force -Verbose
    /// 
    /// UpdateDatabase DatabaseUpdate Database-Update
    /// </summary>
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        //public DbSet<ContactDetails> ContactsDetails { get; set; }
        public DbSet<UserMessageData> UserMessages { get; set; }
        public DbSet<UserFavoriteData> UserFavorites { get; set; }
        public DbSet<UserActionData> UserActions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyAccount> CompanyAccounts { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderAccount> ProviderAccounts { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatEntry> ChatEntries { get; set; }
        public DbSet<SpreadInfo> Spreads { get; set; }
        public DbSet<FXRate> FXRates { get; set; }
        public DbSet<HistoricalFXRate> HistoricalFXRates { get; set; }
        public DbSet<DailyFXRate> DailyFXRates { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderMatch> OrderMatches { get; set; }
        public DbSet<QueryData> Querys { get; set; }
        public DbSet<QueryPerProvider> QueriesPerProvider { get; set; }
        public DbSet<ConfigurationRow> Configurations { get; set; }
        public DbSet<LogInfo> LogInfo { get; set; }
        public DbSet<GenericDictionaryItem> GenericDictionary { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }
        public DbSet<NewOrderNotification> NewOrderNotifications { get; set; }

        public ApplicationDBContext()
            : base("name=FFXConnectionString", throwIfV1Schema: false)
        {
            
        }

        public static ApplicationDBContext Create()
        {
            return new ApplicationDBContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ApplicationUser>()
            //        .Property(u => u.ContactDetails.Email)
            //        .HasColumnName("User_Email");

            //one to many
            //modelBuilder.Entity<ProviderAccount>()
            //           .HasRequired<Provider>(pa => pa.Provider)
            //           .WithMany(p => p.Accounts)
            //           .HasForeignKey(p => p.ProviderId);

            ////many to many
            //modelBuilder.Entity<ApplicationUser>()
            //       .HasMany<Company>(u => u.Companies)
            //       .WithMany(c => c.Users)
            //       .Map(cu =>
            //       {
            //           cu.MapLeftKey("ApplicationUser_Id");
            //           cu.MapRightKey("Company_CompanyId");
            //           cu.ToTable("ApplicationUserCompanies");
            //       });

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<IdentityUser>().ToTable("FlatFXUsers").Property(p => p.Id).HasColumnName("UserId");
            //modelBuilder.Entity<ApplicationUser>().ToTable("FlatFXUsers").Property(p => p.Id).HasColumnName("UserId");
            //modelBuilder.Entity<IdentityUserRole>().ToTable("FlatFXUserRoles");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("FlatFXUserLogins");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("FlatFXUserClaims");
            //modelBuilder.Entity<IdentityRole>().ToTable("FlatFXRoles");
        }
    }
}
