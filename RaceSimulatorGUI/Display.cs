using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Ink;
using System.Windows.Media.Imaging;

namespace RaceSimulatorGUI
{
    public static class Display
    {

        private const int direction = 1;
        private static List<SetIMG> assets;
        private static Track track;
        private static Bitmap bitmap;

        #region Assets
        private static readonly string _start = ".\\Assets\\Start.png";
        private static readonly string _finish = ".\\Assets\\Finish.png";
        private static readonly string _straight = ".\\Assets\\Straight.png";
        private static readonly string _corner = ".\\Assets\\Corner.png";

        private static readonly string _carRed = ".\\Assets\\Red_Car.png";
        private static readonly string _carRedBroken = ".\\Assets\\Broken_Red_Car.png";
        private static readonly string _carGreen = ".\\Assets\\Green_Car.png";
        private static readonly string _carGreenBroken = ".\\Assets\\Broken_Green_Car.png";
        private static readonly string _carGrey = ".\\Assets\\Orange_Car.png";
        private static readonly string _carGreyBroken = ".\\Assets\\Broken_Orange_Car.png";
        private static readonly string _carYellow = ".\\Assets\\Yellow_Car.png";
        private static readonly string _carYellowBroken = ".\\Assets\\Broken_Yellow_Car.png";
        private static readonly string _carBlue = ".\\Assets\\Blue_Car.png";
        private static readonly string _carBlueBroken = ".\\Assets\\Broken_Blue_Car.png";
        #endregion

        public static void Initialise()
        {
            assets = new List<SetIMG>();
            SetAssets.Initialise();
        }

        // retrieve assets from list
        public static List<SetIMG> GetGridSquares()
        {
            return assets;
        }

        // Draw Track
        public static BitmapSource DrawTrack(Track track)
        {
            Bitmap Track = DrawBaseTrack(track);
            Bitmap Driver = DrawParticipants(Track);
            return SetAssets.CreateBitmapSourceFromGdiBitmap(Driver);
        }

        // GridSquares
        private static SetIMG AddGridSquare(int x, int y)
        {
            SetIMG square = assets.Find(_square => _square.X == x && _square.Y == y);
            return square;
        }

        // Get participant
        private static Bitmap GetParticipantImage(IParticipant participant, int direction)
        {
            Bitmap Car = null;

            // Cases for each Teamcolour
            switch (participant.TeamColour)
            {
                case TeamColours.Blue:
                    if (participant.Equipment.Broken)
                    {
                        Car = SetAssets.LoadImg(_carBlueBroken);
                    }
                    else
                    {
                        Car = SetAssets.LoadImg(_carBlue);
                    }
                    break;

                case TeamColours.Green:
                    if (participant.Equipment.Broken)
                    {
                        Car = SetAssets.LoadImg(_carGreenBroken);
                    }
                    else
                    {
                        Car = SetAssets.LoadImg(_carGreen);
                    }
                    break;

                case TeamColours.Orange:
                    if (participant.Equipment.Broken)
                    {
                        Car = SetAssets.LoadImg(_carGreyBroken);
                    }
                    else
                    {
                        Car = SetAssets.LoadImg(_carGrey);
                    }
                    break;

                case TeamColours.Red:
                    if (participant.Equipment.Broken)
                    {
                        Car = SetAssets.LoadImg(_carRedBroken);
                    }
                    else
                    {
                        Car = SetAssets.LoadImg(_carRed);
                    }
                    break;

                case TeamColours.Yellow:
                    if (participant.Equipment.Broken)
                    {
                        Car = SetAssets.LoadImg(_carYellowBroken);
                    }
                    else
                    {
                        Car = SetAssets.LoadImg(_carYellow);
                    }
                    break;
            }

            // Change direction of assets
            Bitmap DirectionofCar = new Bitmap(Car);
            switch (direction)
            {
                case 1:
                    DirectionofCar.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    DirectionofCar.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    DirectionofCar.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return DirectionofCar;
        }



        private static int GetXOffset(bool left, int comp, float dist)
        {
            float distance = dist;
            if (dist > 239)
                distance = 239f;
            if (left)
            {
                switch (comp)
                {
                    case 0:
                        return 68;
                    case 1:
                        return (int)distance;
                    case 2:
                        return 140;
                    case 3:
                        return (int)Math.Abs(distance - 239);
                }
            }
            else
            {
                switch (comp)
                {
                    case 0:
                        return 145;
                    case 1:
                        return (int)distance;
                    case 2:
                        return 68;
                    case 3:
                        return (int)Math.Abs(distance - 239);
                }
            }
            return 0;
        }

        private static int GetYOffset(bool left, int comp, float dist)
        {
            float distance = dist;
            if (dist > 239)
                distance = 239f;
            if (left)
            {
                switch (comp)
                {
                    case 0:
                        return (int)Math.Abs(distance - 239);
                    case 1:
                        return 70;
                    case 2:
                        return (int)distance;
                    case 3:
                        return 140;
                }
            }
            else
            {
                switch (comp)
                {
                    case 0:
                        return (int)Math.Abs(distance - 239);
                    case 1:
                        return 140;
                    case 2:
                        return (int)distance;
                    case 3:
                        return 60;
                }
            }
            return 0;
        }

        private static Bitmap DrawParticipants(Bitmap trackBitmap)
        {
            int maxX = assets.Max(_square => _square.X);
            int maxY = assets.Max(_square => _square.Y);
            Bitmap driverBitmap = new Bitmap(trackBitmap);
            Graphics graphics = Graphics.FromImage(driverBitmap);
            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    SetIMG square = AddGridSquare(x, y);
                    if (square?.SectionData.Left == null && square?.SectionData.Right == null) continue;
                    Bitmap car = null;
                    if (square.SectionData.Left != null)
                    {
                        IParticipant participant = square.SectionData.Left;
                        car = GetParticipantImage(participant, square.Direction);
                        graphics.DrawImage(car, x * 239 + GetXOffset(true, square.Direction, square.SectionData.IntervalLeft), y * 239 + GetYOffset(true, square.Direction, square.SectionData.IntervalLeft));
                    }
                    if (square.SectionData.Right != null)
                    {
                        IParticipant participant = square.SectionData.Right;
                        car = GetParticipantImage(participant, square.Direction);
                        graphics.DrawImage(car, x * 239 + GetXOffset(false, square.Direction, square.SectionData.IntervalRight), y * 239 + GetYOffset(false, square.Direction, square.SectionData.IntervalRight));
                    }

                }
            }
            return driverBitmap;
        }

