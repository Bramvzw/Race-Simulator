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
        private static int direction;
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
        private static string[] Empty = { "             ", "             ", "             ", "             ", "             ", "             ", "             " };

        #endregion

        public static void Initialise()
        {
            direction = 1;
            GridSquares = new List<GridSquare>();
            Data.CurrentRace.DriversChanged += OnDriversChanged;
            Race.RaceStarted += OnRaceStarted;
        }

        public static void OnDriversChanged(object sender, EventArgs e)
        {
            DriversChangedEventArgs track = (DriversChangedEventArgs)e;
            DrawTrack(track.Track);
        }


        // Draws track
        public static void DrawTrack(Track track)
        {
            Console.SetCursorPosition(0, 0);
            DetermineGrid(track.Sections);
            MoveGrid(Math.Abs(GridSquare.XCoordinate), Math.Abs(GridSquare.YCoordinate));
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
                        Console.Write(square == null ? Empty[internalY] : InsertParticipants(square.Section[internalY], square.SectionData.Left, square.SectionData.Right));
                    }
                    Console.WriteLine();
                }
            }
            DrawScore();
        }

        public static void DrawScore()
        {
            int maxX = GridSquares.Max(_square =>
            {
                if (_square is null)
                {
                    throw new ArgumentNullException(nameof(_square));
                }

                return _square.X;
            });
            Console.SetCursorPosition((maxX + 5) * 4, 0);
            var bestParticipant = Data.Competition.ContestorOvertaken.GetLeadingContestor();
            Console.Write($"{bestParticipant} heeft het vaakst ingehaald");
        }

        public static void OnRaceStarted(object sender, EventArgs e)
        {
            RaceStartedEventArgs track = (RaceStartedEventArgs)e;
            track.Race.DriversChanged += OnDriversChanged;
            Console.Clear();
            DrawTrack(track.Race.Track);
        }

        public static string InsertParticipants(string track, IParticipant leftParticipant, IParticipant rightParticipant)
        {
            char initial1 = leftParticipant?.Name[0] ?? ' ';
            char initial2 = rightParticipant?.Name[0] ?? ' ';
            if (leftParticipant != null && leftParticipant.Equipment.Broken)
            {
                initial1 = '×';
                Debug.WriteLine($"{leftParticipant.Name} is gechrasht");

            }
            if (rightParticipant != null && rightParticipant.Equipment.Broken)
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

        // Determines the placement and direction of grids
        private static void DetermineGrid(LinkedList<Section> sections)
        {
            Race race = Data.CurrentRace;
            int direct = direction;
            int x = 0, y = 0;
            GridSquares?.Clear();
            GridSquare.XCoordinate = 0;
            GridSquare.YCoordinate = 0;
            foreach (Section section in sections)
            {
                SectionTypes type = section.SectionType;
                SectionData data = race.GetSectionData(section);
                switch (type)
                {
                    // Check if a grid needs to be horizontal or vertical

                    // StartGrid
                    case SectionTypes.StartGrid:
                        if (direct == 1 || direct == 3)
                            GridSquares.Add(new GridSquare(x, y, _startHorizontal, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _startVertical, data));
                        break;

                    // Straight grid
                    case SectionTypes.Straight:
                        if (direct == 1 || direct == 3)
                        {
                            GridSquares.Add(new GridSquare(x, y, _straightHorizontal, data));
                        }
                        else
                        {
                            GridSquares.Add(new GridSquare(x, y, _straightVertical, data));
                        }
                        break;

                    // Finish grid
                    case SectionTypes.Finish:
                        if (direct == 1 || direct == 3)
                        {
                            GridSquares.Add(new GridSquare(x, y, _finishHorizontal, data));
                        }
                        else
                        {
                            GridSquares.Add(new GridSquare(x, y, _finishVertical, data));
                        }
                        break;

                    // Check which direction a corner needs to be placed

                    // Leftcorners
                    case SectionTypes.LeftCorner:
                        if (direct == 1)
                            GridSquares.Add(new GridSquare(x, y, _cornerNW, data));
                        else if (direct == 2)
                            GridSquares.Add(new GridSquare(x, y, _cornerNE, data));
                        else if (direct == 3)
                            GridSquares.Add(new GridSquare(x, y, _cornerSE, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _cornerSW, data));
                        // Determines the next X,Y coördinates of a grid
                        direct = (direct - 1) % 4;
                        if (direct < 0)
                            direct = 3;
                        break;

                    // RightCorners
                    case SectionTypes.RightCorner:
                        if (direct == 1)
                            GridSquares.Add(new GridSquare(x, y, _cornerSW, data));
                        else if (direct == 2)
                            GridSquares.Add(new GridSquare(x, y, _cornerNW, data));
                        else if (direct == 3)
                            GridSquares.Add(new GridSquare(x, y, _cornerNE, data));
                        else
                            GridSquares.Add(new GridSquare(x, y, _cornerSE, data));
                        //Determines the next X, Y coördinates of a grid
                        direct = (direct + 1) % 4;
                        break;
                }
                // Sets the X and Y coördinates of the next grid
                if (direct == 0)
                {
                    y--;
                }
                else if (direct == 1)
                {
                    x++;
                }
                else if (direct == 2)
                {
                    y++;
                }
                else
                {
                    x--;
                }
            }
        }

            // Gives the coördinates to each grid
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