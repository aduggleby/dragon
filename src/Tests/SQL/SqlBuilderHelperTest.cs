using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Dragon.SQL;

namespace Dragon.Tests.SQL
{
    [TestClass]
    public class SqlBuilderHelperTest
    {
        [TestMethod]
        public void FindUniqueNameInDictionary()
        {
            var dict = new Dictionary<string, object>();
            var subject = "test";

            var s1 = SqlBuilderHelper.FindUniqueNameInDictionary(subject, dict);
            Assert.IsTrue(s1.Equals("test"));
            dict.Add(s1, null);
            var s2 = SqlBuilderHelper.FindUniqueNameInDictionary(subject, dict);
            Assert.IsTrue(s2.Equals("test2"));
            dict.Add(s2, null);
            var s3 = SqlBuilderHelper.FindUniqueNameInDictionary(subject, dict);
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

            var sql = SqlBuilderHelper.BuildWhereClause(tableMD, values, ref parameters);

            Assert.AreEqual("[a]=@aa AND [c]=@cc", sql);
            Assert.AreEqual(2, parameters.Count);
        }

        [TestMethod]
        public void BuildColumnList()
        {
            var tableMD = GetTestMetadata();

            var sql = SqlBuilderHelper.BuildColumnList(tableMD);

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

            var sql = SqlBuilderHelper.BuildSelect(tableMD, values, ref parameters);

            Assert.AreEqual("SELECT [a],[b],[c] FROM [testtable] WHERE [a]=@aa AND [c]=@cc", sql);
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

            var sql = SqlBuilderHelper.BuildSelect(tableMD, values, ref parameters);

            Assert.AreEqual("SELECT [a],[b],[c] FROM [testschema].[testtable] WHERE [a]=@aa AND [c]=@cc", sql);
            Assert.AreEqual(2, parameters.Count);
        }

        [TestMethod]
        public void BuildInsert_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var sql = SqlBuilderHelper.BuildInsert(tableMD);

            Assert.AreEqual("INSERT INTO [testtable] ([a],[b],[c]) VALUES (@aa,@bb,@cc)", sql);
        }

        [TestMethod]
        public void BuildInsert_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = SqlBuilderHelper.BuildInsert(tableMD);

            Assert.AreEqual("INSERT INTO [testschema].[testtable] ([a],[b],[c]) VALUES (@aa,@bb,@cc)", sql);
        }

        [TestMethod]
        public void BuildUpdate_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var sql = SqlBuilderHelper.BuildUpdate(tableMD);

            Assert.AreEqual("UPDATE [testtable] SET [b]=@bb,[c]=@cc WHERE [a]=@aa", sql);
        }

        [TestMethod]
        public void BuildUpdate_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = SqlBuilderHelper.BuildUpdate(tableMD);

            Assert.AreEqual("UPDATE [testschema].[testtable] SET [b]=@bb,[c]=@cc WHERE [a]=@aa", sql);
        }


        [TestMethod]
        public void BuildDelete_NoSchema()
        {
            var tableMD = GetTestMetadata();
            Assert.IsNull(tableMD.Schema);

            var sql = SqlBuilderHelper.BuildDelete(tableMD);

            Assert.AreEqual("DELETE FROM [testtable] WHERE [a]=@aa", sql);
        }

        [TestMethod]
        public void BuildDelete_WithSchema()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = SqlBuilderHelper.BuildDelete(tableMD);

            Assert.AreEqual("DELETE FROM [testschema].[testtable] WHERE [a]=@aa", sql);
        }

        
        [TestMethod]
        public void BuildParameterList_WithKeys()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = SqlBuilderHelper.BuildParameterList(tableMD, withoutKeys: false);

            Assert.AreEqual("@aa,@bb,@cc", sql);
        }

        [TestMethod]
        public void BuildParameterList_WithoutKeys()
        {
            var tableMD = GetTestMetadata();
            tableMD.Schema = "testschema";

            var sql = SqlBuilderHelper.BuildParameterList(tableMD, withoutKeys: true);

            Assert.AreEqual("@bb,@cc", sql);
        }

        private static TableMetadata GetTestMetadata()
        {
            var tableMD = new TableMetadata();
            tableMD.TableName = "testtable";

            tableMD.Properties.Add(new PropertyMetadata()
            {
                IsPK= true,
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

