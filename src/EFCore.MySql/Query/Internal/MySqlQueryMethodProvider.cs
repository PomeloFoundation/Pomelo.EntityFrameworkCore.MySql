using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query
{
	public class MySqlQueryMethodProvider : QueryMethodProvider
	{

		private static readonly MethodInfo _baseShapedQuery =
			typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_ShapedQuery");

		private static readonly MethodInfo _baseDefaultIfEmptyShapedQuery =
			typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_DefaultIfEmptyShapedQuery");

		private static readonly MethodInfo _baseQuery =
			typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_Query");

		private static readonly MethodInfo _baseInclude =
			typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_Include");


		public override MethodInfo ShapedQueryMethod => _shapedQueryMethodInfo;

		private static readonly MethodInfo _shapedQueryMethodInfo
			= typeof(MySqlQueryMethodProvider).GetTypeInfo()
				.GetDeclaredMethod(nameof(_ShapedQuery));

		private static IEnumerable<T> _ShapedQuery<T>(
			QueryContext queryContext,
			ShaperCommandContext shaperCommandContext,
			IShaper<T> shaper)
		{
			return new MySqlQueryingEnumerable<T>(queryContext as MySqlQueryContext,
				(IEnumerable<T>) _baseShapedQuery.MakeGenericMethod(typeof(T))
					.Invoke(null, new object[] {queryContext, shaperCommandContext, shaper}));
		}


		public override MethodInfo DefaultIfEmptyShapedQueryMethod => _defaultIfEmptyShapedQueryMethodInfo;

		private static readonly MethodInfo _defaultIfEmptyShapedQueryMethodInfo
			= typeof(MySqlQueryMethodProvider).GetTypeInfo()
				.GetDeclaredMethod(nameof(_DefaultIfEmptyShapedQuery));

		[UsedImplicitly]
		private static IEnumerable<T> _DefaultIfEmptyShapedQuery<T>(
			QueryContext queryContext,
			ShaperCommandContext shaperCommandContext,
			IShaper<T> shaper)
		{
			return new MySqlQueryingEnumerable<T>(queryContext as MySqlQueryContext,
				(IEnumerable<T>) _baseDefaultIfEmptyShapedQuery.MakeGenericMethod(typeof(T))
					.Invoke(null, new object[] {queryContext, shaperCommandContext, shaper}));
		}


		public override MethodInfo QueryMethod => _queryMethodInfo;

		private static readonly MethodInfo _queryMethodInfo
			= typeof(MySqlQueryMethodProvider).GetTypeInfo()
				.GetDeclaredMethod(nameof(_Query));

		private static IEnumerable<ValueBuffer> _Query(
			QueryContext queryContext,
			ShaperCommandContext shaperCommandContext,
			int? queryIndex)
		{
			return new MySqlQueryingEnumerable<ValueBuffer>(queryContext as MySqlQueryContext,
				(IEnumerable<ValueBuffer>)
				_baseQuery.Invoke(null, new object[] {queryContext, shaperCommandContext, queryIndex}));
		}

		private static IEnumerable<T> _Include<T>(
			RelationalQueryContext queryContext,
			IEnumerable<T> innerResults,
			Func<T, object> entityAccessor,
			IReadOnlyList<INavigation> navigationPath,
			IReadOnlyList<Func<QueryContext, IRelatedEntitiesLoader>> relatedEntitiesLoaderFactories,
			bool querySourceRequiresTracking)
		{
			// ReSharper disable once PossibleNullReferenceException
			(queryContext as MySqlQueryContext).HasInclude = true;
			return (IEnumerable<T>) _baseInclude.MakeGenericMethod(typeof(T))
				.Invoke(null, new object[] {queryContext, innerResults, entityAccessor, navigationPath, relatedEntitiesLoaderFactories, querySourceRequiresTracking});
		}

	}
}
