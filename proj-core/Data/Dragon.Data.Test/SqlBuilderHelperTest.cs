using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Dragon.Data;
using System.Linq;

namespace Dragon.Data.Test
{
    [TestClass]
    public class SqlBuilderHelperTest
    {
        [TestMethod]
        public void FindUniqueNameInDictionary()
        {
            var dict = new Dictionary<string, object>();
            var subject = "test";

            var s1 = TSQLGenerator.FindUniqueNameInDictionary(subject, dict);
            Assert.IsTrue(s1.Equals("test"));
            dict.Add(s1, null);
            var s2 = TSQLGenerator.FindUniqueNameInDictionary(subject, dict);
            Assert.IsTrue(s2.Equals("test2"));
            dict.Add(s2, null);
            var s3 = TSQLGenerator.FindUniqueNameInDictionary(subject, dict);
            Assert.IsTrue(s3.Equals("test3"));
        }


        [TestMethod]
        public void BuildWhereClause()
        {
            var tableMD = GetTestMetadata();

            var values = new Dictionary<string, object>();
            values.Add("aa", 1);
            values.Add("cc", "test");

            var parameters = new Dictionary<string, object>();

            var sql = TSQLGenerator.BuildWhereClause(tableMD, values, ref parameters);

            Assert.AreEqual("[a]=@aa AND [c]=@cc", sql);
            Assert.AreEqual(2, parameters.Count);
        }

        [TestMethod]
        public void BuildColumnList()
        {
            var tableMD = GetTestMetadata();

            var sql = TSQLGenerator.BuildColumnList(tableMD);

            Assert.AreEqual("[a],[b],[c]", sql);
        }

        [TestMethod]
        public void BuildSelect_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var values = new Dictionary<string, object>();
            values.Add("aa", 1);
            values.Add("cc", "test");

            var parameters = new Dictionary<string, object>();

            var sql = TSQLGenerator.BuildSelect(tableMD, values, ref parameters);

            Assert.AreEqual("SELECT [a] AS 'aa',[b] AS 'bb',[c] AS 'cc' FROM [testtable] WHERE [a]=@aa AND [c]=@cc", sql);
            Assert.AreEqual(2, parameters.Count);
        }

        [TestMethod]
        public void BuildSelect_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var values = new Dictionary<string, object>();
            values.Add("aa", 1);
            values.Add("cc", "test");

            var parameters = new Dictionary<string, object>();

            var sql = TSQLGenerator.BuildSelect(tableMD, values, ref parameters);

