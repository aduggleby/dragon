using System;
using System.Linq;
using Dapper;
using Dragon.Data.Attributes;
using Dragon.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Dragon.Data.Test
{
    [TestClass]
    public class MetadataHelperTest
    {
        [TestMethod]
        public void IfAttributeElse_TypeHasAttribute()
        {
            var test = MetadataHelper.IfAttributeElse<TestClassAttribute, bool>(
                typeof(MetadataHelperTest),
                typeof(TestClassAttribute),
                att => true, () => false);
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void IfAttributeElse_TypeDoesNotHaveAttribute()
        {
            var test = MetadataHelper.IfAttributeElse<TestMethodAttribute, bool>(
                typeof(MetadataHelperTest),
                typeof(TestMethodAttribute),
                att => true, () => false);
            Assert.IsFalse(test);
        }

        [TestMethod]
        public void IfAttributeElse_PropertyHasAttribute()
        {
            var test = MetadataHelper.IfAttributeElse<ColumnAttribute, bool>(
                typeof(TestClass).GetProperty("TestWithColumn"),
                typeof(ColumnAttribute),
                att => true, () => false);
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void IfAttributeElse_PropertyDoesNotHaveAttribute()
        {
            var test = MetadataHelper.IfAttributeElse<ColumnAttribute, bool>(
                typeof(TestClass).GetProperty("TestNoColumn"),
                typeof(ColumnAttribute),
                att => true, () => false);
            Assert.IsFalse(test);
        }

        [TestMethod]
        public void MetadataForProperty_NoAttributes()
        {
            var metadata = new PropertyMetadata();

            MetadataHelper.MetadataForProperty(
                typeof(TestClass).GetProperty("TestNoColumn"),
                ref metadata);

            Assert.AreEqual("TestNoColumn", metadata.ColumnName);
            Assert.AreEqual("TestNoColumn", metadata.PropertyName);
            Assert.IsFalse(metadata.IsPK);
        }

        [TestMethod]
        public void MetadataForProperty_WithColumnAttribute()
        {
            var metadata = new PropertyMetadata();

            MetadataHelper.MetadataForProperty(
                typeof(TestClass).GetProperty("TestWithColumn"),
                ref metadata);

            Assert.AreEqual("SqlColumn", metadata.ColumnName);
            Assert.AreEqual("TestWithColumn", metadata.PropertyName);
        }

        [TestMethod]
        public void MetadataForProperty_WithKeyAttribute()
        {
            var metadata = new PropertyMetadata();

            MetadataHelper.MetadataForProperty(
                typeof(TestClass).GetProperty("Key"),
                ref metadata);

            Assert.IsTrue(metadata.IsPK);
        }

        [TestMethod]
        public void MetadataForProperty_WithLengthAttribute()
        {
            var metadata = new PropertyMetadata();

            MetadataHelper.MetadataForProperty(
                typeof(TestClass).GetProperty("Length"),
                ref metadata);

            Assert.AreEqual("100", metadata.Length);
        }

        [TestMethod]
        public void MetadataForProperty_WithLengthMaxAttribute()
        {
            var metadata = new PropertyMetadata();

            MetadataHelper.MetadataForProperty(
                typeof(TestClass).GetProperty("LengthMax"),
                ref metadata);

            Assert.AreEqual("MAX", metadata.Length);
            Assert.IsFalse(metadata.IsPK);
            Assert.AreEqual("LengthMax", metadata.PropertyName);

        }

        [TestMethod]
        public void MetadataForProperty_WithMultipleAttributes()
        {
            var metadata = new PropertyMetadata();

            MetadataHelper.MetadataForProperty(
                typeof(TestClass).GetProperty("Multiple"),
                ref metadata);

            Assert.AreEqual("100", metadata.Length);
            Assert.IsFalse(metadata.IsPK);
            Assert.AreEqual("SqlColumnMulti", metadata.ColumnName);
            Assert.AreEqual("Multiple", metadata.PropertyName);

        }


        [TestMethod]
        public void MetadataForClass()
        {
            var metadata = new TableMetadata();

            MetadataHelper.MetadataForClass(typeof(TestClass), ref metadata);


            Assert.AreEqual("TestClass", metadata.ClassName);
            Assert.AreEqual("SqlTable", metadata.TableName);

            Assert.AreEqual(6, metadata.Properties.Count);
            Assert.AreEqual(1, metadata.Properties.Count(x => x.IsPK));
        }


        [Table("SqlTable")]
        public class TestClass
        {
            public string TestNoColumn { get; set; }

            [Column("SqlColumn")]
            public string TestWithColumn { get; set; }

            [Key]
            public Guid Key { get; set; }

            [Length(100)]
            public string Length { get; set; }

            [Length("MAX")]
            public string LengthMax { get; set; }

            [Length(100)]
            [Column("SqlColumnMulti")]
            public string Multiple { get; set; }
        }
    }
}
