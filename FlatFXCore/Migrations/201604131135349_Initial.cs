namespace FlatFXCore.Migrations
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
                        UserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        Text = c.String(maxLength: 4000, storeType: "nvarchar"),
                        Time = c.DateTime(nullable: false, precision: 0),
                        QueryId = c.Long(nullable: false),
                        EntryType = c.Int(nullable: false),
                        ObjectData = c.String(maxLength: 4000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ChatEntryId)
                .ForeignKey("dbo.ChatSessions", t => t.ChatSessionId, cascadeDelete: true)
                .ForeignKey("dbo.Queries", t => t.QueryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ChatSessionId)
                .Index(t => t.UserId)
                .Index(t => t.QueryId);
            
            CreateTable(
                "dbo.ChatSessions",
                c => new
                    {
                        ChatSessionId = c.Long(nullable: false, identity: true),
                        CompanyId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                        StartTime = c.DateTime(nullable: false, precision: 0),
                        EndTime = c.DateTime(precision: 0),
                        ChatListenerUsers = c.String(maxLength: 200, storeType: "nvarchar"),
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
                        CompanyId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        CompanyName = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        CompanyFullName = c.String(maxLength: 200, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(precision: 0),
                        LastUpdate = c.DateTime(precision: 0),
                        ValidIP = c.String(maxLength: 300, storeType: "nvarchar"),
                        CustomerType = c.Int(),
                        IsDepositValid = c.Boolean(nullable: false),
                        IsSignOnRegistrationAgreement = c.Boolean(nullable: false),
                        CompanyVolumePerYearUSD = c.Int(),
                        UserList_SendEmail = c.String(maxLength: 800, storeType: "nvarchar"),
                        UserList_SendInvoice = c.String(maxLength: 800, storeType: "nvarchar"),
                        Email1 = c.String(maxLength: 200, storeType: "nvarchar"),
                        Email2 = c.String(maxLength: 200, storeType: "nvarchar"),
                        OfficePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        OfficePhone2 = c.String(maxLength: 30, storeType: "nvarchar"),
                        Fax = c.String(maxLength: 30, storeType: "nvarchar"),
                        HomePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        MobilePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        MobilePhone2 = c.String(maxLength: 30, storeType: "nvarchar"),
                        CarPhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        Address = c.String(maxLength: 400, storeType: "nvarchar"),
                        Country = c.Int(),
                        WebSite = c.String(maxLength: 400, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.CompanyId)
                .Index(t => t.CompanyName, unique: true);
            
            CreateTable(
                "dbo.CompanyAccounts",
                c => new
                    {
                        CompanyAccountId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        CompanyId = c.String(maxLength: 128, storeType: "nvarchar"),
                        AccountName = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                        IsDefaultAccount = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyAccountId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId)
                .Index(t => t.AccountName, unique: true);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        FirstName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        MiddleName = c.String(maxLength: 100, storeType: "nvarchar"),
                        LastName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                        RoleInCompany = c.String(maxLength: 50, storeType: "nvarchar"),
                        Language = c.Int(nullable: false),
                        SigningKey = c.String(maxLength: 16, storeType: "nvarchar"),
                        InvoiceCurrency = c.Int(nullable: false),
                        IsApprovedByFlatFX = c.Boolean(nullable: false),
                        Email1 = c.String(maxLength: 200, storeType: "nvarchar"),
                        Email2 = c.String(maxLength: 200, storeType: "nvarchar"),
                        OfficePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        OfficePhone2 = c.String(maxLength: 30, storeType: "nvarchar"),
                        Fax = c.String(maxLength: 30, storeType: "nvarchar"),
                        HomePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        MobilePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        MobilePhone2 = c.String(maxLength: 30, storeType: "nvarchar"),
                        CarPhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        Address = c.String(maxLength: 400, storeType: "nvarchar"),
                        Country = c.Int(),
                        WebSite = c.String(maxLength: 400, storeType: "nvarchar"),
                        Email = c.String(maxLength: 256, storeType: "nvarchar"),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(unicode: false),
                        SecurityStamp = c.String(unicode: false),
                        PhoneNumber = c.String(unicode: false),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 0),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IsActive)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.UserActions",
                c => new
                    {
                        ActionId = c.Long(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        Time = c.DateTime(nullable: false, precision: 0),
                        Text = c.String(maxLength: 500, storeType: "nvarchar"),
                        Priority = c.Short(nullable: false),
                        IsSucceded = c.Boolean(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ActionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ClaimType = c.String(unicode: false),
                        ClaimValue = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserFavorites",
                c => new
                    {
                        FavoriteId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        Category = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Text = c.String(nullable: false, maxLength: 400, storeType: "nvarchar"),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FavoriteId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ProviderKey = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Providers",
                c => new
                    {
                        ProviderId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Name = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        FullName = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        BankNumber = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        ProviderType = c.Int(nullable: false),
                        QuoteResponse_Enabled = c.Boolean(nullable: false),
                        QuoteResponse_SpreadMethod = c.Int(nullable: false),
                        QuoteResponse_StartTime = c.DateTime(nullable: false, precision: 0),
                        QuoteResponse_EndTime = c.DateTime(nullable: false, precision: 0),
                        QuoteResponse_FridayStartTime = c.DateTime(nullable: false, precision: 0),
                        QuoteResponse_FridayEndTime = c.DateTime(nullable: false, precision: 0),
                        QuoteResponse_UserConfirmationTimeInterval = c.Short(nullable: false),
                        QuoteResponse_AutomaticResponseEnabled = c.Boolean(nullable: false),
                        QuoteResponse_MinRequestVolumeUSD = c.Int(nullable: false),
                        QuoteResponse_MaxDailyVolumeUSD = c.Int(nullable: false),
                        QuoteResponse_NumberOfPromilsWithoutDiscount = c.Double(nullable: false),
                        Email1 = c.String(maxLength: 200, storeType: "nvarchar"),
                        Email2 = c.String(maxLength: 200, storeType: "nvarchar"),
                        OfficePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        OfficePhone2 = c.String(maxLength: 30, storeType: "nvarchar"),
                        Fax = c.String(maxLength: 30, storeType: "nvarchar"),
                        HomePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        MobilePhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        MobilePhone2 = c.String(maxLength: 30, storeType: "nvarchar"),
                        CarPhone = c.String(maxLength: 30, storeType: "nvarchar"),
                        Address = c.String(maxLength: 400, storeType: "nvarchar"),
                        Country = c.Int(),
                        WebSite = c.String(maxLength: 400, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ProviderId)
                .Index(t => t.Name, unique: true, name: "IX_ProviderName")
                .Index(t => t.FullName, unique: true);
            
            CreateTable(
                "dbo.ProviderAccounts",
                c => new
                    {
                        CompanyAccountId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ProviderId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        AccountName = c.String(maxLength: 100, storeType: "nvarchar"),
                        BankAccountName = c.String(maxLength: 100, storeType: "nvarchar"),
                        BankBranchNumber = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        BankAccountNumber = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        BankAddress = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        IBAN = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        SWIFT = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        AllowToTradeDirectlly = c.Boolean(nullable: false),
                        ApprovedBYFlatFX = c.Boolean(nullable: false),
                        ApprovedBYProvider = c.Boolean(nullable: false),
                        UserKeyInProviderSystems = c.String(unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDemoAccount = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(precision: 0),
                        LastUpdate = c.DateTime(precision: 0),
                        LastUpdateBy = c.String(unicode: false),
                        QuoteResponse_IsBlocked = c.Boolean(nullable: false),
                        QuoteResponse_CustomerPromil = c.Double(),
                    })
                .PrimaryKey(t => new { t.CompanyAccountId, t.ProviderId })
                .ForeignKey("dbo.CompanyAccounts", t => t.CompanyAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Providers", t => t.ProviderId, cascadeDelete: true)
                .Index(t => t.CompanyAccountId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.Deals",
                c => new
                    {
                        DealId = c.Long(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderUserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderDealRef = c.String(maxLength: 100, storeType: "nvarchar"),
                        QueryId = c.Long(),
                        DealType = c.Int(nullable: false),
                        DealProductType = c.Int(nullable: false),
                        BuySell = c.Int(nullable: false),
                        Symbol = c.String(nullable: false, maxLength: 10, unicode: false),
                        CreditedCurrency = c.String(unicode: false),
                        AmountToExchangeCreditedCurrency = c.Double(nullable: false),
                        ChargedCurrency = c.String(unicode: false),
                        AmountToExchangeChargedCurrency = c.Double(nullable: false),
                        AmountUSD = c.Double(nullable: false),
                        CustomerRate = c.Double(nullable: false),
                        BankRate = c.Double(),
                        MidRate = c.Double(),
                        OfferingDate = c.DateTime(nullable: false, precision: 0),
                        ContractDate = c.DateTime(precision: 0),
                        MaturityDate = c.DateTime(precision: 0),
                        CallPut = c.Int(nullable: false),
                        Commission = c.Double(),
                        Comment = c.String(maxLength: 500, storeType: "nvarchar"),
                        IsDelivery = c.Boolean(nullable: false),
                        IsExotic = c.Boolean(nullable: false),
                        IsCanceled = c.Boolean(nullable: false),
                        IsOffer = c.Boolean(nullable: false),
                        IsDemo = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        StatusDetails = c.String(maxLength: 500, storeType: "nvarchar"),
                        CustomerTotalProfitUSD = c.Double(),
                        FlatFXIncomeUSD = c.Double(),
                        BankIncomeUSD = c.Double(),
                        HandleBy = c.String(unicode: false),
                        EnsureOnLinePrice = c.Boolean(nullable: false),
                        PvPEnabled = c.Boolean(nullable: false),
                        FastTransferEnabled = c.Boolean(nullable: false),
                        OfferingMidRate = c.Double(),
                        ChargedAccount_CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ChargedAccount_ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CreditedAccount_CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CreditedAccount_ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderAccount_CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderAccount_ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.DealId)
                .ForeignKey("dbo.ProviderAccounts", t => new { t.ChargedAccount_CompanyAccountId, t.ChargedAccount_ProviderId })
                .ForeignKey("dbo.CompanyAccounts", t => t.CompanyAccountId)
                .ForeignKey("dbo.ProviderAccounts", t => new { t.CreditedAccount_CompanyAccountId, t.CreditedAccount_ProviderId })
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .ForeignKey("dbo.AspNetUsers", t => t.ProviderUserId)
                .ForeignKey("dbo.Queries", t => t.QueryId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.ProviderAccounts", t => new { t.ProviderAccount_CompanyAccountId, t.ProviderAccount_ProviderId })
                .Index(t => t.UserId)
                .Index(t => t.CompanyAccountId)
                .Index(t => t.ProviderId)
                .Index(t => t.ProviderUserId)
                .Index(t => t.QueryId)
                .Index(t => new { t.ChargedAccount_CompanyAccountId, t.ChargedAccount_ProviderId })
                .Index(t => new { t.CreditedAccount_CompanyAccountId, t.CreditedAccount_ProviderId })
                .Index(t => new { t.ProviderAccount_CompanyAccountId, t.ProviderAccount_ProviderId });
            
            CreateTable(
                "dbo.Queries",
                c => new
                    {
                        QueryId = c.Long(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                        Symbol = c.String(nullable: false, maxLength: 10, unicode: false),
                        DealType = c.Int(nullable: false),
                        BuySell = c.Int(nullable: false),
                        AmountCCY1 = c.Double(nullable: false),
                        AmountUSD = c.Double(),
                    })
                .PrimaryKey(t => t.QueryId)
                .ForeignKey("dbo.CompanyAccounts", t => t.CompanyAccountId)
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CompanyAccountId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        RoleId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Configuration",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Value = c.String(nullable: false, maxLength: 2000, storeType: "nvarchar"),
                        Description = c.String(nullable: false, maxLength: 2000, storeType: "nvarchar"),
                        AutomaticDisplay = c.Boolean(nullable: false),
                        TabName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        UserOption = c.Boolean(nullable: false),
                        DisplayTag = c.String(maxLength: 100, storeType: "nvarchar"),
                        Type = c.Int(nullable: false),
                        MinValue = c.Int(),
                        MaxValue = c.Int(),
                        TabOrder = c.Int(nullable: false),
                        DefaultValue = c.String(nullable: false, maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Key, t.UserId });
            
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.DailyFXRates",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        Time = c.DateTime(nullable: false, precision: 0),
                        Bid = c.Double(nullable: false),
                        Ask = c.Double(nullable: false),
                        Mid = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.Key, t.Time })
                .ForeignKey("dbo.FXRates", t => t.Key, cascadeDelete: true)
                .Index(t => t.Key);
            
            CreateTable(
                "dbo.FXRates",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        Bid = c.Double(nullable: false),
                        Ask = c.Double(nullable: false),
                        Mid = c.Double(nullable: false),
                        LastUpdate = c.DateTime(nullable: false, precision: 0),
                        IsActive = c.Boolean(nullable: false),
                        IsTradable = c.Boolean(nullable: false),
                        IsActiveForSimpleTrading = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.GenericDictionaryItems",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 300, storeType: "nvarchar"),
                        Category = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Info1 = c.String(maxLength: 200, storeType: "nvarchar"),
                        Info2 = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Key, t.Category });
            
            CreateTable(
                "dbo.HistoricalFXRates",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 10, unicode: false),
                        Time = c.DateTime(nullable: false, precision: 0),
                        Bid = c.Double(nullable: false),
                        Ask = c.Double(nullable: false),
                        Mid = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.Key, t.Time })
                .ForeignKey("dbo.FXRates", t => t.Key, cascadeDelete: true)
                .Index(t => t.Key);
            
            CreateTable(
                "dbo.LogInfo",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 4000, storeType: "nvarchar"),
                        ExtendedDescription = c.String(unicode: false),
                        ApplicationName = c.String(maxLength: 30, storeType: "nvarchar"),
                        UserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        UserIP = c.String(unicode: false),
                        SessionId = c.String(maxLength: 30, storeType: "nvarchar"),
                        LogLevel = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        OperationLevel = c.Int(nullable: false),
                        OperationName = c.String(maxLength: 200, storeType: "nvarchar"),
                        OperationStatus = c.Int(),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.OrderMatch",
                c => new
                    {
                        MatchId = c.Long(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        TriggerSource = c.Int(nullable: false),
                        TriggerDate = c.DateTime(nullable: false, precision: 0),
                        MaturityDate = c.DateTime(precision: 0),
                        CloseDate = c.DateTime(precision: 0),
                        HandleBy = c.String(unicode: false),
                        MidRate = c.Double(),
                        Deal1_DealId = c.Long(),
                        Deal2_DealId = c.Long(),
                        Order1_OrderId = c.Long(),
                        Order2_OrderId = c.Long(),
                    })
                .PrimaryKey(t => t.MatchId)
                .ForeignKey("dbo.Deals", t => t.Deal1_DealId)
                .ForeignKey("dbo.Deals", t => t.Deal2_DealId)
                .ForeignKey("dbo.Order", t => t.Order1_OrderId)
                .ForeignKey("dbo.Order", t => t.Order2_OrderId)
                .Index(t => t.Deal1_DealId)
                .Index(t => t.Deal2_DealId)
                .Index(t => t.Order1_OrderId)
                .Index(t => t.Order2_OrderId);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        OrderId = c.Long(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        DealType = c.Int(nullable: false),
                        DealProductType = c.Int(nullable: false),
                        BuySell = c.Int(nullable: false),
                        Symbol = c.String(nullable: false, maxLength: 10, unicode: false),
                        AmountCCY1 = c.Double(nullable: false),
                        AmountCCY2_Estimation = c.Double(nullable: false),
                        AmountUSD_Estimation = c.Double(nullable: false),
                        OrderDate = c.DateTime(nullable: false, precision: 0),
                        Comment = c.String(maxLength: 500, storeType: "nvarchar"),
                        IsDemo = c.Boolean(nullable: false),
                        CustomerTotalProfitUSD_Estimation = c.Double(),
                        FlatFXCommissionUSD_Estimation = c.Double(),
                        MinimalPartnerExecutionAmountCCY1 = c.Double(),
                        ExpiryDate = c.DateTime(precision: 0),
                        MinimalPartnerTotalVolumeUSD = c.Double(),
                        PartnerMinScore = c.Int(),
                        AmountCCY1_Executed = c.Double(),
                        AmountCCY1_Remainder = c.Double(),
                        Status = c.Int(nullable: false),
                        StatusDetails = c.String(maxLength: 500, storeType: "nvarchar"),
                        ChargedAccount_CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ChargedAccount_ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CreditedAccount_CompanyAccountId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CreditedAccount_ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.ProviderAccounts", t => new { t.ChargedAccount_CompanyAccountId, t.ChargedAccount_ProviderId })
                .ForeignKey("dbo.CompanyAccounts", t => t.CompanyAccountId)
                .ForeignKey("dbo.ProviderAccounts", t => new { t.CreditedAccount_CompanyAccountId, t.CreditedAccount_ProviderId })
                .ForeignKey("dbo.Providers", t => t.ProviderId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CompanyAccountId)
                .Index(t => t.ProviderId)
                .Index(t => new { t.ChargedAccount_CompanyAccountId, t.ChargedAccount_ProviderId })
                .Index(t => new { t.CreditedAccount_CompanyAccountId, t.CreditedAccount_ProviderId });
            
            CreateTable(
                "dbo.QueriesPerProvider",
                c => new
                    {
                        QueryId = c.Long(nullable: false),
                        ProviderId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ProviderApprovedBuyRate = c.Double(),
                        ProviderApprovedSellRate = c.Double(),
                        ProviderLastAction = c.Int(),
                        CustomerLastAction = c.Int(),
                        BankResponseTime = c.DateTime(precision: 0),
                        UserResponseTime = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => new { t.QueryId, t.ProviderId })
                .ForeignKey("dbo.Providers", t => t.ProviderId, cascadeDelete: true)
                .ForeignKey("dbo.Queries", t => t.QueryId, cascadeDelete: true)
                .Index(t => t.QueryId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Name = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Spreads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 10, unicode: false),
                        CompanyId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProviderId = c.String(maxLength: 128, storeType: "nvarchar"),
                        Spread = c.Double(),
                        Promil = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
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
                        FromUserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ToUserId = c.String(maxLength: 128, storeType: "nvarchar"),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        Text = c.String(maxLength: 2000, storeType: "nvarchar"),
                        IsRemoved = c.Boolean(nullable: false),
                        Priority = c.Short(nullable: false),
                        DueDate = c.DateTime(nullable: false, precision: 0),
                        IsCompleted = c.Boolean(nullable: false),
                        IsImportant = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.FromUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
            CreateTable(
                "dbo.ApplicationUserCompanies",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Company_CompanyId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Company_CompanyId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.Company_CompanyId, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Company_CompanyId);
            
            CreateTable(
                "dbo.ProviderApplicationUsers",
                c => new
                    {
                        Provider_ProviderId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Provider_ProviderId, t.ApplicationUser_Id })
                .ForeignKey("dbo.Providers", t => t.Provider_ProviderId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Provider_ProviderId)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMessages", "ToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserMessages", "FromUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Spreads", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Spreads", "Key", "dbo.FXRates");
            DropForeignKey("dbo.Spreads", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.QueriesPerProvider", "QueryId", "dbo.Queries");
            DropForeignKey("dbo.QueriesPerProvider", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.OrderMatch", "Order2_OrderId", "dbo.Order");
            DropForeignKey("dbo.OrderMatch", "Order1_OrderId", "dbo.Order");
            DropForeignKey("dbo.Order", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Order", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Order", new[] { "CreditedAccount_CompanyAccountId", "CreditedAccount_ProviderId" }, "dbo.ProviderAccounts");
            DropForeignKey("dbo.Order", "CompanyAccountId", "dbo.CompanyAccounts");
            DropForeignKey("dbo.Order", new[] { "ChargedAccount_CompanyAccountId", "ChargedAccount_ProviderId" }, "dbo.ProviderAccounts");
            DropForeignKey("dbo.OrderMatch", "Deal2_DealId", "dbo.Deals");
            DropForeignKey("dbo.OrderMatch", "Deal1_DealId", "dbo.Deals");
            DropForeignKey("dbo.LogInfo", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.HistoricalFXRates", "Key", "dbo.FXRates");
            DropForeignKey("dbo.DailyFXRates", "Key", "dbo.FXRates");
            DropForeignKey("dbo.ChatEntries", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChatEntries", "QueryId", "dbo.Queries");
            DropForeignKey("dbo.ChatEntries", "ChatSessionId", "dbo.ChatSessions");
            DropForeignKey("dbo.ChatSessions", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.ChatSessions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProviderApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProviderApplicationUsers", "Provider_ProviderId", "dbo.Providers");
            DropForeignKey("dbo.ProviderAccounts", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Deals", new[] { "ProviderAccount_CompanyAccountId", "ProviderAccount_ProviderId" }, "dbo.ProviderAccounts");
            DropForeignKey("dbo.Deals", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Deals", "QueryId", "dbo.Queries");
            DropForeignKey("dbo.Queries", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Queries", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Queries", "CompanyAccountId", "dbo.CompanyAccounts");
            DropForeignKey("dbo.Deals", "ProviderUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Deals", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.Deals", new[] { "CreditedAccount_CompanyAccountId", "CreditedAccount_ProviderId" }, "dbo.ProviderAccounts");
            DropForeignKey("dbo.Deals", "CompanyAccountId", "dbo.CompanyAccounts");
            DropForeignKey("dbo.Deals", new[] { "ChargedAccount_CompanyAccountId", "ChargedAccount_ProviderId" }, "dbo.ProviderAccounts");
            DropForeignKey("dbo.ProviderAccounts", "CompanyAccountId", "dbo.CompanyAccounts");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserFavorites", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserCompanies", "Company_CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ApplicationUserCompanies", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserActions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompanyAccounts", "CompanyId", "dbo.Companies");
            DropIndex("dbo.ProviderApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ProviderApplicationUsers", new[] { "Provider_ProviderId" });
            DropIndex("dbo.ApplicationUserCompanies", new[] { "Company_CompanyId" });
            DropIndex("dbo.ApplicationUserCompanies", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.UserMessages", new[] { "ToUserId" });
            DropIndex("dbo.UserMessages", new[] { "FromUserId" });
            DropIndex("dbo.Spreads", new[] { "ProviderId" });
            DropIndex("dbo.Spreads", new[] { "CompanyId" });
            DropIndex("dbo.Spreads", new[] { "Key" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.QueriesPerProvider", new[] { "ProviderId" });
            DropIndex("dbo.QueriesPerProvider", new[] { "QueryId" });
            DropIndex("dbo.Order", new[] { "CreditedAccount_CompanyAccountId", "CreditedAccount_ProviderId" });
            DropIndex("dbo.Order", new[] { "ChargedAccount_CompanyAccountId", "ChargedAccount_ProviderId" });
            DropIndex("dbo.Order", new[] { "ProviderId" });
            DropIndex("dbo.Order", new[] { "CompanyAccountId" });
            DropIndex("dbo.Order", new[] { "UserId" });
            DropIndex("dbo.OrderMatch", new[] { "Order2_OrderId" });
            DropIndex("dbo.OrderMatch", new[] { "Order1_OrderId" });
            DropIndex("dbo.OrderMatch", new[] { "Deal2_DealId" });
            DropIndex("dbo.OrderMatch", new[] { "Deal1_DealId" });
            DropIndex("dbo.LogInfo", new[] { "UserId" });
            DropIndex("dbo.HistoricalFXRates", new[] { "Key" });
            DropIndex("dbo.DailyFXRates", new[] { "Key" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Queries", new[] { "ProviderId" });
            DropIndex("dbo.Queries", new[] { "CompanyAccountId" });
            DropIndex("dbo.Queries", new[] { "UserId" });
            DropIndex("dbo.Deals", new[] { "ProviderAccount_CompanyAccountId", "ProviderAccount_ProviderId" });
            DropIndex("dbo.Deals", new[] { "CreditedAccount_CompanyAccountId", "CreditedAccount_ProviderId" });
            DropIndex("dbo.Deals", new[] { "ChargedAccount_CompanyAccountId", "ChargedAccount_ProviderId" });
            DropIndex("dbo.Deals", new[] { "QueryId" });
            DropIndex("dbo.Deals", new[] { "ProviderUserId" });
            DropIndex("dbo.Deals", new[] { "ProviderId" });
            DropIndex("dbo.Deals", new[] { "CompanyAccountId" });
            DropIndex("dbo.Deals", new[] { "UserId" });
            DropIndex("dbo.ProviderAccounts", new[] { "ProviderId" });
            DropIndex("dbo.ProviderAccounts", new[] { "CompanyAccountId" });
            DropIndex("dbo.Providers", new[] { "FullName" });
            DropIndex("dbo.Providers", "IX_ProviderName");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.UserFavorites", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.UserActions", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "IsActive" });
            DropIndex("dbo.CompanyAccounts", new[] { "AccountName" });
            DropIndex("dbo.CompanyAccounts", new[] { "CompanyId" });
            DropIndex("dbo.Companies", new[] { "CompanyName" });
            DropIndex("dbo.ChatSessions", new[] { "ProviderId" });
            DropIndex("dbo.ChatSessions", new[] { "CompanyId" });
            DropIndex("dbo.ChatEntries", new[] { "QueryId" });
            DropIndex("dbo.ChatEntries", new[] { "UserId" });
            DropIndex("dbo.ChatEntries", new[] { "ChatSessionId" });
            DropTable("dbo.ProviderApplicationUsers");
            DropTable("dbo.ApplicationUserCompanies");
            DropTable("dbo.UserMessages");
            DropTable("dbo.Spreads");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.QueriesPerProvider");
            DropTable("dbo.Order");
            DropTable("dbo.OrderMatch");
            DropTable("dbo.LogInfo");
            DropTable("dbo.HistoricalFXRates");
            DropTable("dbo.GenericDictionaryItems");
            DropTable("dbo.FXRates");
            DropTable("dbo.DailyFXRates");
            DropTable("dbo.Currency");
            DropTable("dbo.Configuration");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Queries");
            DropTable("dbo.Deals");
            DropTable("dbo.ProviderAccounts");
            DropTable("dbo.Providers");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.UserFavorites");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.UserActions");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.CompanyAccounts");
            DropTable("dbo.Companies");
            DropTable("dbo.ChatSessions");
            DropTable("dbo.ChatEntries");
        }
    }
}
