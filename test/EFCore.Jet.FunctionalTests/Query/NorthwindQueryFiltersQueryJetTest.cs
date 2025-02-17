﻿// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Jet.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming
namespace EntityFrameworkCore.Jet.FunctionalTests.Query
{
    public class NorthwindQueryFiltersQueryJetTest : NorthwindQueryFiltersQueryTestBase<NorthwindQueryJetFixture<NorthwindQueryFiltersCustomizer>>
    {
        public NorthwindQueryFiltersQueryJetTest(NorthwindQueryJetFixture<NorthwindQueryFiltersCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Count_query(bool async)
        {
            await base.Count_query(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})");
        }

        public override async Task Materialized_query(bool async)
        {
            await base.Materialized_query(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})");
        }

        public override async Task Find(bool async)
        {
            await base.Find(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__p_0='ALFKI' (Size = 5)")}

SELECT TOP 1 `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) AND `c`.`CustomerID` = {AssertSqlHelper.Parameter("@__p_0")}");
        }

        public override async Task Materialized_query_parameter(bool async)
        {
            await base.Materialized_query_parameter(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='F' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='F' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='F' (Size = 255)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})");
        }

        public override async Task Materialized_query_parameter_new_context(bool async)
        {
            await base.Materialized_query_parameter_new_context(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})",
                //
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='T' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='T' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='T' (Size = 255)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})");
        }

        public override async Task Projection_query_parameter(bool async)
        {
            await base.Projection_query_parameter(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='F' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='F' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='F' (Size = 255)")}

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})");
        }

        public override async Task Projection_query(bool async)
        {
            await base.Projection_query(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})");
        }

        public override async Task Include_query(bool async)
        {
            await base.Include_query(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `t0`.`OrderID`, `t0`.`CustomerID`, `t0`.`EmployeeID`, `t0`.`OrderDate`, `t0`.`CustomerID0`
FROM `Customers` AS `c`
LEFT JOIN (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`, `t`.`CustomerID` AS `CustomerID0`
    FROM `Orders` AS `o`
    LEFT JOIN (
        SELECT `c0`.`CustomerID`, `c0`.`CompanyName`
        FROM `Customers` AS `c0`
        WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c0`.`CompanyName` IS NOT NULL) AND LEFT(`c0`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})
    ) AS `t` ON `o`.`CustomerID` = `t`.`CustomerID`
    WHERE (`t`.`CustomerID` IS NOT NULL) AND (`t`.`CompanyName` IS NOT NULL)
) AS `t0` ON `c`.`CustomerID` = `t0`.`CustomerID`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})
ORDER BY `c`.`CustomerID`, `t0`.`OrderID`");
        }

        public override async Task Include_query_opt_out(bool async)
        {
            await base.Include_query_opt_out(async);

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Included_many_to_one_query(bool async)
        {
            await base.Included_many_to_one_query(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`, `t`.`CustomerID`, `t`.`Address`, `t`.`City`, `t`.`CompanyName`, `t`.`ContactName`, `t`.`ContactTitle`, `t`.`Country`, `t`.`Fax`, `t`.`Phone`, `t`.`PostalCode`, `t`.`Region`
FROM `Orders` AS `o`
LEFT JOIN (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})
) AS `t` ON `o`.`CustomerID` = `t`.`CustomerID`
WHERE (`t`.`CustomerID` IS NOT NULL) AND (`t`.`CompanyName` IS NOT NULL)");
        }

