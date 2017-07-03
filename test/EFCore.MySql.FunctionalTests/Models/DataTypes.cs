using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models
{
	public class DataTypesSimple
	{
		public int Id { get; set; }

		// bool
		public bool    TypeBool { get; set; }
		// nullable bool
		public bool?   TypeBoolN { get; set; }

		// integers
		public short   TypeShort { get; set; }
		public ushort  TypeUshort { get; set; }
		public int     TypeInt { get; set; }
		public uint    TypeUint { get; set; }
		public long    TypeLong { get; set; }
		public ulong   TypeUlong { get; set; }
		// nullable integers
		public short?  TypeShortN { get; set; }
		public ushort? TypeUshortN { get; set; }
		public int?    TypeIntN { get; set; }
		public uint?   TypeUintN { get; set; }
		public long?   TypeLongN { get; set; }
		public ulong?  TypeUlongN { get; set; }

		// decimals
		public decimal  TypeDecimal { get; set; }
		public double   TypeDouble { get; set; }
		public float    TypeFloat { get; set; }
		// nullable decimals
		public decimal? TypeDecimalN { get; set; }
		public double?  TypeDoubleN { get; set; }
		public float?   TypeFloatN { get; set; }

		// byte
		public sbyte    TypeSbyte  { get; set; }
		public byte     TypeByte   { get; set; }
		public char     TypeChar   { get; set; }
		// nullable byte
		public sbyte?   TypeSbyteN { get; set; }
		public byte?    TypeByteN  { get; set; }
		public char?    TypeCharN  { get; set; }

		// DateTime
		public DateTime        TypeDateTime { get; set; }
		public DateTimeOffset  TypeDateTimeOffset  { get; set; }
		public TimeSpan        TypeTimeSpan { get; set; }
		// nullable DateTime
		public DateTime?       TypeDateTimeN { get; set; }
		public DateTimeOffset? TypeDateTimeOffsetN  { get; set; }
		public TimeSpan?       TypeTimeSpanN { get; set; }

		// Enum
		public TestEnum TypeEnum           { get; set; }
		public TestEnumByte TypeEnumByte   { get; set; }
		// nullable Enum
		public TestEnum? TypeEnumN         { get; set; }
		public TestEnumByte? TypeEnumByteN { get; set; }

		// guid
		public Guid  TypeGuid { get; set; }
		// nullable guid
		public Guid? TypeGuidN { get; set; }
	}

	public class DataTypesVariable
	{
		public int Id { get; set; }

		// string not null
		[Required]
		public string TypeString     { get; set; }

		[Required]
		[MaxLength(255)]
		public string TypeString255  { get; set; }

		// string null
		public string TypeStringN    { get; set; }

		[MaxLength(255)]
		public string TypeString255N { get; set; }


		// binary not null
		[Required]
		[MaxLength(255)]
		public byte[] TypeByteArray255  { get; set; }

		[Required]
		public byte[] TypeByteArray     { get; set; }

		// binary null
		[MaxLength(255)]
		public byte[] TypeByteArray255N { get; set; }

		public byte[] TypeByteArrayN    { get; set; }


		// json not null
		[Required]
		public JsonObject<List<string>>               TypeJsonArray   { get; set; }

		[Required]
		public JsonObject<Dictionary<string, string>> TypeJsonObject  { get; set; }

		// json null
		public JsonObject<List<string>>               TypeJsonArrayN  { get; set; }

		public JsonObject<Dictionary<string, string>> TypeJsonObjectN { get; set; }


	    // static method to create a new empty object
	    public static readonly byte[] EmptyByteArray = new byte[0];
	    public static readonly JsonObject<List<string>> EmptyJsonArray = new JsonObject<List<string>>(new List<string>());
	    public static readonly JsonObject<Dictionary<string, string>> EmptyJsonObject = new JsonObject<Dictionary<string, string>>(new Dictionary<string, string>());

	    public static DataTypesVariable CreateEmpty()
	    {
	        return new DataTypesVariable
	        {
	            TypeString = "",
	            TypeString255 = "",
	            TypeByteArray    = EmptyByteArray,
	            TypeByteArray255 = EmptyByteArray,
	            TypeJsonArray    = EmptyJsonArray,
	            TypeJsonObject   = EmptyJsonObject,
	        };
	    }
	}

	public enum TestEnum{
		TestDefault=0,
		TestOne=1
	}

	public enum TestEnumByte : byte{
		TestDefault=0,
		TestOne=1
	}

}
