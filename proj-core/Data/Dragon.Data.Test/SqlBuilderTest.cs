using System;
using System.Collections.Generic;
using System.Text;
using Dragon.Data.Attributes;
using Dragon.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Data.Test
{
    [TestClass]
    public class SqlBuilderTest
    {
        [TestMethod]
        public void Or_Expression()
        {
            var expr =
                    new Where<TestClass>().Like(x => x.A, "Alice").Or().SmallerThan(x => x.B, 2);

            var param = new Dictionary<string, object>();

            var sql = expr.Build(param).ToString();

            Assert.AreEqual(
                "WHERE [AA] LIKE @A OR [B]<@B",
                sql);

            Assert.AreEqual(2, param.Count);

        }

        [TestMethod]
        public void Or_Expression_2()
        {
            var expr =
                new Where<TestClass>().Group(g => g.Like(x => x.A, "Alice").Or().SmallerThan(x => x.B, 2));

            var param = new Dictionary<string, object>();

            var sql = expr.Build(param).ToString();

            Assert.AreEqual(
                "WHERE ([AA] LIKE @A OR [B]<@B)",
                sql);

            Assert.AreEqual(2, param.Count);

        }

        [TestMethod]
        public void And_Expression()
        {
            var expr =
                    new Where<TestClass>().Like(x => x.A, "Alice").And().SmallerThan(x => x.B, 2);

            var param = new Dictionary<string, object>();

            var sql = expr.Build(param).ToString();

            Assert.AreEqual(
                "WHERE [AA] LIKE @A AND [B]<@B",
                sql);

            Assert.AreEqual(2, param.Count);

        }

        [TestMethod]
        public void And_Expression_2()
        {
            var expr =
                new Where<TestClass>().Group(g => g.Like(x => x.A, "Alice").And().SmallerThan(x => x.B, 2));

            var param = new Dictionary<string, object>();

            var sql = expr.Build(param).ToString();

            Assert.AreEqual(
                "WHERE ([AA] LIKE @A AND [B]<@B)",
                sql);

            Assert.AreEqual(2, param.Count);

        }

        [TestMethod]
        public void Or_Used_As_Compound_Expression()
        {
            var expr =
                new Where<TestClass>().IsEqual(x => x.A, "Bob")
                    .Or(g => g.Like(x => x.B, "Alice"));

            var param = new Dictionary<string, object>();

            var sql = expr.Build(param).ToString();

            Assert.AreEqual(
                "WHERE [AA]=@A OR ([B] LIKE @B)",
                sql);

            Assert.AreEqual(2, param.Count);

        }


        [TestMethod]
        public void Complex_Expression_1()
        {
            var expr =
                    new Where<TestClass>().IsEqual(x => x.A, "Bob")
                        .And(g => g.Like(x => x.B, "Alice").Or().SmallerThan(x => x.C, 2))
                        .And(g => g.GreaterThanOrEqualTo(x => x.C, 3).Or().IsEqual(x => x.D, "Chris"));

            var param = new Dictionary<string, object>();

            var sql = expr.Build(param).ToString();

            Assert.AreEqual(
                "WHERE [AA]=@A AND ([B] LIKE @B OR [CC]<@C) AND ([CC]>=@C2 OR [D]=@D)",
                sql);

            Assert.AreEqual(5, param.Count);
        }

     
        public class TestClass
        {
            [Column("AA")]
            public string A { get; set; }

            public string B { get; set; }
            
            [Column("CC")]
            public string C { get; set; }
            
            public string D { get; set; }
        }
    }
}
