// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
namespace EntityFrameworkCore.Jet.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class JetSqlTranslatingExpressionVisitor : RelationalSqlTranslatingExpressionVisitor
{
    private static readonly HashSet<string> DateTimeDataTypes
        = new()
        {
            "time",
            "date",
            "datetime",
            "datetime2",
            "datetimeoffset"
        };

    private static readonly HashSet<Type> DateTimeClrTypes
        = new()
        {
            typeof(TimeOnly),
            typeof(DateOnly),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(DateTimeOffset)
        };

    private static readonly HashSet<ExpressionType> ArithmeticOperatorTypes
        = new()
        {
            ExpressionType.Add,
            ExpressionType.Subtract,
            ExpressionType.Multiply,
            ExpressionType.Divide,
            ExpressionType.Modulo
        };

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public JetSqlTranslatingExpressionVisitor(
        RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
        QueryCompilationContext queryCompilationContext,
        QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
        : base(dependencies, queryCompilationContext, queryableMethodTranslatingExpressionVisitor)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override Expression VisitBinary(BinaryExpression binaryExpression)
    {
        if (binaryExpression.NodeType == ExpressionType.ArrayIndex
            && binaryExpression.Left.Type == typeof(byte[]))
        {
            var left = Visit(binaryExpression.Left);
            var right = Visit(binaryExpression.Right);

            if (left is SqlExpression leftSql
                && right is SqlExpression rightSql)
            {
                var midfunc = Dependencies.SqlExpressionFactory.Function(
                    "MID",
                    new[]
                    {
                        leftSql,
                        Dependencies.SqlExpressionFactory.Add(
                            Dependencies.SqlExpressionFactory.ApplyDefaultTypeMapping(rightSql),
                            Dependencies.SqlExpressionFactory.Constant(1)),
                        Dependencies.SqlExpressionFactory.Constant(1)
                    },
                    nullable: true,
                    argumentsPropagateNullability: new[] { true, true, true },
                    typeof(byte[]));
                return Dependencies.SqlExpressionFactory.Convert(
                    Dependencies.SqlExpressionFactory.Function(
                        "ASC",
                        new[]
                        {
                            midfunc
                        },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true },
                        typeof(byte)),
                    binaryExpression.Type);
            }
        }

        var visitedExpression = base.VisitBinary(binaryExpression);

        if (visitedExpression is SqlBinaryExpression sqlBinaryExpression
            && ArithmeticOperatorTypes.Contains(sqlBinaryExpression.OperatorType))
        {
            var inferredProviderType = GetProviderType(sqlBinaryExpression.Left) ?? GetProviderType(sqlBinaryExpression.Right);
            if (inferredProviderType != null)
            {
                if (DateTimeDataTypes.Contains(inferredProviderType))
                {
                    return QueryCompilationContext.NotTranslatedExpression;
                }
            }
            else
            {
                var leftType = sqlBinaryExpression.Left.Type;
                var rightType = sqlBinaryExpression.Right.Type;
                if (DateTimeClrTypes.Contains(leftType)
                    || DateTimeClrTypes.Contains(rightType))
                {
                    return QueryCompilationContext.NotTranslatedExpression;
                }
            }
        }

        return visitedExpression;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override Expression VisitUnary(UnaryExpression unaryExpression)
    {
        if (unaryExpression.NodeType == ExpressionType.ArrayLength
            && unaryExpression.Operand.Type == typeof(byte[]))
        {
            if (!(base.Visit(unaryExpression.Operand) is SqlExpression sqlExpression))
            {
                return QueryCompilationContext.NotTranslatedExpression;
            }

            var isBinaryMaxDataType = GetProviderType(sqlExpression) == "varbinary(max)" || sqlExpression is SqlParameterExpression;
            var dataLengthSqlFunction = Dependencies.SqlExpressionFactory.Function(
                "DATALENGTH",
                new[] { sqlExpression },
                nullable: true,
                argumentsPropagateNullability: new[] { true },
                isBinaryMaxDataType ? typeof(long) : typeof(int));

            return isBinaryMaxDataType
                ? Dependencies.SqlExpressionFactory.Convert(dataLengthSqlFunction, typeof(int))
                : dataLengthSqlFunction;
        }

        return base.VisitUnary(unaryExpression);
    }

    private static string? GetProviderType(SqlExpression expression)
        => expression.TypeMapping?.StoreType;
}
