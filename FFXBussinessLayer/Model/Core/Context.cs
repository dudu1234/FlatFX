using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlatFX.Model.Data;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FlatFX.Model.Core
{
    public class FfxContext : DbContext
    {
        public DbSet<UserData> Users { get; set; }
        public DbSet<ContactDetails> ContactsDetails { get; set; }
        public DbSet<UserMessageData> UserMessages { get; set; }
        public DbSet<UserFavoriteData> UserFavorites { get; set; }
        public DbSet<UserActionData> UserActions { get; set; }
        public DbSet<CompanyData> Companies { get; set; }
        public DbSet<CompanyAccountData> CompanyAccounts { get; set; }
        public DbSet<ProviderData> Providers { get; set; }
        public DbSet<ProviderAccountData> ProviderAccounts { get; set; }
        public DbSet<ChatSessionData> ChatSessions { get; set; }
        public DbSet<ChatEntrieData> ChatEntries { get; set; }
        public DbSet<SpreadData> Spreads { get; set; }
        public DbSet<FXRateData> FXRates { get; set; }
        public DbSet<HistoricalFXRateData> HistoricalFXRates { get; set; }
        public DbSet<DailyFXRateData> DailyFXRates { get; set; }
        public DbSet<DealData> Deals { get; set; }
        public DbSet<QueryData> Querys { get; set; }
        public DbSet<QueryPerProviderData> QueriesPerProvider { get; set; }
        public DbSet<ConfigurationData> Configurations { get; set; }
        public DbSet<LogInfoData> LogInfo { get; set; }

        public FfxContext()
            : base("name=FFXConnectionString")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<UserData>()
                    .HasMany(u => u.ToMessageData)
                    .HasRequired(m => m.FromUser)
                    .HasForeignKey(m => m.FromUserId)
                    .WillCascadeOnDelete(false);
            */
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
            
           // var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
           //.Where(type => !String.IsNullOrEmpty(type.Namespace))
           //.Where(type => type.BaseType != null && type.BaseType.IsGenericType
           //     && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
           // foreach (var type in typesToRegister)
           // {
           //     dynamic configurationInstance = Activator.CreateInstance(type);
           //     modelBuilder.Configurations.Add(configurationInstance);
           // }
           // base.OnModelCreating(modelBuilder);
        }
    }
}
