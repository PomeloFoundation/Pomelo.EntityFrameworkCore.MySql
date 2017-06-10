// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using JetBrains.Annotations;
// using Microsoft.EntityFrameworkCore.Utilities;
// using MySql.Data.MySqlClient;


// namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
// {
//     public class MySqlColumnModelAnnotations
//     {
//         private readonly ColumnModel _column;

//         public MySqlColumnModelAnnotations(/* [NotNull] */ ColumnModel column)
//         {
//             // Check.NotNull(column, nameof(column));

//             _column = column;
//         }
        
//         public bool IsSerial
//         {
//             get
//             {
//                 var value = _column[MySqlDatabaseModelAnnotationNames.IsSerial];
//                 return value is bool && (bool)value;
//             }
//             //[param: CanBeNull]
//             set { _column[MySqlDatabaseModelAnnotationNames.IsSerial] = value; }
//         }

//         internal string ElementDataType
//         {
//             get
//             {
//                 return (string)_column[MySqlDatabaseModelAnnotationNames.ElementDataType];
//             }
//             //[param: CanBeNull]
//             set { _column[MySqlDatabaseModelAnnotationNames.ElementDataType] = value; }
//         }
//     }
// }
