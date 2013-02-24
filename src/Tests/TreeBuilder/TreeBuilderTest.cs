using Dragon.Common.Objects.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Dragon.Interfaces;
using FluentAssertions;
using System.Linq;

namespace Dragon.Tests
{


    /// <summary>
    ///This is a test class for TreeBuilderTest and is intended
    ///to contain all TreeBuilderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TreeBuilderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod()]
        public void TwoLevels()
        {
            var nodes = new List<FakeNode>();
            nodes.Add(new FakeNode() { Parent = "A", Child = "A1" });
            nodes.Add(new FakeNode() { Parent = "A", Child = "A2" });

            var tree = TreeBuilder.Build<string, object, FakeNode>(nodes, x => x.Parent, x => x.Child, x => (object)null);

            tree.Count().Should().Be(1);

            var a = tree.FirstOrDefault();
            a.Children.Count().Should().Be(2);

            var a1 = a.Children.FirstOrDefault(X => X.Node == "A1");
            a1.Should().NotBeNull();
            a1.Children.Count().Should().Be(0);

            var a2 = a.Children.FirstOrDefault(X => X.Node == "A2");
            a2.Should().NotBeNull();
            a2.Children.Count().Should().Be(0);
        }

        [TestMethod()]
        public void ThreeLevels()
        {
            var nodes = new List<FakeNode>();
            nodes.Add(new FakeNode() { Parent = "A", Child = "A1" });
            nodes.Add(new FakeNode() { Parent = "A1", Child = "A11" });
            nodes.Add(new FakeNode() { Parent = "A1", Child = "A12" });
            nodes.Add(new FakeNode() { Parent = "A", Child = "A2" });

            var tree = TreeBuilder.Build<string, object, FakeNode>(nodes, x => x.Parent, x => x.Child, x => (object)null);

            tree.Count().Should().Be(1);

            var a = tree.FirstOrDefault();
            a.Children.Count().Should().Be(2);

            var a1 = a.Children.FirstOrDefault(X => X.Node == "A1");
            a1.Should().NotBeNull();
            a1.Children.Count().Should().Be(2);

            var a11 = a1.Children.FirstOrDefault(X => X.Node == "A11");
            a11.Should().NotBeNull();
            a11.Children.Count().Should().Be(0);

            var a12 = a1.Children.FirstOrDefault(X => X.Node == "A12");
            a12.Should().NotBeNull();
            a12.Children.Count().Should().Be(0);

            var a2 = a.Children.FirstOrDefault(X => X.Node == "A2");
            a2.Should().NotBeNull();
            a2.Children.Count().Should().Be(0);
        }

        [TestMethod()]
        public void ThreeLevelsWithDoubleNode()
        {
            var nodes = new List<FakeNode>();
            nodes.Add(new FakeNode() { Parent = "A", Child = "A1" });
            nodes.Add(new FakeNode() { Parent = "A1", Child = "A11" });
            nodes.Add(new FakeNode() { Parent = "A1", Child = "A12" });
            nodes.Add(new FakeNode() { Parent = "A", Child = "A2" });
            nodes.Add(new FakeNode() { Parent = "A2", Child = "S" });
            nodes.Add(new FakeNode() { Parent = "A11", Child = "S" });

            var tree = TreeBuilder.Build<string, object, FakeNode>(nodes, x => x.Parent, x => x.Child, x => (object)null);

            tree.Count().Should().Be(1);

            var a = tree.FirstOrDefault();
            a.Children.Count().Should().Be(2);

            var a1 = a.Children.FirstOrDefault(X => X.Node == "A1");
            a1.Should().NotBeNull();
            a1.Children.Count().Should().Be(2);

            var a11 = a1.Children.FirstOrDefault(X => X.Node == "A11");
            a11.Should().NotBeNull();
            a11.Children.Count().Should().Be(1);

            var a12 = a1.Children.FirstOrDefault(X => X.Node == "A12");
            a12.Should().NotBeNull();
            a12.Children.Count().Should().Be(0);

            var a2 = a.Children.FirstOrDefault(X => X.Node == "A2");
            a2.Should().NotBeNull();
            a2.Children.Count().Should().Be(1);
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryingToCreateLoop()
        {
            var nodes = new List<FakeNode>();
            nodes.Add(new FakeNode() { Parent = "A", Child = "A1" });
            nodes.Add(new FakeNode() { Parent = "A1", Child = "A11" });
            nodes.Add(new FakeNode() { Parent = "A1", Child = "A12" });
            nodes.Add(new FakeNode() { Parent = "A", Child = "A2" });
            nodes.Add(new FakeNode() { Parent = "A12", Child = "A" });

            var tree = TreeBuilder.Build<string, object, FakeNode>(nodes, x => x.Parent, x => x.Child, x => (object)null);
        }

        [TestMethod()]
        public void Data_on_root()
        {
            var nodes = new List<FakeNode>();
            nodes.Add(new FakeNode() { Parent = "A", Child = "A1" });

            var rights = new List<KeyValuePair<string, string>>();
            rights.Add(new KeyValuePair<string, string>("A", "1"));
            rights.Add(new KeyValuePair<string, string>("A", "2"));

            var tree = TreeBuilder.Build<string, IEnumerable<string>, FakeNode>(
                nodes,
                x => x.Parent,
                x => x.Child,
                x => rights.Where(n => n.Key.Equals(x)).Select(v => v.Value));

            tree.Count().Should().Be(1);

            var a = tree.FirstOrDefault();
            a.Children.Count().Should().Be(1);

            a.Data.Should().NotBeNull();
            a.Data.Count().Should().Be(2);
            a.Data.FirstOrDefault(x => x == "1").Should().NotBeNull();
            a.Data.FirstOrDefault(x => x == "2").Should().NotBeNull();
        }

        [TestMethod()]
        public void Data_on_child()
        {
            var nodes = new List<FakeNode>();
            nodes.Add(new FakeNode() { Parent = "A", Child = "A1" });

            var rights = new List<KeyValuePair<string, string>>();
            rights.Add(new KeyValuePair<string, string>("A1", "1"));
            rights.Add(new KeyValuePair<string, string>("A1", "2"));

            var tree = TreeBuilder.Build<string, IEnumerable<string>, FakeNode>(
                nodes,
                x => x.Parent,
                x => x.Child,
                x => rights.Where(n => n.Key.Equals(x)).Select(v => v.Value));

            tree.Count().Should().Be(1);

            var a = tree.FirstOrDefault();
            a.Children.Count().Should().Be(1);
            
            var a1 = a.Children.First();
            a1.Data.Should().NotBeNull();
            a1.Data.Count().Should().Be(2);
            a1.Data.FirstOrDefault(x => x == "1").Should().NotBeNull();
            a1.Data.FirstOrDefault(x => x == "2").Should().NotBeNull();
        }

        private class FakeNode
        {
            public string Parent { get; set; }
            public string Child { get; set; }
        }
    }
}
