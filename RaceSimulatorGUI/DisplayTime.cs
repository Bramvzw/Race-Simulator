using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace RaceSimulatorGUI
{
    public class DisplayTime : IParticipantDisplay
    {

        public IParticipant Contestor { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public Brush Brush { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public bool RaceisFinished { get; set; }

        public DisplayTime(IParticipant contestor, int position, bool finished)
        {
            Contestor = contestor;
            Position = position;
            Name = contestor.Name;
            switch (contestor.TeamColour)
            {
                case TeamColours.Orange:
                    Brush = new SolidColorBrush(Color.FromRgb(245, 63, 39));
                    break;
                case TeamColours.Red:
                    Brush = new SolidColorBrush(Color.FromRgb(255, 15, 15));
                    break;
                case TeamColours.Green:
                    Brush = new SolidColorBrush(Color.FromRgb(15, 255, 15));
                    break;
                case TeamColours.Yellow:
                    Brush = new SolidColorBrush(Color.FromRgb(255, 255, 15));
                    break;

                case TeamColours.Blue:
                    Brush = new SolidColorBrush(Color.FromRgb(15, 255, 255));
                    break;
            }
            IParticipantData data = Data.Competition.ContestorTime.GetContestorData(contestor);
            TimeSpan = ((ParticipantTime) data)?.Time ?? TimeSpan.Zero;
            RaceisFinished = finished;
            Brush.Freeze();
        }
    }
}
