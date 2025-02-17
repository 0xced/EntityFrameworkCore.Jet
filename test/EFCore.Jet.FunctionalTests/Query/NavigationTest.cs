// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Jet.FunctionalTests.TestUtilities;
using EntityFrameworkCore.Jet.Data;
using System.Data.OleDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable InconsistentNaming
namespace EntityFrameworkCore.Jet.FunctionalTests.Query
{
    public class NavigationTest : IClassFixture<NavigationTestFixture>
    {
        [ConditionalFact]
        public void Duplicate_entries_are_not_created_for_navigations_to_principal()
        {
            using var context = _fixture.CreateContext();
            context.ConfigAction = modelBuilder =>
            {
                modelBuilder.Entity<GoTPerson>().HasMany(p => p.Siblings).WithOne(p => p.SiblingReverse).IsRequired(false);
                modelBuilder.Entity<GoTPerson>().HasOne(p => p.Lover).WithOne(p => p.LoverReverse).IsRequired(false);
                return 0;
            };

            var model = context.Model;
            var entityType = model.GetEntityTypes().First();

            Assert.Equal(
                "ForeignKey: GoTPerson {'LoverId'} -> GoTPerson {'Id'} Unique ToDependent: LoverReverse ToPrincipal: Lover ClientSetNull",
                entityType.GetForeignKeys().First().ToString());

            Assert.Equal(
                "ForeignKey: GoTPerson {'SiblingReverseId'} -> GoTPerson {'Id'} ToDependent: Siblings ToPrincipal: SiblingReverse ClientSetNull",
                entityType.GetForeignKeys().Skip(1).First().ToString());
        }

        [ConditionalFact]
        public void Duplicate_entries_are_not_created_for_navigations_to_dependent()
        {
            using var context = _fixture.CreateContext();
            context.ConfigAction = modelBuilder =>
            {
                modelBuilder.Entity<GoTPerson>().HasOne(p => p.SiblingReverse).WithMany(p => p.Siblings).IsRequired(false);
                modelBuilder.Entity<GoTPerson>().HasOne(p => p.Lover).WithOne(p => p.LoverReverse).IsRequired(false);
                return 0;
            };

            var model = context.Model;
            var entityType = model.GetEntityTypes().First();

            Assert.Equal(
                "ForeignKey: GoTPerson {'LoverId'} -> GoTPerson {'Id'} Unique ToDependent: LoverReverse ToPrincipal: Lover ClientSetNull",
                entityType.GetForeignKeys().First().ToString());

            Assert.Equal(
                "ForeignKey: GoTPerson {'SiblingReverseId'} -> GoTPerson {'Id'} ToDependent: Siblings ToPrincipal: SiblingReverse ClientSetNull",
                entityType.GetForeignKeys().Skip(1).First().ToString());
        }

        private readonly NavigationTestFixture _fixture;

        public NavigationTest(NavigationTestFixture fixture) => _fixture = fixture;
    }

    public class GoTPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<GoTPerson> Siblings { get; set; }
        public GoTPerson Lover { get; set; }
        public GoTPerson LoverReverse { get; set; }
        public GoTPerson SiblingReverse { get; set; }
    }

    public class GoTContext : DbContext
    {
        public GoTContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<GoTPerson> People { get; set; }
        public Func<ModelBuilder, int> ConfigAction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => ConfigAction.Invoke(modelBuilder);
    }

    public class NavigationTestFixture
    {
        private readonly DbContextOptions _options;

        public NavigationTestFixture()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkJet()
                .BuildServiceProvider(validateScopes: true);

            var dataAccessProviderType = JetConnection.GetDataAccessProviderType(TestEnvironment.DefaultConnection);
            var dataAccessProviderFactory = JetFactory.Instance.GetDataAccessProviderFactory(dataAccessProviderType);
            var connectionStringBuilder = dataAccessProviderFactory.CreateConnectionStringBuilder();
            connectionStringBuilder.ConnectionString = TestEnvironment.DefaultConnection;
            connectionStringBuilder.SetDataSource("StateManagerBug.accdb");

            _options = new DbContextOptionsBuilder()
                .UseJet(connectionStringBuilder.ConnectionString, TestEnvironment.DataAccessProviderFactory, b => b.ApplyConfiguration())
                .UseInternalServiceProvider(serviceProvider)
                .Options;
        }

        public virtual GoTContext CreateContext() => new GoTContext(_options);
    }
}
