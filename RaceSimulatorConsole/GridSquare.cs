using Model;

namespace RaceSimulator
{
    public class GridSquare
    {
        public int X { get; set; }
        public int Y { get; set; }
        public static int XCoordinate { get; set; }
        public static int YCoordinate { get; set; }
        public string[] Section { get; set; }

        public SectionData SectionData { get; set; }

        public GridSquare(int x, int y, string[] section, SectionData sectionData)
        {
            X = x;
            Y = y;
            Section = section;
            SectionData = sectionData;
            AppendCoördinates(x, y);
        }

        private void AppendCoördinates(int x, int y)
        {
            if (x < XCoordinate)
                XCoordinate = x;
            if (y < YCoordinate)
                YCoordinate = y;
        }
    }
}