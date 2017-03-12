using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Data.Attributes;

namespace Dragon.Data.Objects
{
    [Schema("information_schema")]
    [Table("columns")]
    public class InformationSchemaColumn
    {
        [Column("TABLE_CATALOG")]
        public string TableCatalog { get; set; }

        [Column("TABLE_SCHEMA")]
        public string TableSchema { get; set; }

        [Column("TABLE_NAME")]
        public string TableName { get; set; }

        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }

        [Column("ORDINAL_POSITION")]
        public int ColumnPosition { get; set; }

        [Column("DATA_TYPE")]
        public string ColumnDataType { get; set; }
        
        [Column("COLUMN_DEFAULT")]
        public string ColumnDefault { get; set; }

        [Column("IS_NULLABLE")]
        public string ColumnNullableString { get; set; }

        public bool ColumnNullable
        {
            get
            {
                return ColumnNullableString == "YES";
            }
        }

        [Column("CHARACTER_MAXIMUM_LENGTH")]
        public int? ColumnCharMaxLength { get; set; }

        [Column("CHARACTER_OCTET_LENGTH")]
        public int? ColumnOctetLength { get; set; }

        [Column("NUMERIC_PRECISION")]
        public byte? ColumnNumericPrecision { get; set; }

        [Column("NUMERIC_PRECISION_RADIX")]
        public Int16? ColumnNumericPrecisionRadix { get; set; }

        [Column("NUMERIC_SCALE")]
        public int? ColumnNumericScale { get; set; }

        [Column("DATETIME_PRECISION")]
        public Int16? ColumnDatetimePrecision { get; set; }

        [Column("CHARACTER_SET_CATALOG")]
        public string ColumnCharSetCatalog { get; set; }

        [Column("CHARACTER_SET_SCHEMA")]
        public string ColumnCharSetSchema { get; set; }

        [Column("CHARACTER_SET_NAME")]
        public string ColumnCharSetName { get; set; }

        [Column("COLLATION_CATALOG")]
        public string ColumnCollationCatalog { get; set; }

        [Column("COLLATION_SCHEMA")]
        public string ColumnCollationSchema { get; set; }

        [Column("COLLATION_NAME")]
        public string ColumnCollationName { get; set; }

        [Column("DOMAIN_CATALOG")]
        public string ColumnDomainCatalog { get; set; }

        [Column("DOMAIN_SCHEMA")]
        public string ColumnDomainSchema { get; set; }

        [Column("DOMAIN_NAME")]
        public string ColumnDomainName { get; set; }
    }
}
