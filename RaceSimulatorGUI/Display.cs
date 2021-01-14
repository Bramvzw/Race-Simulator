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

        #region imagePaths
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


        public static List<SetIMG> GetGridSquares()
        {
            return assets;
        }

        public static BitmapSource DrawTrack(Track track)
        {
            Bitmap trackBitmap = DrawBaseTrack(track);
            Bitmap driverBitmap = DrawParticipants(trackBitmap);
            return SetAssets.CreateBitmapSourceFromGdiBitmap(driverBitmap);
        }

        private static SetIMG GetGridSquare(int x, int y)
        {
            SetIMG square = assets.Find(_square => _square.X == x && _square.Y == y);
            return square;
        }


        private static Bitmap GetParticipantImage(IParticipant participant, int compass)
        {
            Bitmap car = null;
            switch (participant.TeamColour)
            {
                case TeamColours.Blue:
                    if (participant.Equipment.IsBroken)
                        car = SetAssets.LoadImg(_carBlueBroken);
                    else
                        car = SetAssets.LoadImg(_carBlue);
                    break;
                case TeamColours.Green:
                    if (participant.Equipment.IsBroken)
                        car = SetAssets.LoadImg(_carGreenBroken);
                    else
                        car = SetAssets.LoadImg(_carGreen);
                    break;
                case TeamColours.Orange:
                    if (participant.Equipment.IsBroken)
                        car = SetAssets.LoadImg(_carGreyBroken);
                    else
                        car = SetAssets.LoadImg(_carGrey);
                    break;
                case TeamColours.Red:
                    if (participant.Equipment.IsBroken)
                        car = SetAssets.LoadImg(_carRedBroken);
                    else
                        car = SetAssets.LoadImg(_carRed);
                    break;
                case TeamColours.Yellow:
                    if (participant.Equipment.IsBroken)
                        car = SetAssets.LoadImg(_carYellowBroken);
                    else
                        car = SetAssets.LoadImg(_carYellow);
                    break;
            }
            Bitmap carCorrectOrientation = new Bitmap(car);
            switch (compass)
            {
                case 1:
                    carCorrectOrientation.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    carCorrectOrientation.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    carCorrectOrientation.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return carCorrectOrientation;
        }


        //private static void MoveParticipantsAtStart()
        //{
        //    Race race = Data.CurrentRace;
        //    foreach (var section in race.Track.Sections.Where(section => section.SectionType == SectionTypes.StartGrid))
        //    {
        //        SectionData data = race.GetSectionData(section);
        //        switch (direction)
        //        {
        //            case 0:
        //                data.DistanceLeft = 60;
        //                data.DistanceRight = 170;
        //                break;
        //            case 1:
        //                data.DistanceLeft = 140;
        //                data.DistanceRight = 30;
        //                break;
        //            case 2:
        //                data.DistanceLeft = 140;
        //                data.DistanceRight = 15;
        //                break;
        //            case 3:
        //                data.DistanceLeft = 55;
        //                data.DistanceRight = 170;
        //                break;
        //        }
        //    }
        //}

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
                    SetIMG square = GetGridSquare(x, y);
                    if (square?.SectionData.Left == null && square?.SectionData.Right == null) continue;
                    Bitmap car = null;
                    if (square.SectionData.Left != null)
                    {
                        IParticipant participant = square.SectionData.Left;
                        car = GetParticipantImage(participant, square.Compass);
                        graphics.DrawImage(car, x * 239 + GetXOffset(true, square.Compass, square.SectionData.DistanceLeft), y * 239 + GetYOffset(true, square.Compass, square.SectionData.DistanceLeft));
                    }
                    if (square.SectionData.Right != null)
                    {
                        IParticipant participant = square.SectionData.Right;
                        car = GetParticipantImage(participant, square.Compass);
                        graphics.DrawImage(car, x * 239 + GetXOffset(false, square.Compass, square.SectionData.DistanceRight), y * 239 + GetYOffset(false, square.Compass, square.SectionData.DistanceRight));
                    }

                }
            }
            return driverBitmap;
        }

        private static Bitmap DrawBaseTrack(Track track)
        {
            if (Display.track == track) return bitmap;
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
                    SetIMG square = GetGridSquare(x, y);
                    Bitmap currTile = null;
                    if (square == null)
                    {
                        currTile = empty;
                    }
                    else
                    {
                        currTile = new Bitmap(SetAssets.LoadImg(square.ImagePath));
                        switch (square.Compass)
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
            int comp = direction;
            int x = 0, y = 0;
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
                        assets.Add(new SetIMG(x, y, _start, data, comp));
                        break;
                    case SectionTypes.Straight:
                        assets.Add(new SetIMG(x, y, _straight, data, comp));
                        break;
                    case SectionTypes.LeftCorner:
                        assets.Add(new SetIMG(x, y, _corner, data, comp, true));
                        comp = (comp - 1) % 4;
                        if (comp < 0)
                            comp = 3;
                        break;
                    case SectionTypes.RightCorner:
                        assets.Add(new SetIMG(x, y, _corner, data, comp));
                        comp = (comp + 1) % 4;
                        break;
                    case SectionTypes.Finish:
                        assets.Add(new SetIMG(x, y, _finish, data, comp));
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
            foreach (SetIMG square in assets)
            {
                square.X += x;
                square.Y += y;
            }
        }
    }
}
