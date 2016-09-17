using System;
using System.Collections.Generic;

namespace Pomelo.EntityFrameworkCore.MySql.Functional.Models
{
    public class DataTypes
    {
        public int Id { get; set; }

        // numbers
        public short    TypeShort     { get; set; }
        public ushort   TypeUshort    { get; set; }
        public int      TypeInt       { get; set; }
        public uint     TypeUint      { get; set; }
        public long     TypeLong      { get; set; }
        public ulong    TypeUlong     { get; set; }

        // string
        public string   TypeString    { get; set; }
        
        // binary
        public byte     TypeByte      { get; set; }
        public byte[]   TypeByteArray { get; set; }

        // json
        public JsonObject<List<string>>               TypeJsonObj    { get; set; }
        public JsonObject<Dictionary<string, string>> TypeJsonArray  { get; set; }
    }
}
