// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class CharSetInfo
    {
        public readonly CharSet CharSet;
        public readonly int BytesPerChar;
        public readonly string CharSetName;

        public CharSetInfo(CharSet charSet)
        {
            CharSet = charSet;
            CharSetName = charSet.ToString().ToLower();
            switch (CharSet)
            {
                case CharSet.Latin1:
                    BytesPerChar = 1;
                    break;
                case CharSet.Ucs2:
                    BytesPerChar = 2;
                    break;
                case CharSet.Utf8mb3:
                    BytesPerChar = 3;
                    break;
                case CharSet.Utf8mb4:
                    BytesPerChar = 4;
                    break;
                default:
                    throw new InvalidOperationException($"No BytesPerChar defined for CharSet '{CharSetName}'");
            }
        }

        public override int GetHashCode()
        {
            return CharSet.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CharSetInfo other)
            {
                return CharSet == other.CharSet;
            }

            return false;
        }
    }
}
