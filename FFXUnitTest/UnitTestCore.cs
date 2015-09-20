using System;  
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlatFX;
using FlatFX.Model.Data;
using FlatFX.Model;


namespace FFXUnitTest
{
    /*
            On Exception: The model backing the 'FfxContext' context has changed since the database was created
            Perform the following:
            - Tools -> NuGet Package Manager -> Package Manager Console
            - Run Command: Update-Database -Verbose
            - If case an exception of type "Automatic migration was not applied because it would result in data loss" occurs
              consider using the following command: 
              Update-Database -Verbose -Force
         * 
         * 
         * 
         * For the first initialization of the Database instance:
         * Database.SetInitializer<EFDbContext>(new CreateDatabaseIfNotExists<EFDbContext>());  
         * context.Database.Create();
        */
    /// <summary>
    /// FlatFX Testing project
    /// </summary>
    [TestClass]
    public partial class FFXUnitTest
    {
        public FFXUnitTest()
        {
        }
    }
}