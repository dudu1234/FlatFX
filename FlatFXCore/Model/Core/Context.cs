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
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyAccount> CompanyAccounts { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderAccount> ProviderAccounts { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatEntrie> ChatEntries { get; set; }
        public DbSet<SpreadInfo> Spreads { get; set; }
        public DbSet<FXRate> FXRates { get; set; }
        public DbSet<HistoricalFXRate> HistoricalFXRates { get; set; }
        public DbSet<DailyFXRate> DailyFXRates { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<QueryData> Querys { get; set; }
        public DbSet<QueryPerProvider> QueriesPerProvider { get; set; }
        public DbSet<ConfigurationRow> Configurations { get; set; }
        public DbSet<LogInfo> LogInfo { get; set; }

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
