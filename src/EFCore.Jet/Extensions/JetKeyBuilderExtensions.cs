using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using EntityFrameworkCore.Jet.Metadata.Internal;
using EntityFrameworkCore.Jet.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#nullable enable
// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Extension methods for <see cref="KeyBuilder" /> for Jet-specific metadata.
    /// </summary>
    public static class JetKeyBuilderExtensions
    {
        /// <summary>
        ///     Configures whether the key is clustered when targeting SQL Server.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and SQL Azure databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="keyBuilder">The builder for the key being configured.</param>
        /// <param name="clustered">A value indicating whether the key is clustered.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static KeyBuilder IsClustered(this KeyBuilder keyBuilder, bool clustered = true)
        {
            keyBuilder.Metadata.SetIsClustered(clustered);

            return keyBuilder;
        }

        /// <summary>
        ///     Configures whether the key is clustered when targeting SQL Server.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and SQL Azure databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="keyBuilder">The builder for the key being configured.</param>
        /// <param name="clustered">A value indicating whether the key is clustered.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static KeyBuilder<TEntity> IsClustered<TEntity>(
            this KeyBuilder<TEntity> keyBuilder,
            bool clustered = true)
            => (KeyBuilder<TEntity>)IsClustered((KeyBuilder)keyBuilder, clustered);

        /// <summary>
        ///     Configures whether the key is clustered when targeting SQL Server.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and SQL Azure databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="keyBuilder">The builder for the key being configured.</param>
        /// <param name="clustered">A value indicating whether the key is clustered.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionKeyBuilder? IsClustered(
            this IConventionKeyBuilder keyBuilder,
            bool? clustered,
            bool fromDataAnnotation = false)
        {
            if (keyBuilder.CanSetIsClustered(clustered, fromDataAnnotation))
            {
                keyBuilder.Metadata.SetIsClustered(clustered, fromDataAnnotation);
                return keyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the key can be configured as clustered.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and SQL Azure databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="keyBuilder">The builder for the key being configured.</param>
        /// <param name="clustered">A value indicating whether the key is clustered.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the key can be configured as clustered.</returns>
        public static bool CanSetIsClustered(
            this IConventionKeyBuilder keyBuilder,
            bool? clustered,
            bool fromDataAnnotation = false)
            => keyBuilder.CanSetAnnotation(JetAnnotationNames.Clustered, clustered, fromDataAnnotation);
    }
}