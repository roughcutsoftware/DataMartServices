using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Roughcut.DataMartServices.Core.Enums;
using Roughcut.DataMartServices.Infrastructure.DbContexts;
using Roughcut.DataMartServices.Infrastructure.DbModels;
using Roughcut.DataMartServices.Infrastructure.Helpers;
using Roughcut.DataMartServices.Infrastructure.Services;
using Shouldly;

namespace Roughcut.DataMartServices.Tests.Integration
{
    public class DataMartServiceTests
    {
        // arrange
        // act
        // assert
        private DataMartServicesDbContext db;

        private string _dbConnString = null;

        //private DataMartServiceTests(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public IConfiguration Configuration { get; }

        public DataMartServiceTests()
        {

            // https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getexecutingassembly?view=net-5.0
            Assembly assembly = Assembly.GetExecutingAssembly();

            // google search:
            // https://www.google.com/search?q=how+to+use+Microsoft.Extensions.Configuration
            // https://swimburger.net/blog/dotnet/using-configurationproviders-from-microsoft-extensions-configuration-on-dotnet-framework
            var configuration = new ConfigurationBuilder()
                
                //
                .AddUserSecrets(assembly, optional: false)

                //.AddEnvironmentVariables()
                .Build();

            //
            Configuration = configuration;
        }




        [SetUp]
        public void Setup()
        {
            //
            this._dbConnString = Configuration["ConnectionString:DataMartServicesDbContext"];

            //
            db = new DataMartServicesDbContext(this._dbConnString);

        }

        [TearDown]
        public void TearDown()
        {
            db.Dispose();

        }

        [TestCase()]
        public void Should_DoSomething()
        {

            // arrange
            //DataMartService service = new DataMartService();
            DateTime beginDateTimeYear = DateTime.Now;
            int numberOfYears = 1;
            DateTimeGrainTypes grain = DateTimeGrainTypes.ThirtyMinutes;

            // act
            // purge/reset table
            long result = DataMartService.PurgeDimDateTimeTable(this._dbConnString);

            //
            DataMartService.CreateDateTimeDimension(this._dbConnString, beginDateTimeYear, numberOfYears, grain);

            // unknown issue when attempt this line of code - returns 'sql-login-failure' - implementing 'hack-work-around'
            // int rowsCount = db.DimDateTime.Count();
            long rowsCount = DataMartService.GetTableRowCount(this._dbConnString, tableName: "DimDateTime");

            // assert
            rowsCount.ShouldBe(35040);
        }
    }
}