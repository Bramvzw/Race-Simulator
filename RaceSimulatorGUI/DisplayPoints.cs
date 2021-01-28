using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace RaceSimulatorGUI
{
    public class DisplayPoints : IParticipantDisplay
    {

        public IParticipant Contestor { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public Brush Brush { get; set; }
        public int points { get; set; }

        public DisplayPoints(IParticipant contestor, int position)
        {
            Contestor = contestor;
            Position = position;
            Name = contestor.Name;
            switch (contestor.TeamColour)
            {
                case TeamColours.Orange:
                    Brush = new SolidColorBrush(Color.FromRgb(245, 63, 39));
                    break;

                case TeamColours.Yellow:
                    Brush = new SolidColorBrush(Color.FromRgb(255, 255, 15));
                    break;
                case TeamColours.Blue:
                    Brush = new SolidColorBrush(Color.FromRgb(15, 255, 255));
                    break;
                case TeamColours.Green:
                    Brush = new SolidColorBrush(Color.FromRgb(15, 255, 15));
                    break;
                case TeamColours.Red:
                    Brush = new SolidColorBrush(Color.FromRgb(255, 15, 15));
                    break;
            }
            IParticipantData contestorData = Data.Competition.ContestorPoints.GetContestorData(contestor);
            if (contestorData == null)
                points = 0;
            else
                points = ((ParticipantPoints)contestorData).points;
            Brush.Freeze();
        }
    }
}
