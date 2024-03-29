﻿using System;
using System.Drawing;
using NUnit.Framework;
using RaceSimulatorGUI;

namespace ControllerTest.GUI
{
    [TestFixture]
    public class GUI_ImageLoader
    {

        [SetUp]
        public void Setup()
        {
            SetAssets.Initialise();
        }

        [Test]
        public void ClearCache_CacheEmpty()
        {
            SetAssets.CreateEmptyBitmap(10, 10);
            SetAssets.ClearCache();
            Assert.IsEmpty(SetAssets.GetImg());
        }

        [Test]
        public void GetImageFromCache()
        {
            Bitmap expected = new Bitmap(@"C:\Users\Bram van Zwolle\source\repos\Race-Simulator\RaceSimulatorGUI\Assets\Blue_Car.png");
            Bitmap actual = SetAssets.LoadImg(@"C:\Users\Bram van Zwolle\source\repos\Race-Simulator\RaceSimulatorGUI\Assets\Blue_Car.png");
            Assert.IsNotEmpty(SetAssets.GetImg());
            Assert.AreEqual(expected.Size, actual.Size);
        }

        [Test]
        public void CreateBitmapSource_BitmapNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => SetAssets.CreateBitmapSourceFromGdiBitmap(null));
        }
    }
}