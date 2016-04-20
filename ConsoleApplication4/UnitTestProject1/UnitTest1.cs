using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ConsoleApplication4;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        string Source = "";

        [TestInitialize]
        public void ReadFromFile()
        {
            this.Source = File.ReadAllText(@"json1.json");
        }

        [TestMethod]
        public void GetNumberOfCDsTest()
        {
            Parser p = new Parser();
            var result = p.GetAuthorsWithCds(this.Source);
            Assert.IsTrue(result.Count.Equals(3));
        }

        [TestMethod]
        public void GetLongCDsTest()
        {
            Parser p = new Parser();
            var result = p.GetLongCDs(this.Source);
            Assert.IsTrue(result.Count.Equals(2));
        }

        [TestMethod]
        public void GetTopItems()
        {
            Parser p = new Parser();
            var result = p.GetTopResults(this.Source);
        }

        [TestMethod]
        public void GetItemsWithYearName()
        {
            Parser p = new Parser();
            var result = p.GetItemsWithYearName(this.Source);
            Assert.IsTrue(result.Contains("BlackSands"));
            Assert.IsTrue(result.Contains("A history of the world in 20 chapters"));

            this.Source = @"[{""price"":15.99,""chapters"":[""one"",""two"",""three""],""year"":1999,""title"":""foo"",""author"":""mary"",""type"":""book""},{""price"":11.99,""minutes"":90,""year"":2004,""title"":""bar"",""director"":""alan"",""type"":""dvd""},{""price"":15.99,""tracks"":[{""seconds"":180,""name"":""one""},{""seconds"":200,""name"":""two""}],""year"":2000,""title"":""baz2008"",""author"":""joan"",""type"":""cd""}]";
            result = p.GetItemsWithYearName(this.Source);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(result[0].ToString(), "baz2008");
        }
    }
}
