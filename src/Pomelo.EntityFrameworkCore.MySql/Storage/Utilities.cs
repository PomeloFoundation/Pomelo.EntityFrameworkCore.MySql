using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage
{
	public static class Utilities
	{
		#if NET451
		const long UnixEpochTicks = 621355968000000000;
		const long UnixEpochMilliseconds = 62135596800000;
		const long MinTicks = 0;
		const long MaxTicks = 3155378975999999999;

		public static long ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset) {
			// Truncate sub-millisecond precision before offsetting by the Unix Epoch to avoid
			// the last digit being off by one for dates that result in negative Unix times
			long milliseconds = dateTimeOffset.UtcDateTime.Ticks / TimeSpan.TicksPerMillisecond;
			return milliseconds - UnixEpochMilliseconds;
		}

		public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds) {
			const long minMilliseconds = MinTicks / TimeSpan.TicksPerMillisecond - UnixEpochMilliseconds;
			const long maxMilliseconds = MaxTicks / TimeSpan.TicksPerMillisecond - UnixEpochMilliseconds;

			if (milliseconds < minMilliseconds || milliseconds > maxMilliseconds) {
				throw new ArgumentOutOfRangeException(nameof(milliseconds));
			}

			long ticks = milliseconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks;
			return new DateTimeOffset(ticks, TimeSpan.Zero);
		}
		#endif
	}
}
