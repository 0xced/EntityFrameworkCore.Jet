﻿// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityFrameworkCore.Jet.FunctionalTests.TestUtilities;
using EntityFrameworkCore.Jet.Metadata;
using Identity30.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.AspNetIdentity;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using JetDatabaseCreator = EntityFrameworkCore.Jet.Storage.Internal.JetDatabaseCreator;

// ReSharper disable InconsistentNaming
namespace EntityFrameworkCore.Jet.FunctionalTests
{
    [JetCondition(JetCondition.IsNotCI)]
    public class MigrationsInfrastructureJetTest : MigrationsInfrastructureTestBase<MigrationsInfrastructureJetTest.MigrationsInfrastructureJetFixture>
    {
        public MigrationsInfrastructureJetTest(MigrationsInfrastructureJetFixture fixture)
            : base(fixture)
        {
        }

        public override void Can_generate_migration_from_initial_database_to_initial()
        {
            base.Can_generate_migration_from_initial_database_to_initial();

            Assert.Equal(
                @"IF NOT EXISTS (SELECT * FROM `INFORMATION_SCHEMA.TABLES` WHERE `TABLE_NAME` = '__EFMigrationsHistory') THEN CREATE TABLE `__EFMigrationsHistory` (
        `MigrationId` varchar(150) NOT NULL,
        `ProductVersion` varchar(32) NOT NULL,
        CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
    );
;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_no_migration_script()
        {
            base.Can_generate_no_migration_script();

            Assert.Equal(
                @"IF NOT EXISTS (SELECT * FROM `INFORMATION_SCHEMA.TABLES` WHERE `TABLE_NAME` = '__EFMigrationsHistory') THEN CREATE TABLE `__EFMigrationsHistory` (
        `MigrationId` varchar(150) NOT NULL,
        `ProductVersion` varchar(32) NOT NULL,
        CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
    );
;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_up_scripts()
        {
            base.Can_generate_up_scripts();

            Assert.Equal(
                @"IF NOT EXISTS (SELECT * FROM `INFORMATION_SCHEMA.TABLES` WHERE `TABLE_NAME` = '__EFMigrationsHistory') THEN CREATE TABLE `__EFMigrationsHistory` (
        `MigrationId` varchar(150) NOT NULL,
        `ProductVersion` varchar(32) NOT NULL,
        CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
    );
;

BEGIN TRANSACTION;

CREATE TABLE `Table1` (
    `Id` integer NOT NULL,
    `Foo` integer NOT NULL,
    CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_Migration1', '7.0.0-test');

COMMIT TRANSACTION;

BEGIN TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT TRANSACTION;

BEGIN TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000003_Migration3', '7.0.0-test');

COMMIT TRANSACTION;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_one_up_script()
        {
            base.Can_generate_one_up_script();

            Assert.Equal(
                @"BEGIN TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT TRANSACTION;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_up_script_using_names()
        {
            base.Can_generate_up_script_using_names();

            Assert.Equal(
                @"BEGIN TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT TRANSACTION;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_idempotent_up_scripts()
        {
            base.Can_generate_idempotent_up_scripts();

            Assert.Equal(
                @"IF OBJECT_ID('`__EFMigrationsHistory`') IS NULL
BEGIN
    CREATE TABLE `__EFMigrationsHistory` (
        `MigrationId` nvarchar(150) NOT NULL,
        `ProductVersion` nvarchar(32) NOT NULL,
        CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
    );
END;

GO

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1')
BEGIN
    CREATE TABLE `Table1` (
        `Id` int NOT NULL,
        `Foo` int NOT NULL,
        CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
    );
END;

GO

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000001_Migration1', '7.0.0-test');
END;

GO

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2')
BEGIN
    EXEC sp_rename '`Table1`.`Foo`', 'Bar', 'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000002_Migration2', '7.0.0-test');
END;

GO

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000003_Migration3')
BEGIN
    CREATE DATABASE TransactionSuppressed;
END;

GO

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000003_Migration3')
BEGIN
    DROP DATABASE TransactionSuppressed;
END;

GO

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000003_Migration3')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000003_Migration3', '7.0.0-test');
END;

GO

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_down_scripts()
        {
            base.Can_generate_down_scripts();

            Assert.Equal(
                @"BEGIN TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT TRANSACTION;

BEGIN TRANSACTION;

DROP TABLE `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000001_Migration1';

COMMIT TRANSACTION;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_idempotent_down_scripts()
        {
            base.Can_generate_idempotent_down_scripts();

            Assert.Equal(
                @"IF EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2')
BEGIN
    EXEC sp_rename '`Table1`.`Bar`', 'Foo', 'COLUMN';
END;

GO

IF EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2')
BEGIN
    DELETE FROM `__EFMigrationsHistory`
    WHERE `MigrationId` = '00000000000002_Migration2';
END;

GO

IF EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1')
BEGIN
    DROP TABLE `Table1`;
END;

GO

IF EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1')
BEGIN
    DELETE FROM `__EFMigrationsHistory`
    WHERE `MigrationId` = '00000000000001_Migration1';
END;

GO

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_one_down_script()
        {
            base.Can_generate_one_down_script();

            Assert.Equal(
                @"BEGIN TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT TRANSACTION;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_down_script_using_names()
        {
            base.Can_generate_down_script_using_names();

            Assert.Equal(
                @"BEGIN TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT TRANSACTION;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_get_active_provider()
        {
            base.Can_get_active_provider();

            Assert.Equal("EntityFrameworkCore.Jet", ActiveProvider);
        }
        
        [ConditionalFact]
        public async Task Empty_Migration_Creates_Database()
        {
            using (var context = new BloggingContext(
                Fixture.TestStore.AddProviderOptions(
                    new DbContextOptionsBuilder().EnableServiceProviderCaching(false)).Options))
            {
                var creator = (JetDatabaseCreator)context.GetService<IRelationalDatabaseCreator>();
                // creator.RetryTimeout = TimeSpan.FromMinutes(10);

                await context.Database.MigrateAsync();

                Assert.True(creator.Exists());
            }
        }

        private class BloggingContext : DbContext
        {
            public BloggingContext(DbContextOptions options)
                : base(options)
            {
            }

            // ReSharper disable once UnusedMember.Local
            public DbSet<Blog> Blogs { get; set; }

            // ReSharper disable once ClassNeverInstantiated.Local
            public class Blog
            {
                // ReSharper disable UnusedMember.Local
                public int Id { get; set; }

                public string Name { get; set; }
                // ReSharper restore UnusedMember.Local
            }
        }

        [DbContext(typeof(BloggingContext))]
        [Migration("00000000000000_Empty")]
        public class EmptyMigration : Migration
        {
            protected override void Up(MigrationBuilder migrationBuilder)
            {
            }
        }

        public override void Can_diff_against_2_2_model()
        {
            using (var context = new ModelSnapshot22.BloggingContext())
            {
                DiffSnapshot(new BloggingContextModelSnapshot22(), context);
            }
        }

        public class BloggingContextModelSnapshot22 : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "ModelSnapshot22.Blog", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("Name");

                        b.HasKey("Id");

                        b.ToTable("Blogs");

                        b.HasData(
                            new { Id = 1, Name = "HalfADonkey" });
                    });

                modelBuilder.Entity(
                    "ModelSnapshot22.Post", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<int?>("BlogId");

                        b.Property<string>("Content");

                        b.Property<DateTime>("EditDate");

                        b.Property<string>("Title");

                        b.HasKey("Id");

                        b.HasIndex("BlogId");

                        b.ToTable("Post");
                    });

                modelBuilder.Entity(
                    "ModelSnapshot22.Post", b =>
                    {
                        b.HasOne("ModelSnapshot22.Blog", "Blog")
                            .WithMany("Posts")
                            .HasForeignKey("BlogId");
                    });
#pragma warning restore 612, 618
            }
        }

        public override void Can_diff_against_2_1_ASP_NET_Identity_model()
        {
            using var context = new ApplicationDbContext();
            DiffSnapshot(new AspNetIdentity21ModelSnapshot(), context);
        }

        public class AspNetIdentity21ModelSnapshot : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "2.1.0")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRole", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Name")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedName")
                            .IsUnique()
                            .HasName("RoleNameIndex") // Don't change to HasDatabaseName
                            .HasFilter("`NormalizedName` IS NOT NULL");

                        b.ToTable("AspNetRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("RoleId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetRoleClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUser", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<int>("AccessFailedCount");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Email")
                            .HasMaxLength(256);

                        b.Property<bool>("EmailConfirmed");

                        b.Property<bool>("LockoutEnabled");

                        b.Property<DateTimeOffset?>("LockoutEnd");

                        b.Property<string>("NormalizedEmail")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedUserName")
                            .HasMaxLength(256);

                        b.Property<string>("PasswordHash");

                        b.Property<string>("PhoneNumber");

                        b.Property<bool>("PhoneNumberConfirmed");

                        b.Property<string>("SecurityStamp");

                        b.Property<bool>("TwoFactorEnabled");

                        b.Property<string>("UserName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedEmail")
                            .HasName("EmailIndex");

                        b.HasIndex("NormalizedUserName")
                            .IsUnique()
                            .HasName("UserNameIndex")
                            .HasFilter("`NormalizedUserName` IS NOT NULL");

                        b.ToTable("AspNetUsers");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderKey")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderDisplayName");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("LoginProvider", "ProviderKey");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserLogins");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("RoleId");

                        b.HasKey("UserId", "RoleId");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetUserRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("Name")
                            .HasMaxLength(128);

                        b.Property<string>("Value");

                        b.HasKey("UserId", "LoginProvider", "Name");

                        b.ToTable("AspNetUserTokens");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);

                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });
#pragma warning restore 612, 618
            }
        }

        public override void Can_diff_against_2_2_ASP_NET_Identity_model()
        {
            using var context = new ApplicationDbContext();
            DiffSnapshot(new AspNetIdentity22ModelSnapshot(), context);
        }

        public class AspNetIdentity22ModelSnapshot : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "2.2.0-preview1")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRole", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Name")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedName")
                            .IsUnique()
                            .HasName("RoleNameIndex")
                            .HasFilter("`NormalizedName` IS NOT NULL");

                        b.ToTable("AspNetRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("RoleId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetRoleClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUser", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<int>("AccessFailedCount");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Email")
                            .HasMaxLength(256);

                        b.Property<bool>("EmailConfirmed");

                        b.Property<bool>("LockoutEnabled");

                        b.Property<DateTimeOffset?>("LockoutEnd");

                        b.Property<string>("NormalizedEmail")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedUserName")
                            .HasMaxLength(256);

                        b.Property<string>("PasswordHash");

                        b.Property<string>("PhoneNumber");

                        b.Property<bool>("PhoneNumberConfirmed");

                        b.Property<string>("SecurityStamp");

                        b.Property<bool>("TwoFactorEnabled");

                        b.Property<string>("UserName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedEmail")
                            .HasName("EmailIndex");

                        b.HasIndex("NormalizedUserName")
                            .IsUnique()
                            .HasName("UserNameIndex")
                            .HasFilter("`NormalizedUserName` IS NOT NULL");

                        b.ToTable("AspNetUsers");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderKey")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderDisplayName");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("LoginProvider", "ProviderKey");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserLogins");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("RoleId");

                        b.HasKey("UserId", "RoleId");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetUserRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("Name")
                            .HasMaxLength(128);

                        b.Property<string>("Value");

                        b.HasKey("UserId", "LoginProvider", "Name");

                        b.ToTable("AspNetUserTokens");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);

                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });
#pragma warning restore 612, 618
            }
        }

        public override void Can_diff_against_3_0_ASP_NET_Identity_model()
        {
            using var context = new ApplicationDbContext();
            DiffSnapshot(new AspNetIdentity30ModelSnapshot(), context);
        }

        public class AspNetIdentity30ModelSnapshot : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "3.0.0")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRole", b =>
                    {
                        b.Property<string>("Id")
                            .HasColumnType("varchar(255)");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken()
                            .HasColumnType("longchar");

                        b.Property<string>("Name")
                            .HasColumnType("longchar")
                            .HasMaxLength(255);

                        b.Property<string>("NormalizedName")
                            .HasColumnType("longchar")
                            .HasMaxLength(255);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedName")
                            .IsUnique()
                            .HasName("RoleNameIndex")
                            .HasFilter("`NormalizedName` IS NOT NULL");

                        b.ToTable("AspNetRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasColumnType("int")
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType")
                            .HasColumnType("longchar");

                        b.Property<string>("ClaimValue")
                            .HasColumnType("longchar");

                        b.Property<string>("RoleId")
                            .IsRequired()
                            .HasColumnType("varchar(255)");

                        b.HasKey("Id");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetRoleClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUser", b =>
                    {
                        b.Property<string>("Id")
                            .HasColumnType("varchar(255)");

                        b.Property<int>("AccessFailedCount")
                            .HasColumnType("int");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken()
                            .HasColumnType("longchar");

                        b.Property<string>("Email")
                            .HasColumnType("longchar")
                            .HasMaxLength(255);

                        b.Property<bool>("EmailConfirmed")
                            .HasColumnType("smallint");

                        b.Property<bool>("LockoutEnabled")
                            .HasColumnType("smallint");

                        b.Property<DateTimeOffset?>("LockoutEnd")
                            .HasColumnType("datetime");

                        b.Property<string>("NormalizedEmail")
                            .HasColumnType("longchar")
                            .HasMaxLength(255);

                        b.Property<string>("NormalizedUserName")
                            .HasColumnType("longchar")
                            .HasMaxLength(255);

                        b.Property<string>("PasswordHash")
                            .HasColumnType("longchar");

                        b.Property<string>("PhoneNumber")
                            .HasColumnType("longchar");

                        b.Property<bool>("PhoneNumberConfirmed")
                            .HasColumnType("smallint");

                        b.Property<string>("SecurityStamp")
                            .HasColumnType("longchar");

                        b.Property<bool>("TwoFactorEnabled")
                            .HasColumnType("smallint");

                        b.Property<string>("UserName")
                            .HasColumnType("longchar")
                            .HasMaxLength(255);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedEmail")
                            .HasName("EmailIndex");

                        b.HasIndex("NormalizedUserName")
                            .IsUnique()
                            .HasName("UserNameIndex")
                            .HasFilter("`NormalizedUserName` IS NOT NULL");

                        b.ToTable("AspNetUsers");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasColumnType("int")
                            .HasAnnotation("Jet:ValueGenerationStrategy", JetValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType")
                            .HasColumnType("longchar");

                        b.Property<string>("ClaimValue")
                            .HasColumnType("longchar");

                        b.Property<string>("UserId")
                            .IsRequired()
                            .HasColumnType("varchar(255)");

                        b.HasKey("Id");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .HasColumnType("varchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderKey")
                            .HasColumnType("varchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderDisplayName")
                            .HasColumnType("longchar");

                        b.Property<string>("UserId")
                            .IsRequired()
                            .HasColumnType("varchar(255)");

                        b.HasKey("LoginProvider", "ProviderKey");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserLogins");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.Property<string>("UserId")
                            .HasColumnType("varchar(255)");

                        b.Property<string>("RoleId")
                            .HasColumnType("varchar(255)");

                        b.HasKey("UserId", "RoleId");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetUserRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.Property<string>("UserId")
                            .HasColumnType("varchar(255)");

                        b.Property<string>("LoginProvider")
                            .HasColumnType("varchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("Name")
                            .HasColumnType("varchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("Value")
                            .HasColumnType("longchar");

                        b.HasKey("UserId", "LoginProvider", "Name");

                        b.ToTable("AspNetUserTokens");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();

                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });
#pragma warning restore 612, 618
            }
        }


        public class MigrationsInfrastructureJetFixture : MigrationsInfrastructureFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => JetTestStoreFactory.Instance;

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();
                await ((JetTestStore)TestStore).ExecuteNonQueryAsync(
                    @"DROP DATABASE TransactionSuppressed");
            }

            public override MigrationsContext CreateContext()
            {
                var options = AddOptions(TestStore.AddProviderOptions(new DbContextOptionsBuilder()))
                    .UseJet(TestStore.ConnectionString, b => b.ApplyConfiguration())
                    .UseInternalServiceProvider(ServiceProvider)
                    .Options;
                return new MigrationsContext(options);
            }
        }
    }
}

namespace ModelSnapshot22
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime EditDate { get; set; }

        public Blog Blog { get; set; }
    }

    public class BloggingContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseJet(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0", TestEnvironment.DataAccessProviderFactory);

        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasData(
                new Blog { Id = 1, Name = "HalfADonkey" });
        }
    }
}

namespace Identity30.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseJet(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0", TestEnvironment.DataAccessProviderFactory);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>(
                b =>
                {
                    b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                    b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
                    b.ToTable("AspNetUsers");
                });

            builder.Entity<IdentityUserClaim<string>>(
                b =>
                {
                    b.ToTable("AspNetUserClaims");
                });

            builder.Entity<IdentityUserLogin<string>>(
                b =>
                {
                    b.ToTable("AspNetUserLogins");
                });

            builder.Entity<IdentityUserToken<string>>(
                b =>
                {
                    b.ToTable("AspNetUserTokens");
                });

            builder.Entity<IdentityRole>(
                b =>
                {
                    b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
                    b.ToTable("AspNetRoles");
                });

            builder.Entity<IdentityRoleClaim<string>>(
                b =>
                {
                    b.ToTable("AspNetRoleClaims");
                });

            builder.Entity<IdentityUserRole<string>>(
                b =>
                {
                    b.ToTable("AspNetUserRoles");
                });
        }
    }
}
