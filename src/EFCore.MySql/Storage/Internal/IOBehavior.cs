// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Specifies whether to perform synchronous or asynchronous I/O.
	/// </summary>
	internal enum IOBehavior
	{
		/// <summary>
		/// Use synchronous I/O.
		/// </summary>
		Synchronous,

		/// <summary>
		/// Use asynchronous I/O.
		/// </summary>
		Asynchronous,
	}
}
