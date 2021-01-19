using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaceSimulatorGUI
{
    public class SetIMG
    {

        public int X { get; set; }
        public int Y { get; set; }
        public static int MinX { get; set; }
        public static int MinY { get; set; }
        public string ImagePath { get; set; }
        public int Direction { get; set; }
        public bool Turn { get; set; }

        public SectionData SectionData { get; set; }

        public SetIMG(int x, int y, string imagePath, SectionData sectionData, int compass) : this(x, y, imagePath, sectionData, compass, false) { }

        public SetIMG(int x, int y, string imagePath, SectionData sectionData, int compass, bool flip)
        {
            X = x;
            Y = y;
            ImagePath = imagePath;
            SectionData = sectionData;
            Direction = compass;
            Turn = flip;
            SetLowestCoordinates(x, y);

        }

        private void SetLowestCoordinates(int x, int y)
        {
            if (x < MinX)
                MinX = x;
            if (y < MinY)
                MinY = y;
        }

    }
}