            Assert.AreEqual("SELECT [a] AS 'aa',[b] AS 'bb',[c] AS 'cc' FROM [testschema].[testtable] WHERE [a]=@aa AND [c]=@cc", sql);
            Assert.AreEqual(2, parameters.Count);
        }

        [TestMethod]
        public void BuildSelectList_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var values = new Dictionary<string, object>();
            values.Add("aa", new int[] { 1, 2, 3 });
            values.Add("cc", "test");

            var parameters = new Dictionary<string, object>();

            var sql = TSQLGenerator.BuildSelect(tableMD, values, ref parameters);

            Assert.AreEqual("SELECT [a] AS 'aa',[b] AS 'bb',[c] AS 'cc' FROM [testschema].[testtable] WHERE [a] IN @aa AND [c]=@cc", sql);
            Assert.AreEqual(2, parameters.Count);
        }

        [TestMethod]
        public void BuildInsert_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var sql = TSQLGenerator.BuildInsert(tableMD, withoutKeys: false);

            Assert.AreEqual("INSERT INTO [testtable] ([a],[b],[c]) VALUES (@aa,@bb,@cc)", sql);
        }

        [TestMethod]
        public void BuildInsert_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildInsert(tableMD, withoutKeys: false);

            Assert.AreEqual("INSERT INTO [testschema].[testtable] ([a],[b],[c]) VALUES (@aa,@bb,@cc)", sql);
        }

        [TestMethod]
        public void BuildUpdate_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var sql = TSQLGenerator.BuildUpdate(tableMD);

            Assert.AreEqual("UPDATE [testtable] SET [b]=@bb,[c]=@cc WHERE [a]=@aa", sql);
        }

        [TestMethod]
        public void BuildUpdate_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildUpdate(tableMD);

            Assert.AreEqual("UPDATE [testschema].[testtable] SET [b]=@bb,[c]=@cc WHERE [a]=@aa", sql);
        }


        [TestMethod]
        public void BuildDelete_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var sql = TSQLGenerator.BuildDelete(tableMD);

            Assert.AreEqual("DELETE FROM [testtable] WHERE [a]=@aa", sql);
        }

        [TestMethod]
        public void BuildDelete_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildDelete(tableMD);

            Assert.AreEqual("DELETE FROM [testschema].[testtable] WHERE [a]=@aa", sql);
        }


        [TestMethod]
        public void BuildCreate_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var sql = TSQLGenerator.BuildCreate(tableMD);

            Assert.AreEqual(@"CREATE TABLE [dbo].[testtable](
   [a]  NOT NULL,
   [b]  NOT NULL,
   [c]  NOT NULL,
CONSTRAINT [PK_testtable] PRIMARY KEY CLUSTERED ( [a] ASC )
);
", sql);
        }

        [TestMethod]
        public void BuildCreate_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildCreate(tableMD, true);

            Assert.AreEqual(@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[testschema].[testtable]') AND type in (N'U'))
BEGIN
CREATE TABLE [testschema].[testtable](
   [a]  NOT NULL,
   [b]  NOT NULL,
   [c]  NOT NULL,
CONSTRAINT [PK_testtable] PRIMARY KEY CLUSTERED ( [a] ASC )
);
END
", sql);
        }

        [TestMethod]
        public void BuildCreate_WithSchemaAndMultipleKeys()
        {
            var tableMD = GetTestMetadata();
            tableMD.Properties.Add(new PropertyMetadata()
            {
                IsPK = true,
                ColumnName = "a2",
                PropertyName = "aa22"
            });
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildCreate(tableMD, true);

            Assert.AreEqual(@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[testschema].[testtable]') AND type in (N'U'))
BEGIN
CREATE TABLE [testschema].[testtable](
   [a]  NOT NULL,
   [a2]  NOT NULL,
   [b]  NOT NULL,
   [c]  NOT NULL,
CONSTRAINT [PK_testtable] PRIMARY KEY CLUSTERED ( [a] ASC,[a2] ASC )
);
END
", sql);
        }

        [TestMethod]
        public void BuildCreate_WithSchemaAndNoKeys()
        {
            var tableMD = GetTestMetadata();
            tableMD.Properties.Where(x => x.IsPK).ToList().ForEach(x => x.IsPK = false);
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildCreate(tableMD, true);

            Assert.AreEqual(@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[testschema].[testtable]') AND type in (N'U'))
BEGIN
CREATE TABLE [testschema].[testtable](
   [a]  NOT NULL,
   [b]  NOT NULL,
   [c]  NOT NULL,

);
END
", sql);
        }


        [TestMethod]
        public void BuildParameterList_WithKeys()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildParameterList(tableMD, withoutKeys: false);

            Assert.AreEqual("@aa,@bb,@cc", sql);
        }

        [TestMethod]
        public void BuildParameterList_WithoutKeys()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = TSQLGenerator.BuildParameterList(tableMD, withoutKeys: true);

            Assert.AreEqual("@bb,@cc", sql);
        }

        private static TableMetadata GetTestMetadata()
        {
            var tableMD = new TableMetadata();
            tableMD.TableName = "testtable";

            tableMD.Properties.Add(new PropertyMetadata()
            {
                IsPK = true,
                ColumnName = "a",
                PropertyName = "aa"
            });
            tableMD.Properties.Add(new PropertyMetadata()
            {
                ColumnName = "b",
                PropertyName = "bb"
            });
            tableMD.Properties.Add(new PropertyMetadata()
            {
                ColumnName = "c",
                PropertyName = "cc"
            });
            return tableMD;
        }
    }
}

