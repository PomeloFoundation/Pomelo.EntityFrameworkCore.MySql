using System.Text;
using JetBrains.Annotations;

namespace Pomelo.EntityFrameworkCore.MySql.Utilities
{
    internal static class ByteArrayFormatter
    {
        private static readonly char[] _lookup = new char[16]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        public static string ToHex([NotNull] byte[] b)
        {
            if (b.Length == 0)
            {
                return "X''";
            }

            var builder = new StringBuilder("0x", 2 + (b.Length * 2));
            for (var i = 0; i < b.Length; i++)
            {
                var b1 = (byte)(b[i] >> 4);
                var b2 = (byte)(b[i] & 0xF);
                builder.Append(_lookup[b1]);
                builder.Append(_lookup[b2]);
            }
            return builder.ToString();
        }
    }
}
