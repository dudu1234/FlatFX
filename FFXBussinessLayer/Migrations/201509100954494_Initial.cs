namespace FlatFX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatEntries",
                c => new
                    {
                        ChatEntryId = c.Long(nullable: false, identity: true),
                        ChatSessionId = c.Long(nullable: false),
                        UserId = c.Int(nullable: false),
                        Text = c.String(maxLength: 4000),
                        Time = c.DateTime(nullable: false),
                        QueryId = c.Long(nullable: false),
                        EntryType = c.Int(nullable: false),
                        ObjectData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ChatEntryId)
                .ForeignKey("dbo.ChatSessions", t => t.ChatSessionId)
                .ForeignKey("dbo.Queries", t => t.QueryId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.ChatSessionId)
                .Index(t => t.UserId)
                .Index(t => t.QueryId);
            
            CreateTable(
                "dbo.ChatSessions",
                c => new
                    {
                        ChatSessionId = c.Long(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(),
                        ChatListenerUsers = c.String(maxLength: 200),
                        LastChatEntry = c.Long(),
                    })
                .PrimaryKey(t => t.ChatSessionId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .Index(t => t.CompanyId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyShortName = c.String(nullable: false, maxLength: 20),
                        CompanyFullName = c.String(nullable: false, maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                        LastUpdate = c.DateTime(),
                        ValidIP = c.String(maxLength: 300),
                        CustomerType = c.Int(),
                        IsDepositValid = c.Boolean(),
                        IsSignOnRegistrationAgreement = c.Boolean(),
                        ContactDetailsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.ContactDetails", t => t.ContactDetailsId)
                .Index(t => t.CompanyShortName, unique: true)
                .Index(t => t.ContactDetailsId);
            
            CreateTable(
                "dbo.CompanyAccounts",
                c => new
                    {
                        CompanyAccountId = c.Int(nullable: false, identity: true),
                        AccountName = c.String(nullable: false, maxLength: 200),
                        CompanyId = c.Int(nullable: false),
                        AccountFullName = c.String(nullable: false, maxLength: 400),
                        IsActive = c.Boolean(nullable: false),
                        IsDefaultAccount = c.Boolean(nullable: false),
                        Balance = c.Double(nullable: false),
                        Equity = c.Double(nullable: false),
                        DailyPNL = c.Double(nullable: false),
                        GrossPNL = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyAccountId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.AccountName, unique: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ContactDetails",
                c => new
                    {
                        ContactDetailsId = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 200),
                        Email2 = c.String(maxLength: 200),
                        OfficePhone = c.String(maxLength: 30),
                        OfficePhone2 = c.String(maxLength: 30),
                        Fax = c.String(maxLength: 30),
                        HomePhone = c.String(maxLength: 30),
                        MobilePhone = c.String(maxLength: 30),
                        MobilePhone2 = c.String(maxLength: 30),
                        CarPhone = c.String(maxLength: 30),
                        Address = c.String(maxLength: 400),
                        Country = c.String(maxLength: 200),
                        WebSite = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.ContactDetailsId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        MiddleName = c.String(maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Role = c.String(maxLength: 100),
                        ContactDetailsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ContactDetails", t => t.ContactDetailsId)
                .Index(t => t.UserName, unique: true)
                .Index(t => t.IsActive)
                .Index(t => t.ContactDetailsId);
            
            CreateTable(
                "dbo.UserActions",
                c => new
                    {
                        ActionId = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Text = c.String(maxLength: 500),
                        Priority = c.Short(nullable: false),
                        IsSucceded = c.Boolean(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ActionId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserFavorites",
                c => new
                    {
                        FavoriteId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Category = c.String(nullable: false, maxLength: 50),
                        Text = c.String(nullable: false, maxLength: 400),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FavoriteId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Providers",
                c => new
                    {
                        ProviderId = c.Int(nullable: false, identity: true),
                        ShortName = c.String(nullable: false, maxLength: 10),
                        FullName = c.String(nullable: false, maxLength: 200),
                        BankNumber = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        ProviderType = c.Int(nullable: false),
                        QuoteResponse_Enabled = c.Boolean(nullable: false),
                        QuoteResponse_SpreadMethod = c.Int(nullable: false),
                        QuoteResponse_StartTime = c.DateTime(nullable: false),
                        QuoteResponse_EndTime = c.DateTime(nullable: false),
                        QuoteResponse_FridayStartTime = c.DateTime(nullable: false),
                        QuoteResponse_FridayEndTime = c.DateTime(nullable: false),
                        QuoteResponse_UserConfirmationTimeInterval = c.Short(nullable: false),
                        QuoteResponse_AutomaticResponseEnabled = c.Boolean(nullable: false),
                        QuoteResponse_MinRequestVolumeUSD = c.Int(nullable: false),
                        QuoteResponse_MaxDailyVolumeUSD = c.Int(nullable: false),
                        QuoteResponse_NumberOfPromilsWithoutDiscount = c.Double(nullable: false),
                        ContactDetailsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProviderId)
                .ForeignKey("dbo.ContactDetails", t => t.ContactDetailsId)
                .Index(t => t.ShortName, unique: true)
                .Index(t => t.FullName, unique: true)
                .Index(t => t.ContactDetailsId);
            
            CreateTable(
                "dbo.ProviderAccounts",
                c => new
                    {
                        CompanyAccountId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        BankAccountName = c.String(nullable: false, maxLength: 200),
                        BankAccountFullName = c.String(maxLength: 400),
                        BankBranchNumber = c.String(maxLength: 10),
                        BankAccountNumber = c.String(maxLength: 20),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(),
                        QuoteResponse_IsBlocked = c.Boolean(nullable: false),
                        QuoteResponse_CustomerPromil = c.Double(),
                    })
                .PrimaryKey(t => new { t.CompanyAccountId, t.ProviderId })
                .ForeignKey("dbo.CompanyAccounts", t => t.CompanyAccountId)
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .Index(t => t.CompanyAccountId)
                .Index(t => t.ProviderId)
                .Index(t => t.BankAccountName, unique: true);
            
            CreateTable(
                "dbo.Queries",
                c => new
                    {
                        QueryId = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CompanyAccountId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Symbol = c.String(nullable: false, maxLength: 10, unicode: false),
                        DealType = c.Int(nullable: false),
                        BuySell = c.Int(nullable: false),
                        AmountCCY1 = c.Double(nullable: false),
                        AmountUSD = c.Double(),
                    })
                .PrimaryKey(t => t.QueryId)
                .ForeignKey("dbo.CompanyAccounts", t => t.CompanyAccountId)
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CompanyAccountId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.Configuration",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 100),
                        UserId = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 2000),
                        Description = c.String(nullable: false, maxLength: 2000),
                        AutomaticDisplay = c.Boolean(nullable: false),
                        TabName = c.String(nullable: false, maxLength: 100),
                        UserOption = c.Boolean(nullable: false),
                        DisplayTag = c.String(maxLength: 100),
                        Type = c.Int(nullable: false),
                        MinValue = c.Int(),
                        MaxValue = c.Int(),
                        TabOrder = c.Int(nullable: false),
                        DefaultValue = c.String(nullable: false, maxLength: 2000),
                    })
                .PrimaryKey(t => new { t.Key, t.UserId });
            
            CreateTable(
                "dbo.DailyFXRates",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        Time = c.DateTime(nullable: false),
                        Bid = c.Double(nullable: false),
                        Ask = c.Double(nullable: false),
                        Mid = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.Key, t.Time })
                .ForeignKey("dbo.FXRates", t => t.Key)
                .Index(t => t.Key);
            
            CreateTable(
                "dbo.FXRates",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        Bid = c.Double(nullable: false),
                        Ask = c.Double(nullable: false),
                        Mid = c.Double(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.Deals",
                c => new
                    {
                        DealId = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CompanyAccountId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        ProviderUserId = c.Int(nullable: false),
                        ProviderDealRef = c.String(maxLength: 100),
                        QueryId = c.Long(nullable: false),
                        DealType = c.Int(nullable: false),
                        BuySell = c.Int(nullable: false),
                        Symbol = c.String(nullable: false, maxLength: 10, unicode: false),
                        Amount1 = c.Double(),
                        Amount2 = c.Double(),
                        AmountUSD = c.Double(nullable: false),
                        Rate = c.Double(nullable: false),
                        SpotRate = c.Double(),
                        ContractDate = c.DateTime(nullable: false),
                        MaturityDate = c.DateTime(),
                        CallPut = c.Int(nullable: false),
                        Commission = c.Double(),
                        Comment = c.String(maxLength: 500),
                        IsDelivery = c.Boolean(nullable: false),
                        IsExotic = c.Boolean(nullable: false),
                        IsCanceled = c.Boolean(nullable: false),
                        TotalProfitUSD = c.Double(),
                    })
                .PrimaryKey(t => t.DealId)
                .ForeignKey("dbo.CompanyAccounts", t => t.CompanyAccountId)
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .ForeignKey("dbo.Users", t => t.ProviderUserId)
                .ForeignKey("dbo.Queries", t => t.QueryId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CompanyAccountId)
                .Index(t => t.ProviderId)
                .Index(t => t.ProviderUserId)
                .Index(t => t.QueryId);
            
            CreateTable(
                "dbo.HistoricalFXRates",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        Time = c.DateTime(nullable: false),
                        Bid = c.Double(nullable: false),
                        Ask = c.Double(nullable: false),
                        Mid = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.Key, t.Time })
                .ForeignKey("dbo.FXRates", t => t.Key)
                .Index(t => t.Key);
            
            CreateTable(
                "dbo.LogInfo",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 4000),
                        ExtendedDescription = c.String(),
                        ApplicationName = c.String(maxLength: 30),
                        UserId = c.Int(),
                        UserIP = c.String(),
                        SessionId = c.String(maxLength: 30),
                        LogLevel = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        OperationLevel = c.Int(nullable: false),
                        OperationName = c.String(maxLength: 200),
                        OperationStatus = c.Int(),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.QueriesPerProvider",
                c => new
                    {
                        QueryId = c.Long(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        ProviderApprovedBuyRate = c.Double(),
                        ProviderApprovedSellRate = c.Double(),
                        ProviderLastAction = c.Int(),
                        CustomerLastAction = c.Int(),
                        BankResponseTime = c.DateTime(),
                        UserResponseTime = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.QueryId, t.ProviderId })
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .ForeignKey("dbo.Queries", t => t.QueryId)
                .Index(t => t.QueryId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.Spreads",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        CompanyId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        IsStartingSpread = c.Boolean(nullable: false),
                        Spread = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Key, t.CompanyId, t.ProviderId, t.IsStartingSpread })
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.FXRates", t => t.Key)
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .Index(t => t.Key)
                .Index(t => t.CompanyId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.UserMessages",
                c => new
                    {
                        MessageId = c.Long(nullable: false, identity: true),
                        FromUserId = c.Int(nullable: false),
                        ToUserId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Text = c.String(maxLength: 2000),
                        IsRemoved = c.Boolean(nullable: false),
                        Priority = c.Short(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        IsImportant = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Users", t => t.FromUserId)
                .ForeignKey("dbo.Users", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
            CreateTable(
                "dbo.UserDataCompanyDatas",
                c => new
                    {
                        UserData_UserId = c.Int(nullable: false),
                        CompanyData_CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserData_UserId, t.CompanyData_CompanyId })
                .ForeignKey("dbo.Users", t => t.UserData_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.CompanyData_CompanyId, cascadeDelete: true)
                .Index(t => t.UserData_UserId)
                .Index(t => t.CompanyData_CompanyId);
            
            CreateTable(
                "dbo.ProviderDataUserDatas",
                c => new
                    {
                        ProviderData_ProviderId = c.Int(nullable: false),
                        UserData_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProviderData_ProviderId, t.UserData_UserId })
                .ForeignKey("dbo.Providers", t => t.ProviderData_ProviderId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserData_UserId, cascadeDelete: true)
                .Index(t => t.ProviderData_ProviderId)
                .Index(t => t.UserData_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMessages", "ToUserId", "dbo.Users");
            DropForeignKey("dbo.UserMessages", "FromUserId", "dbo.Users");
            DropForeignKey("dbo.Spreads", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Spreads", "Key", "dbo.FXRates");
            DropForeignKey("dbo.Spreads", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.QueriesPerProvider", "QueryId", "dbo.Queries");
            DropForeignKey("dbo.QueriesPerProvider", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.LogInfo", "UserId", "dbo.Users");
            DropForeignKey("dbo.HistoricalFXRates", "Key", "dbo.FXRates");
            DropForeignKey("dbo.Deals", "UserId", "dbo.Users");
            DropForeignKey("dbo.Deals", "QueryId", "dbo.Queries");
            DropForeignKey("dbo.Deals", "ProviderUserId", "dbo.Users");
            DropForeignKey("dbo.Deals", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Deals", "CompanyAccountId", "dbo.CompanyAccounts");
            DropForeignKey("dbo.DailyFXRates", "Key", "dbo.FXRates");
            DropForeignKey("dbo.ChatEntries", "UserId", "dbo.Users");
            DropForeignKey("dbo.ChatEntries", "QueryId", "dbo.Queries");
            DropForeignKey("dbo.Queries", "UserId", "dbo.Users");
            DropForeignKey("dbo.Queries", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Queries", "CompanyAccountId", "dbo.CompanyAccounts");
            DropForeignKey("dbo.ChatEntries", "ChatSessionId", "dbo.ChatSessions");
            DropForeignKey("dbo.ChatSessions", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.ChatSessions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ProviderDataUserDatas", "UserData_UserId", "dbo.Users");
            DropForeignKey("dbo.ProviderDataUserDatas", "ProviderData_ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Providers", "ContactDetailsId", "dbo.ContactDetails");
            DropForeignKey("dbo.ProviderAccounts", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.ProviderAccounts", "CompanyAccountId", "dbo.CompanyAccounts");
            DropForeignKey("dbo.UserFavorites", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "ContactDetailsId", "dbo.ContactDetails");
            DropForeignKey("dbo.UserDataCompanyDatas", "CompanyData_CompanyId", "dbo.Companies");
            DropForeignKey("dbo.UserDataCompanyDatas", "UserData_UserId", "dbo.Users");
            DropForeignKey("dbo.UserActions", "UserId", "dbo.Users");
            DropForeignKey("dbo.Companies", "ContactDetailsId", "dbo.ContactDetails");
            DropForeignKey("dbo.CompanyAccounts", "CompanyId", "dbo.Companies");
            DropIndex("dbo.ProviderDataUserDatas", new[] { "UserData_UserId" });
            DropIndex("dbo.ProviderDataUserDatas", new[] { "ProviderData_ProviderId" });
            DropIndex("dbo.UserDataCompanyDatas", new[] { "CompanyData_CompanyId" });
            DropIndex("dbo.UserDataCompanyDatas", new[] { "UserData_UserId" });
            DropIndex("dbo.UserMessages", new[] { "ToUserId" });
            DropIndex("dbo.UserMessages", new[] { "FromUserId" });
            DropIndex("dbo.Spreads", new[] { "ProviderId" });
            DropIndex("dbo.Spreads", new[] { "CompanyId" });
            DropIndex("dbo.Spreads", new[] { "Key" });
            DropIndex("dbo.QueriesPerProvider", new[] { "ProviderId" });
            DropIndex("dbo.QueriesPerProvider", new[] { "QueryId" });
            DropIndex("dbo.LogInfo", new[] { "UserId" });
            DropIndex("dbo.HistoricalFXRates", new[] { "Key" });
            DropIndex("dbo.Deals", new[] { "QueryId" });
            DropIndex("dbo.Deals", new[] { "ProviderUserId" });
            DropIndex("dbo.Deals", new[] { "ProviderId" });
            DropIndex("dbo.Deals", new[] { "CompanyAccountId" });
            DropIndex("dbo.Deals", new[] { "UserId" });
            DropIndex("dbo.DailyFXRates", new[] { "Key" });
            DropIndex("dbo.Queries", new[] { "ProviderId" });
            DropIndex("dbo.Queries", new[] { "CompanyAccountId" });
            DropIndex("dbo.Queries", new[] { "UserId" });
            DropIndex("dbo.ProviderAccounts", new[] { "BankAccountName" });
            DropIndex("dbo.ProviderAccounts", new[] { "ProviderId" });
            DropIndex("dbo.ProviderAccounts", new[] { "CompanyAccountId" });
            DropIndex("dbo.Providers", new[] { "ContactDetailsId" });
            DropIndex("dbo.Providers", new[] { "FullName" });
            DropIndex("dbo.Providers", new[] { "ShortName" });
            DropIndex("dbo.UserFavorites", new[] { "UserId" });
            DropIndex("dbo.UserActions", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "ContactDetailsId" });
            DropIndex("dbo.Users", new[] { "IsActive" });
            DropIndex("dbo.Users", new[] { "UserName" });
            DropIndex("dbo.CompanyAccounts", new[] { "CompanyId" });
            DropIndex("dbo.CompanyAccounts", new[] { "AccountName" });
            DropIndex("dbo.Companies", new[] { "ContactDetailsId" });
            DropIndex("dbo.Companies", new[] { "CompanyShortName" });
            DropIndex("dbo.ChatSessions", new[] { "ProviderId" });
            DropIndex("dbo.ChatSessions", new[] { "CompanyId" });
            DropIndex("dbo.ChatEntries", new[] { "QueryId" });
            DropIndex("dbo.ChatEntries", new[] { "UserId" });
            DropIndex("dbo.ChatEntries", new[] { "ChatSessionId" });
            DropTable("dbo.ProviderDataUserDatas");
            DropTable("dbo.UserDataCompanyDatas");
            DropTable("dbo.UserMessages");
            DropTable("dbo.Spreads");
            DropTable("dbo.QueriesPerProvider");
            DropTable("dbo.LogInfo");
            DropTable("dbo.HistoricalFXRates");
            DropTable("dbo.Deals");
            DropTable("dbo.FXRates");
            DropTable("dbo.DailyFXRates");
            DropTable("dbo.Configuration");
            DropTable("dbo.Queries");
            DropTable("dbo.ProviderAccounts");
            DropTable("dbo.Providers");
            DropTable("dbo.UserFavorites");
            DropTable("dbo.UserActions");
            DropTable("dbo.Users");
            DropTable("dbo.ContactDetails");
            DropTable("dbo.CompanyAccounts");
            DropTable("dbo.Companies");
            DropTable("dbo.ChatSessions");
            DropTable("dbo.ChatEntries");
        }
    }
}
