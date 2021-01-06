using Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Model {
    [TestFixture]
    public class Model_Driver {

        Driver d1, d2;

        [SetUp]
        public void Setup() {
            d1 = new Driver("Bob", new Car());
            d2 = new Driver("John", new Car());
        }

        [Test]
        public void DriverToString() {
            string expected = "Driver Bob";
            string actual = d1.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AreEqual() {
            Assert.AreNotEqual(d1, d2);
        }

        [Test]
        public void Equal_IsNotDriver() {
            bool expected = false;
            Car c = new Car();
            bool actual = d1.Equals(c);
            Assert.AreEqual(expected, actual);
        }
    }
}
