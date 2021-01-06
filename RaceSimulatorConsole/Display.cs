using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Controller;
using Model;

namespace RaceSimulator
{
    public static class Display
    {
        private static int compass;
        private static List<GridSquare> GridSquares;

        #region graphics

        // Finish
        private static string[] _finishVertical = { "║█ █ █ █ █ █║", "║ █ █ █ █ █ ║", "║█ █.█ █ █ █║", "║ █ █ █ █ █ ║", "║█ █ █ █,█ █║", "║ █ █ █ █ █ ║", "║█ █ █ █ █ █║", };
        private static string[] _finishHorizontal = { "═════════════", " █ █ █ █ █ █ ", "█ █ █ █.█ █ █", " █ █ █ █ █ █ ", "█ █ █ █,█ █ █", " █ █ █ █ █ █ ", "═════════════" };

        // STRAIGHT
        private static string[] _straightVertical = { "║           ║", "║           ║", "║ ,         ║", "║         . ║", "║           ║", "║           ║", "║           ║" };
        private static string[] _straightHorizontal = { "═════════════", "      .      ", "             ", "             ", "             ", "      ,      ", "═════════════", };

        //CORNERS
        private static string[] _cornerSE = { "╔════════════", "║      ,     ", "║            ", "║            ", "║            ", "║          . ", "║           ╔" };
        private static string[] _cornerSW = { "════════════╗", "       .    ║", "            ║", "            ║", "            ║", "  ,         ║", "╗           ║" };
        private static string[] _cornerNW = { "╝           ║", " ,          ║", "            ║", "            ║", "            ║", "  .         ║", "════════════╝" };
        private static string[] _cornerNE = { "║           ╚", "║        ,   ", "║            ", "║            ", "║            ", "║ .          ", "╚════════════", };


        // START
        private static string[] _startHorizontal = { "═════════════", "> > > > > > >", " > > >.> > > ", "> > > > > > >", " > > >,> > > ", "> > > > > > >", "═════════════", };
        private static string[] _startVertical = { "║           ║", "║           ║", "║ .         ║", "║         , ║", "║           ║", "║           ║", "║           ║", };


        // EMPTY
        private static string[] _empty = { "             ", "             ", "             ", "             ", "             ", "             ", "             " };

        #endregion

        public static void Initialise()
        {
            compass = 1;
            GridSquares = new List<GridSquare>();
            Data.CurrentRace.DriversChanged += OnDriversChanged;
            Race.RaceStarted += OnRaceStarted;
        }

        public static void OnDriversChanged(object sender, EventArgs e)
        {
            DriversChangedEventArgs e1 = (DriversChangedEventArgs)e;
            DrawTrack(e1.Track);
        }

        public static void OnRaceStarted(object sender, EventArgs e)
        {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.DriversChanged += OnDriversChanged;
            Console.Clear();
            DrawTrack(e1.Race.Track);
        }

        public static void DrawTrack(Track track)
        {
            Console.SetCursorPosition(0, 0);
            CalculateGrid(track.Sections);
            MoveGrid(Math.Abs(GridSquare.LowestX), Math.Abs(GridSquare.LowestY));
            GridSquares = GridSquares.OrderBy(_square => _square.Y).ToList();
            int maxX = GridSquares.Max(_square => _square.X);
            int maxY = GridSquares.Max(_square => _square.Y);
            for (int y = 0; y <= maxY; y++)
            {
                for (int internalY = 0; internalY < 7; internalY++)
                {
                    for (int x = 0; x <= maxX; x++)
                    {
                        GridSquare square = GetGridSquare(x, y);
                        Console.Write(square == null ? _empty[internalY] : InsertParticipants(square.Section[internalY], square.SectionData.Left, square.SectionData.Right));
                    }
                    Console.WriteLine();
                }
            }
            DrawScore();
        }

        public static void DrawScore()
        {
            int maxX = GridSquares.Max(_square => _square.X);
            Console.SetCursorPosition((maxX + 5) * 4, 0);
            var bestParticipant = Data.Competition.ContestorOvertaken.GetLeadingContestor();
            Console.Write($"{bestParticipant} heeft het vaakst ingehaald");
        }

        public static string InsertParticipants(string track, IParticipant leftParticipant, IParticipant rightParticipant)
        {
            char initial1 = leftParticipant?.Name[0] ?? ' ';
            char initial2 = rightParticipant?.Name[0] ?? ' ';
            if (leftParticipant != null && leftParticipant.Equipment.IsBroken)
            {
                initial1 = '×';
                Debug.WriteLine($"{leftParticipant.Name} is gechrasht");

            }
            if (rightParticipant != null && rightParticipant.Equipment.IsBroken)
            {
                initial2 = '×';
                Debug.WriteLine($"{leftParticipant.Name} is gechrasht");
            }
            string returnValue = track.Replace('.', initial1);
            returnValue = returnValue.Replace(',', initial2);
            return returnValue;
        }


        private static GridSquare GetGridSquare(int x, int y)
        {
            GridSquare square = GridSquares.Find(_square => _square.X == x && _square.Y == y);
            return square;
        }

        private static void CalculateGrid(LinkedList<Section> sections)
        {
            Race race = Data.CurrentRace;
            int comp = compass;
            int x = 0;
            int y = 0;
            GridSquares.Clear();
            GridSquare.LowestX = 0;
            GridSquare.LowestY = 0;
            foreach (Section section in sections)
            {
                SectionTypes type = section.SectionType;
                SectionData data = race.GetSectionData(section);
                switch (type)
                {
                    case SectionTypes.StartGrid:
                        if (comp == 1 || comp == 3)
                            GridSquares.Add(new GridSquare(x, y, _startHorizontal, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _startVertical, data));
                        break;
                    case SectionTypes.Straight:
                        if (comp == 1 || comp == 3)
                            GridSquares.Add(new GridSquare(x, y, _straightHorizontal, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _straightVertical, data));
                        break;
                    case SectionTypes.LeftCorner:
                        if (comp == 1)
                            GridSquares.Add(new GridSquare(x, y, _cornerNW, data));
                        else if (comp == 2)
                            GridSquares.Add(new GridSquare(x, y, _cornerNE, data));
                        else if (comp == 3)
                            GridSquares.Add(new GridSquare(x, y, _cornerSE, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _cornerSW, data));
                        comp = (comp - 1) % 4;
                        if (comp < 0)
                            comp = 3;
                        break;
                    case SectionTypes.RightCorner:
                        if (comp == 1)
                            GridSquares.Add(new GridSquare(x, y, _cornerSW, data));
                        else if (comp == 2)
                            GridSquares.Add(new GridSquare(x, y, _cornerNW, data));
                        else if (comp == 3)
                            GridSquares.Add(new GridSquare(x, y, _cornerNE, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _cornerSE, data));
                        comp = (comp + 1) % 4;
                        break;
                    case SectionTypes.Finish:
                        if (comp == 1 || comp == 3)
                            GridSquares.Add(new GridSquare(x, y, _finishHorizontal, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _finishVertical, data));
                        break;
                }
                if (comp == 0)
                {
                    y--;
                }
                else if (comp == 1)
                {
                    x++;
                }
                else if (comp == 2)
                {
                    y++;
                }
                else
                {
                    x--;
                }
            }
        }

        private static void MoveGrid(int x, int y)
        {
            foreach (GridSquare square in GridSquares)
            {
                square.X += x;
                square.Y += y;
            }
        }
    }
}