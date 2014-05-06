using System;
using Dragon.Common.Attributes.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Dragon.SQL;
using System.Linq;

namespace Dragon.Tests.SQL
{
    [TestClass]
    public class PKMetadataAndSqlBuilderTest
    {
        [TestMethod]
        public void BuildCreate_OneIntKey()
        {
            var md = new TableMetadata();
            MetadataHelper.MetadataForClass(typeof(TableWithOneIntKey),ref md);

            var sql = TSQLGenerator.BuildCreate(md, true);

            Assert.AreEqual(@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TableWithOneIntKey]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TableWithOneIntKey](
   [Key1] [int] IDENTITY(1,1) NOT NULL,
   [Other] [NVARCHAR](50) NULL,
CONSTRAINT [PK_TableWithOneIntKey] PRIMARY KEY CLUSTERED ( [Key1] ASC )
);
END
", sql);
        }

        [TestMethod]
        public void BuildCreate_TwoIntKeys()
        {
            var md = new TableMetadata();
            MetadataHelper.MetadataForClass(typeof(TableWithTwoIntKeys), ref md);

            var sql = TSQLGenerator.BuildCreate(md, true);

            Assert.AreEqual(@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TableWithTwoIntKeys]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TableWithTwoIntKeys](
   [Key1] [INT] NOT NULL,
   [Key2] [INT] NOT NULL,
   [Other] [NVARCHAR](50) NULL,
CONSTRAINT [PK_TableWithTwoIntKeys] PRIMARY KEY CLUSTERED ( [Key1] ASC,[Key2] ASC )
);
END
", sql);
        }

        public class TableWithOneIntKey
        {
            [Key]
            public int Key1 { get; set; }
            public string Other { get; set; }
        }

        public class TableWithTwoIntKeys
        {
            [Key]
            public int Key1 { get; set; }
            [Key]
            public int Key2 { get; set; }
            
            public string Other { get; set; }
        }
    }
}