        public override async Task Project_reference_that_itself_has_query_filter_with_another_reference(bool async)
        {
            await base.Project_reference_that_itself_has_query_filter_with_another_reference(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_1='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_1='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter___quantity_0='50'")}

SELECT `t0`.`OrderID`, `t0`.`CustomerID`, `t0`.`EmployeeID`, `t0`.`OrderDate`
FROM `Order Details` AS `o`
INNER JOIN (
    SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`, `t`.`CustomerID` AS `CustomerID0`, `t`.`Address`, `t`.`City`, `t`.`CompanyName`, `t`.`ContactName`, `t`.`ContactTitle`, `t`.`Country`, `t`.`Fax`, `t`.`Phone`, `t`.`PostalCode`, `t`.`Region`
    FROM `Orders` AS `o0`
    LEFT JOIN (
        SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
        FROM `Customers` AS `c`
        WHERE ({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_1")} = '') OR (`c`.`CompanyName` IS NOT NULL AND (LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_1")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_1")}))
    ) AS `t` ON `o0`.`CustomerID` = `t`.`CustomerID`
    WHERE `t`.`CustomerID` IS NOT NULL AND `t`.`CompanyName` IS NOT NULL
) AS `t0` ON `o`.`OrderID` = `t0`.`OrderID`
WHERE `o`.`Quantity` > {AssertSqlHelper.Parameter("@__ef_filter___quantity_0")}");
        }

        public override async Task Navs_query(bool async)
        {
            await base.Navs_query(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter___quantity_1='50'")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
INNER JOIN (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    LEFT JOIN (
        SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
        FROM `Customers` AS `c0`
        WHERE ({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '') OR (`c0`.`CompanyName` IS NOT NULL AND (LEFT(`c0`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")}))
    ) AS `t` ON `o`.`CustomerID` = `t`.`CustomerID`
    WHERE `t`.`CustomerID` IS NOT NULL AND `t`.`CompanyName` IS NOT NULL
) AS `t0` ON `c`.`CustomerID` = `t0`.`CustomerID`
INNER JOIN (
    SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
    FROM `Order Details` AS `o0`
    WHERE `o0`.`Quantity` > {AssertSqlHelper.Parameter("@__ef_filter___quantity_1")}
) AS `t1` ON `t0`.`OrderID` = `t1`.`OrderID`
WHERE (({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '') OR ((`c`.`CompanyName` IS NOT NULL) AND (LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")}))) AND (`t1`.`Discount` < IIF(10 IS NULL, NULL, CSNG(10)))");
        }

        [ConditionalFact]
        public void FromSql_is_composed()
        {
            using (var context = Fixture.CreateContext())
            {
                var results = context.Customers.FromSqlRaw("select * from Customers").ToList();

                Assert.Equal(7, results.Count);
            }

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `m`.`CustomerID`, `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`Fax`, `m`.`Phone`, `m`.`PostalCode`, `m`.`Region`
FROM (
    select * from Customers
) AS `m`
WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`m`.`CompanyName` IS NOT NULL) AND LEFT(`m`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})");
        }

        [ConditionalFact]
        public void FromSql_is_composed_when_filter_has_navigation()
        {
            using (var context = Fixture.CreateContext())
            {
                var results = context.Orders.FromSqlRaw("select * from Orders").ToList();

                Assert.Equal(80, results.Count);
            }

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `m`.`OrderID`, `m`.`CustomerID`, `m`.`EmployeeID`, `m`.`OrderDate`
FROM (
    select * from Orders
) AS `m`
LEFT JOIN (
    SELECT `c`.`CustomerID`, `c`.`CompanyName`
    FROM `Customers` AS `c`
    WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})
) AS `t` ON `m`.`CustomerID` = `t`.`CustomerID`
WHERE (`t`.`CustomerID` IS NOT NULL) AND (`t`.`CompanyName` IS NOT NULL)");
        }

        public override void Compiled_query()
        {
            base.Compiled_query();

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__customerID='BERGS' (Size = 5)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '') OR ((`c`.`CompanyName` IS NOT NULL) AND (LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")}))) AND (`c`.`CustomerID` = {AssertSqlHelper.Parameter("@__customerID")})",
                //
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

{AssertSqlHelper.Declaration("@__customerID='BLAUS' (Size = 5)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '') OR (`c`.`CompanyName` IS NOT NULL AND (LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")}))) AND (`c`.`CustomerID` = {AssertSqlHelper.Parameter("@__customerID")})");
        }

        public override async Task Entity_Equality(bool async)
        {
            await base.Entity_Equality(async);

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}
{AssertSqlHelper.Declaration("@__ef_filter__TenantPrefix_0='B' (Size = 255)")}

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
LEFT JOIN (
    SELECT `c`.`CustomerID`, `c`.`CompanyName`
    FROM `Customers` AS `c`
    WHERE {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")} = '' OR ((`c`.`CompanyName` IS NOT NULL) AND LEFT(`c`.`CompanyName`, LEN({AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})) = {AssertSqlHelper.Parameter("@__ef_filter__TenantPrefix_0")})
) AS `t` ON `o`.`CustomerID` = `t`.`CustomerID`
WHERE (`t`.`CustomerID` IS NOT NULL) AND (`t`.`CompanyName` IS NOT NULL)");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
