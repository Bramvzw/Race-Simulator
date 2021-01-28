using Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Model
{
    [TestFixture]
    public class Model_DriverData
    {

        Driver d1, d2;

        [SetUp]
        public void Setup()
        {
            d1 = new Driver("Byron", new Car());
            d2 = new Driver("Harvick", new Car());
        }

        [Test]
        public void DriverToString()
        {
            string expected = "Bestuurder Byron";
            string actual = d1.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AreEqual()
        {
            Assert.AreNotEqual(d1, d2);
        }

        [Test]
        public void Equal_IsNotDriver()
        {
            bool expected = false;
            Car c = new Car();
            bool actual = d1.Equals(c);
            Assert.AreEqual(expected, actual);
        }
    }
}