        private static Bitmap DrawBaseTrack(Track track)
        {
            if (Display.track == track)
            {
                return bitmap;
            }
            Display.track = track;
            CalculateGrid(track.Sections);
            MoveGrid(Math.Abs(SetIMG.MinX), Math.Abs(SetIMG.MinY));
            assets = assets.OrderBy(_square => _square.Y).ToList();
            int maxX = assets.Max(_square => _square.X);
            int maxY = assets.Max(_square => _square.Y);
            Bitmap background = SetAssets.CreateEmptyBitmap(239 * maxX + 239, 239 * maxY + 239);
            Bitmap empty = SetAssets.CreateEmptyBitmap(239, 239);
            Graphics graphics = Graphics.FromImage(background);
            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    SetIMG square = AddGridSquare(x, y);
                    Bitmap currTile = null;
                    if (square == null)
                    {
                        currTile = empty;
                    }
                    else
                    {
                        currTile = new Bitmap(SetAssets.LoadImg(square.ImagePath));
                        switch (square.Direction)
                        {
                            case 0:
                                if (square.Turn)
                                    currTile.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                break;
                            case 1:
                                if (square.Turn)
                                    currTile.RotateFlip(RotateFlipType.Rotate90FlipY);
                                else
                                    currTile.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                break;
                            case 2:
                                if (square.Turn)
                                    currTile.RotateFlip(RotateFlipType.Rotate180FlipX);
                                else
                                    currTile.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                break;
                            case 3:
                                if (square.Turn)
                                    currTile.RotateFlip(RotateFlipType.Rotate270FlipY);
                                else
                                    currTile.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                break;
                        }
                    }
                    graphics.DrawImage(currTile, x * 239, y * 239);
                }

            }
            bitmap = background;
            return background;
        }

        private static void CalculateGrid(LinkedList<Section> sections)
        {
            Race race = Data.CurrentRace;
            int align = direction;
            int x = 0;
            int y = 0;
            assets?.Clear();

            SetIMG.MinX = 0;
            SetIMG.MinY = 0;

            foreach (Section section in sections)
            {
                SectionTypes type = section.SectionType;
                SectionData data = race.GetSectionData(section);
                switch (type)
                {
                    case SectionTypes.StartGrid:
                        assets.Add(new SetIMG(x, y, _start, data, align));
                        break;
                    case SectionTypes.Straight:
                        assets.Add(new SetIMG(x, y, _straight, data, align));
                        break;
                    case SectionTypes.LeftCorner:
                        assets.Add(new SetIMG(x, y, _corner, data, align, true));
                        align = (align - 1) % 4;
                        if (align < 0)
                            align = 3;
                        break;
                    case SectionTypes.RightCorner:
                        assets.Add(new SetIMG(x, y, _corner, data, align));
                        align = (align + 1) % 4;
                        break;
                    case SectionTypes.Finish:
                        assets.Add(new SetIMG(x, y, _finish, data, align));
                        break;
                }
                if (align == 0)
                {
                    y--;
                }
                else if (align == 1)
                {
                    x++;
                }
                else if (align == 2)
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
            foreach (SetIMG square in assets)
            {
                square.X += x;
                square.Y += y;
            }
        }
    }
}
