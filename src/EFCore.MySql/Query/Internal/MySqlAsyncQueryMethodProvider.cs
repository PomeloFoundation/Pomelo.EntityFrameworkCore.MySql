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
	public class MySqlAsyncQueryMethodProvider : AsyncQueryMethodProvider
	{

		private static readonly MethodInfo _baseShapedQuery =
			typeof(AsyncQueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_ShapedQuery");

		private static readonly MethodInfo _baseDefaultIfEmptyShapedQuery =
			typeof(AsyncQueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_DefaultIfEmptyShapedQuery");

		private static readonly MethodInfo _baseQuery =
			typeof(AsyncQueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_Query");

		private static readonly MethodInfo _baseInclude =
			typeof(AsyncQueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_Include");


		public override MethodInfo ShapedQueryMethod => _shapedQueryMethodInfo;

		private static readonly MethodInfo _shapedQueryMethodInfo
			= typeof(MySqlAsyncQueryMethodProvider).GetTypeInfo()
				.GetDeclaredMethod(nameof(_ShapedQuery));

		private static IAsyncEnumerable<T> _ShapedQuery<T>(
			QueryContext queryContext,
			ShaperCommandContext shaperCommandContext,
			IShaper<T> shaper)
		{
			return new MySqlAsyncQueryingEnumerable<T>(queryContext as MySqlQueryContext,
				(IAsyncEnumerable<T>) _baseShapedQuery.MakeGenericMethod(typeof(T))
					.Invoke(null, new object[] {queryContext, shaperCommandContext, shaper}));
		}


		public override MethodInfo DefaultIfEmptyShapedQueryMethod => _defaultIfEmptyShapedQueryMethodInfo;

		private static readonly MethodInfo _defaultIfEmptyShapedQueryMethodInfo
			= typeof(MySqlAsyncQueryMethodProvider).GetTypeInfo()
				.GetDeclaredMethod(nameof(_DefaultIfEmptyShapedQuery));

		[UsedImplicitly]
		private static IAsyncEnumerable<T> _DefaultIfEmptyShapedQuery<T>(
			QueryContext queryContext,
			ShaperCommandContext shaperCommandContext,
			IShaper<T> shaper)
		{
			return new MySqlAsyncQueryingEnumerable<T>(queryContext as MySqlQueryContext,
				(IAsyncEnumerable<T>) _baseDefaultIfEmptyShapedQuery.MakeGenericMethod(typeof(T))
				.Invoke(null, new object[] {queryContext, shaperCommandContext, shaper}));
		}


		public override MethodInfo QueryMethod => _queryMethodInfo;

		private static readonly MethodInfo _queryMethodInfo
			= typeof(MySqlAsyncQueryMethodProvider).GetTypeInfo()
				.GetDeclaredMethod(nameof(_Query));

		private static IAsyncEnumerable<ValueBuffer> _Query(
			QueryContext queryContext,
			ShaperCommandContext shaperCommandContext,
			int? queryIndex)
		{
			return new MySqlAsyncQueryingEnumerable<ValueBuffer>(queryContext as MySqlQueryContext,
				(IAsyncEnumerable<ValueBuffer>) _baseQuery.Invoke(null, new object[] {queryContext, shaperCommandContext, queryIndex}));
		}

		private static IAsyncEnumerable<T> _Include<T>(
			RelationalQueryContext queryContext,
			IAsyncEnumerable<T> innerResults,
			Func<T, object> entityAccessor,
			IReadOnlyList<INavigation> navigationPath,
			IReadOnlyList<Func<QueryContext, IAsyncRelatedEntitiesLoader>> relatedEntitiesLoaderFactories,
			bool querySourceRequiresTracking)
		{
			// ReSharper disable once PossibleNullReferenceException
			(queryContext as MySqlQueryContext).HasInclude = true;
			return (IAsyncEnumerable<T>) _baseInclude.MakeGenericMethod(typeof(T))
				.Invoke(null, new object[] {queryContext, innerResults, entityAccessor, navigationPath, relatedEntitiesLoaderFactories, querySourceRequiresTracking});
		}

	}
}
