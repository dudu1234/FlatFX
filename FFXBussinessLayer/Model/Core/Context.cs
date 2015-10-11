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

namespace FlatFXCore.Model.Core
{
    public class FfxContext : IdentityDbContext<ApplicationUser>
    {
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
            : base("name=FFXConnectionString", throwIfV1Schema: false)
        {
            
        }

        public static FfxContext Create()
        {
            return new FfxContext();
        }
    }
}
