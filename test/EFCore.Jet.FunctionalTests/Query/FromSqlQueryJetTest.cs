// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using System.Linq;
using EntityFrameworkCore.Jet.Data;
using System.Data.OleDb;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EntityFrameworkCore.Jet.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.Jet.FunctionalTests.Query
{
    public class FromSqlQueryJetTest : FromSqlQueryTestBase<NorthwindQueryJetFixture<NoopModelCustomizer>>
    {
        public FromSqlQueryJetTest(NorthwindQueryJetFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }


        public override async Task FromSqlRaw_queryable_simple(bool async)
        {
            await base.FromSqlRaw_queryable_simple(async);

            AssertSql(
                $@"SELECT * FROM ""Customers"" WHERE ""ContactName"" LIKE '%z%'");
        }

        public override async Task FromSqlRaw_queryable_simple_columns_out_of_order(bool async)
        {
            await base.FromSqlRaw_queryable_simple_columns_out_of_order(async);

            AssertSql(
                $@"SELECT ""Region"", ""PostalCode"", ""Phone"", ""Fax"", ""CustomerID"", ""Country"", ""ContactTitle"", ""ContactName"", ""CompanyName"", ""City"", ""Address"" FROM ""Customers""");
        }

        public override async Task FromSqlRaw_queryable_simple_columns_out_of_order_and_extra_columns(bool async)
        {
            await base.FromSqlRaw_queryable_simple_columns_out_of_order_and_extra_columns(async);

            AssertSql(
                $@"SELECT ""Region"", ""PostalCode"", ""PostalCode"" AS ""Foo"", ""Phone"", ""Fax"", ""CustomerID"", ""Country"", ""ContactTitle"", ""ContactName"", ""CompanyName"", ""City"", ""Address"" FROM ""Customers""");
        }

        public override async Task<string> FromSqlRaw_queryable_composed(bool async)
        {
            var queryString = await base.FromSqlRaw_queryable_composed(async);

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers""
) AS `c`
WHERE CHARINDEX('z', `c`.`ContactName`) > 0");

            return queryString;
        }

        public override async Task FromSqlRaw_queryable_composed_after_removing_whitespaces(bool async)
        {
            await base.FromSqlRaw_queryable_composed_after_removing_whitespaces(async);

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (

        


    SELECT
    * FROM ""Customers""
) AS `c`
WHERE CHARINDEX('z', `c`.`ContactName`) > 0");
        }

        public override async Task FromSqlRaw_queryable_composed_compiled(bool async)
        {
            await base.FromSqlRaw_queryable_composed_compiled(async);

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers""
) AS `c`
WHERE CHARINDEX('z', `c`.`ContactName`) > 0");
        }

        public override async Task FromSqlRaw_queryable_composed_compiled_with_DbParameter(bool async)
        {
            await base.FromSqlRaw_queryable_composed_compiled_with_DbParameter(async);

            AssertSql(
                $@"customer='CONSH' (Nullable = false) (Size = 5)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers"" WHERE ""CustomerID"" = {AssertSqlHelper.Parameter("@customer")}
) AS `c`
WHERE CHARINDEX('z', `c`.`ContactName`) > 0");
        }

        public override async Task FromSqlRaw_queryable_composed_compiled_with_nameless_DbParameter(bool async)
        {
            await base.FromSqlRaw_queryable_composed_compiled_with_nameless_DbParameter(async);

            AssertSql(
                $@"p0='CONSH' (Nullable = false) (Size = 5)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers"" WHERE ""CustomerID"" = {AssertSqlHelper.Parameter("@p0")}
) AS `c`
WHERE CHARINDEX('z', `c`.`ContactName`) > 0");
        }

        public override async Task FromSqlRaw_queryable_composed_compiled_with_parameter(bool async)
        {
            await base.FromSqlRaw_queryable_composed_compiled_with_parameter(async);

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers"" WHERE ""CustomerID"" = 'CONSH'
) AS `c`
WHERE CHARINDEX('z', `c`.`ContactName`) > 0");
        }

        public override void FromSqlRaw_composed_contains()
        {
            base.FromSqlRaw_composed_contains();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN (
    SELECT `o`.`CustomerID`
    FROM (
        SELECT * FROM ""Orders""
    ) AS `o`
)");
        }

        public override void FromSqlRaw_composed_contains2()
        {
            base.FromSqlRaw_composed_contains2();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` = 'ALFKI') AND `c`.`CustomerID` IN (
    SELECT `o`.`CustomerID`
    FROM (
        SELECT * FROM ""Orders""
    ) AS `o`
)");
        }

        public override void FromSqlRaw_queryable_multiple_composed()
        {
            base.FromSqlRaw_queryable_multiple_composed();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers""
) AS `c`,
(
    SELECT * FROM ""Orders""
) AS `o`
WHERE `c`.`CustomerID` = `o`.`CustomerID`");
        }

        public override void FromSqlRaw_queryable_multiple_composed_with_closure_parameters()
        {
            base.FromSqlRaw_queryable_multiple_composed_with_closure_parameters();

            AssertSql(
                $@"p0='1997-01-01T00:00:00'
p1='1998-01-01T00:00:00'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers""
) AS `c`,
(
    SELECT * FROM ""Orders"" WHERE ""OrderDate"" BETWEEN {AssertSqlHelper.Parameter("@p0")} AND {AssertSqlHelper.Parameter("@p1")}
) AS `o`
WHERE `c`.`CustomerID` = `o`.`CustomerID`");
        }

        public override void FromSqlRaw_queryable_multiple_composed_with_parameters_and_closure_parameters()
        {
            base.FromSqlRaw_queryable_multiple_composed_with_parameters_and_closure_parameters();

            AssertSql(
                $@"p0='London' (Size = 4000)
p1='1997-01-01T00:00:00'
p2='1998-01-01T00:00:00'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")}
) AS `c`,
(
    SELECT * FROM ""Orders"" WHERE ""OrderDate"" BETWEEN {AssertSqlHelper.Parameter("@p1")} AND {AssertSqlHelper.Parameter("@p2")}
) AS `o`
WHERE `c`.`CustomerID` = `o`.`CustomerID`",
                //
                $@"p0='Berlin' (Size = 4000)
p1='1998-04-01T00:00:00'
p2='1998-05-01T00:00:00'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")}
) AS `c`,
(
    SELECT * FROM ""Orders"" WHERE ""OrderDate"" BETWEEN {AssertSqlHelper.Parameter("@p1")} AND {AssertSqlHelper.Parameter("@p2")}
) AS `o`
WHERE `c`.`CustomerID` = `o`.`CustomerID`");
        }

        public override void FromSqlRaw_queryable_multiple_line_query()
        {
            base.FromSqlRaw_queryable_multiple_line_query();

            AssertSql(
                $@"SELECT *
FROM ""Customers""
WHERE ""City"" = 'London'");
        }

        public override void FromSqlRaw_queryable_composed_multiple_line_query()
        {
            base.FromSqlRaw_queryable_composed_multiple_line_query();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT *
    FROM ""Customers""
) AS `c`
WHERE `c`.`City` = 'London'");
        }

        public override void FromSqlRaw_queryable_with_parameters()
        {
            base.FromSqlRaw_queryable_with_parameters();

            AssertSql(
                $@"p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}");
        }

        public override void FromSqlRaw_queryable_with_parameters_inline()
        {
            base.FromSqlRaw_queryable_with_parameters_inline();

            AssertSql(
                $@"p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}");
        }

        public override void FromSqlInterpolated_queryable_with_parameters_interpolated()
        {
            base.FromSqlInterpolated_queryable_with_parameters_interpolated();

            AssertSql(
                $@"p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}");
        }

        public override void FromSqlInterpolated_queryable_with_parameters_inline_interpolated()
        {
            base.FromSqlInterpolated_queryable_with_parameters_inline_interpolated();

            AssertSql(
                $@"p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}");
        }

        public override void FromSqlInterpolated_queryable_multiple_composed_with_parameters_and_closure_parameters_interpolated()
        {
            base.FromSqlInterpolated_queryable_multiple_composed_with_parameters_and_closure_parameters_interpolated();

            AssertSql(
                $@"p0='London' (Size = 4000)
p1='1997-01-01T00:00:00'
p2='1998-01-01T00:00:00'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")}
) AS `c`,
(
    SELECT * FROM ""Orders"" WHERE ""OrderDate"" BETWEEN {AssertSqlHelper.Parameter("@p1")} AND {AssertSqlHelper.Parameter("@p2")}
) AS `o`
WHERE `c`.`CustomerID` = `o`.`CustomerID`",
                //
                $@"p0='Berlin' (Size = 4000)
p1='1998-04-01T00:00:00'
p2='1998-05-01T00:00:00'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")}
) AS `c`,
(
    SELECT * FROM ""Orders"" WHERE ""OrderDate"" BETWEEN {AssertSqlHelper.Parameter("@p1")} AND {AssertSqlHelper.Parameter("@p2")}
) AS `o`
WHERE `c`.`CustomerID` = `o`.`CustomerID`");
        }

        public override void FromSqlRaw_queryable_with_null_parameter()
        {
            base.FromSqlRaw_queryable_with_null_parameter();

            AssertSql(
                $@"p0=NULL (Nullable = false)

SELECT * FROM ""Employees"" WHERE ""ReportsTo"" = {AssertSqlHelper.Parameter("@p0")} OR (""ReportsTo"" IS NULL AND {AssertSqlHelper.Parameter("@p0")} IS NULL)");
        }

        public override string FromSqlRaw_queryable_with_parameters_and_closure()
        {
            var queryString = base.FromSqlRaw_queryable_with_parameters_and_closure();

            AssertSql(
                $@"p0='London' (Size = 4000)
{AssertSqlHelper.Declaration("@__contactTitle_1='Sales Representative' (Size = 4000)")}

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")}
) AS `c`
WHERE `c`.`ContactTitle` = {AssertSqlHelper.Parameter("@__contactTitle_1")}");

            return queryString;
        }

        public override void FromSqlRaw_queryable_simple_cache_key_includes_query_string()
        {
            base.FromSqlRaw_queryable_simple_cache_key_includes_query_string();

            AssertSql(
                $@"SELECT * FROM ""Customers"" WHERE ""City"" = 'London'",
                //
                $@"SELECT * FROM ""Customers"" WHERE ""City"" = 'Seattle'");
        }

        public override void FromSqlRaw_queryable_with_parameters_cache_key_includes_parameters()
        {
            base.FromSqlRaw_queryable_with_parameters_cache_key_includes_parameters();

            AssertSql(
                $@"p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}",
                //
                $@"p0='Madrid' (Size = 4000)
p1='Accounting Manager' (Size = 4000)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}");
        }

        public override void FromSqlRaw_queryable_simple_as_no_tracking_not_composed()
        {
            base.FromSqlRaw_queryable_simple_as_no_tracking_not_composed();

            AssertSql(
                $@"SELECT * FROM ""Customers""");
        }

        public override void FromSqlRaw_queryable_simple_projection_composed()
        {
            base.FromSqlRaw_queryable_simple_projection_composed();

            // issue #16079
            //            AssertSql(
            //                $@"SELECT `p`.`ProductName`
            //FROM (
            //    SELECT *
            //    FROM ""Products""
            //    WHERE ""Discontinued"" <> True
            //    AND ((""UnitsInStock"" + ""UnitsOnOrder"") < ""ReorderLevel"")
            //) AS `p`");
        }

        public override void FromSqlRaw_queryable_simple_include()
        {
            base.FromSqlRaw_queryable_simple_include();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers""
) AS `c`
LEFT JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
ORDER BY `c`.`CustomerID`, `o`.`OrderID`");
        }

        public override void FromSqlRaw_queryable_simple_composed_include()
        {
            base.FromSqlRaw_queryable_simple_composed_include();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers""
) AS `c`
LEFT JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
WHERE `c`.`City` = 'London'
ORDER BY `c`.`CustomerID`, `o`.`OrderID`");
        }

        public override void FromSqlRaw_annotations_do_not_affect_successive_calls()
        {
            base.FromSqlRaw_annotations_do_not_affect_successive_calls();

            AssertSql(
                $@"SELECT * FROM ""Customers"" WHERE ""ContactName"" LIKE '%z%'",
                //
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override void FromSqlRaw_composed_with_nullable_predicate()
        {
            base.FromSqlRaw_composed_with_nullable_predicate();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers""
) AS `c`
WHERE (`c`.`ContactName` = `c`.`CompanyName`) OR (`c`.`ContactName` IS NULL AND `c`.`CompanyName` IS NULL)");
            ;
        }

        public override void FromSqlRaw_with_dbParameter()
        {
            base.FromSqlRaw_with_dbParameter();

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@city='London' (Nullable = false) (Size = 6)")}

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")}");
        }

        public override void FromSqlRaw_with_dbParameter_without_name_prefix()
        {
            base.FromSqlRaw_with_dbParameter_without_name_prefix();
            AssertSql(
                $@"city='London' (Nullable = false) (Size = 6)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")}");
        }

        public override void FromSqlRaw_with_dbParameter_mixed()
        {
            base.FromSqlRaw_with_dbParameter_mixed();

            AssertSql(
                $@"p0='London' (Size = 4000)
{AssertSqlHelper.Declaration("@title='Sales Representative' (Nullable = false) (Size = 20)")}

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@title")}",
                //
                $@"{AssertSqlHelper.Declaration("@city='London' (Nullable = false) (Size = 6)")}

p1='Sales Representative' (Size = 4000)

SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}");
        }

        public override void FromSqlRaw_with_db_parameters_called_multiple_times()
        {
            base.FromSqlRaw_with_db_parameters_called_multiple_times();

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@id='ALFKI' (Nullable = false) (Size = 5)")}

SELECT * FROM ""Customers"" WHERE ""CustomerID"" = {AssertSqlHelper.Parameter("@id")}",
                //
                $@"{AssertSqlHelper.Declaration("@id='ALFKI' (Nullable = false) (Size = 5)")}

SELECT * FROM ""Customers"" WHERE ""CustomerID"" = {AssertSqlHelper.Parameter("@id")}");
        }

        public override void FromSqlRaw_with_SelectMany_and_include()
        {
            base.FromSqlRaw_with_SelectMany_and_include();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Customers"" WHERE ""CustomerID"" = 'ALFKI'
) AS `c`,
(
    SELECT * FROM ""Customers"" WHERE ""CustomerID"" = 'AROUT'
) AS `c0`
LEFT JOIN `Orders` AS `o` ON `c0`.`CustomerID` = `o`.`CustomerID`
ORDER BY `c`.`CustomerID`, `c0`.`CustomerID`, `o`.`OrderID`");
        }

        public override void FromSqlRaw_with_join_and_include()
        {
            base.FromSqlRaw_with_join_and_include();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`, `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
FROM (
    SELECT * FROM ""Customers"" WHERE ""CustomerID"" = 'ALFKI'
) AS `c`
INNER JOIN (
    SELECT * FROM ""Orders"" WHERE ""OrderID"" <> 1
) AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
LEFT JOIN `Order Details` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
ORDER BY `c`.`CustomerID`, `o`.`OrderID`, `o0`.`OrderID`, `o0`.`ProductID`");
        }

        public override void FromSqlInterpolated_with_inlined_db_parameter()
        {
            base.FromSqlInterpolated_with_inlined_db_parameter();

            AssertSql(
                $@"{AssertSqlHelper.Declaration("@somename='ALFKI' (Nullable = false) (Size = 5)")}

SELECT * FROM ""Customers"" WHERE ""CustomerID"" = {AssertSqlHelper.Parameter("@somename")}");
        }

        public override void FromSqlInterpolated_with_inlined_db_parameter_without_name_prefix()
        {
            base.FromSqlInterpolated_with_inlined_db_parameter_without_name_prefix();

            AssertSql(
                $@"somename='ALFKI' (Nullable = false) (Size = 5)

SELECT * FROM ""Customers"" WHERE ""CustomerID"" = {AssertSqlHelper.Parameter("@somename")}");
        }

        [ConditionalFact]
        public virtual void FromSqlRaw_in_subquery_with_dbParameter()
        {
            using (var context = CreateContext())
            {
                var actual = context.Orders.Where(
                        o =>
                            context.Customers
                                .FromSqlRaw(
                                    $@"SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")}",
                                    // ReSharper disable once FormatStringProblem
                                    new OleDbParameter("@city", "London"))
                                .Select(c => c.CustomerID)
                                .Contains(o.CustomerID))
                    .ToArray();

                Assert.Equal(46, actual.Length);

                AssertSql(
                    $@"{AssertSqlHelper.Declaration("@city='London' (Nullable = false) (Size = 6)")}

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` IN (
    SELECT `c`.`CustomerID`
    FROM (
        SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")}
    ) AS `c`
)");
            }
        }

        [ConditionalFact]
        public virtual void FromSqlRaw_in_subquery_with_positional_dbParameter_without_name()
        {
            using (var context = CreateContext())
            {
                var actual = context.Orders.Where(
                        o =>
                            context.Customers
                                .FromSqlRaw(
                                    @"SELECT * FROM ""Customers"" WHERE ""City"" = {0}",
                                    // ReSharper disable once FormatStringProblem
                                    new OleDbParameter { Value = "London" })
                                .Select(c => c.CustomerID)
                                .Contains(o.CustomerID))
                    .ToArray();

                Assert.Equal(46, actual.Length);

                AssertSql(
                    $@"p0='London' (Nullable = false) (Size = 6)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` IN (
    SELECT `c`.`CustomerID`
    FROM (
        SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")}
    ) AS `c`
)");
            }
        }

        [ConditionalFact]
        public virtual void FromSqlRaw_in_subquery_with_positional_dbParameter_with_name()
        {
            using (var context = CreateContext())
            {
                var actual = context.Orders.Where(
                        o =>
                            context.Customers
                                .FromSqlRaw(
                                    @"SELECT * FROM ""Customers"" WHERE ""City"" = {0}",
                                    // ReSharper disable once FormatStringProblem
                                    new OleDbParameter("@city", "London"))
                                .Select(c => c.CustomerID)
                                .Contains(o.CustomerID))
                    .ToArray();

                Assert.Equal(46, actual.Length);

                AssertSql(
                    $@"{AssertSqlHelper.Declaration("@city='London' (Nullable = false) (Size = 6)")}

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` IN (
    SELECT `c`.`CustomerID`
    FROM (
        SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")}
    ) AS `c`
)");
            }
        }

        [ConditionalFact]
        public virtual void FromSqlRaw_with_dbParameter_mixed_in_subquery()
        {
            using (var context = CreateContext())
            {
                const string city = "London";
                const string title = "Sales Representative";

                var actual = context.Orders.Where(
                        o =>
                            context.Customers
                                .FromSqlRaw(
                                    $@"SELECT * FROM ""Customers"" WHERE ""City"" = {0} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@title")}",
                                    city,
                                    // ReSharper disable once FormatStringProblem
                                    new OleDbParameter("@title", title))
                                .Select(c => c.CustomerID)
                                .Contains(o.CustomerID))
                    .ToArray();

                Assert.Equal(26, actual.Length);

                actual = context.Orders.Where(
                        o =>
                            context.Customers
                                .FromSqlRaw(
                                    $@"SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")} AND ""ContactTitle"" = {1}",
                                    // ReSharper disable once FormatStringProblem
                                    new OleDbParameter("@city", city),
                                    title)
                                .Select(c => c.CustomerID)
                                .Contains(o.CustomerID))
                    .ToArray();

                Assert.Equal(26, actual.Length);

                AssertSql(
                    $@"p0='London' (Size = 4000)
{AssertSqlHelper.Declaration("@title='Sales Representative' (Nullable = false) (Size = 20)")}

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` IN (
    SELECT `c`.`CustomerID`
    FROM (
        SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@p0")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@title")}
    ) AS `c`
)",
                    //
                    $@"{AssertSqlHelper.Declaration("@city='London' (Nullable = false) (Size = 6)")}

p1='Sales Representative' (Size = 4000)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` IN (
    SELECT `c`.`CustomerID`
    FROM (
        SELECT * FROM ""Customers"" WHERE ""City"" = {AssertSqlHelper.Parameter("@city")} AND ""ContactTitle"" = {AssertSqlHelper.Parameter("@p1")}
    ) AS `c`
)");
            }
        }

        public override void FromSqlInterpolated_parameterization_issue_12213()
        {
            base.FromSqlInterpolated_parameterization_issue_12213();

            AssertSql(
                $@"p0='10300'

SELECT * FROM ""Orders"" WHERE ""OrderID"" >= {AssertSqlHelper.Parameter("@p0")}",
                //
                $@"{AssertSqlHelper.Declaration("@__max_0='10400'")}

p0='10300'

SELECT `o`.`OrderID`
FROM `Orders` AS `o`
WHERE (`o`.`OrderID` <= {AssertSqlHelper.Parameter("@__max_0")}) AND `o`.`OrderID` IN (
    SELECT `o0`.`OrderID`
    FROM (
        SELECT * FROM ""Orders"" WHERE ""OrderID"" >= {AssertSqlHelper.Parameter("@p0")}
    ) AS `o0`
)",
                //
                $@"{AssertSqlHelper.Declaration("@__max_0='10400'")}

p0='10300'

SELECT `o`.`OrderID`
FROM `Orders` AS `o`
WHERE (`o`.`OrderID` <= {AssertSqlHelper.Parameter("@__max_0")}) AND `o`.`OrderID` IN (
    SELECT `o0`.`OrderID`
    FROM (
        SELECT * FROM ""Orders"" WHERE ""OrderID"" >= {AssertSqlHelper.Parameter("@p0")}
    ) AS `o0`
)");
        }

        public override void FromSqlRaw_does_not_parameterize_interpolated_string()
        {
            base.FromSqlRaw_does_not_parameterize_interpolated_string();

            AssertSql(
                $@"p0='10250'

SELECT * FROM ""Orders"" WHERE ""OrderID"" < {AssertSqlHelper.Parameter("@p0")}");
        }

        public override void Entity_equality_through_fromsql()
        {
            base.Entity_equality_through_fromsql();

            AssertSql(
                $@"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT * FROM ""Orders""
) AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`CustomerID` = 'VINET'");
        }

        public override void FromSqlRaw_with_set_operation()
        {
            base.FromSqlRaw_with_set_operation();

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM (
    SELECT * FROM ""Customers"" WHERE ""City"" = 'London'
) AS `c`
UNION ALL
SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM (
    SELECT * FROM ""Customers"" WHERE ""City"" = 'Berlin'
) AS `c0`");
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new OleDbParameter { ParameterName = name, Value = value };

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
