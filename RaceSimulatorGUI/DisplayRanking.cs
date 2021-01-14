using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace RaceSimulatorGUI
{
    public class DisplayRanking : IParticipantDisplay
    {

        public IParticipant Contestor { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public Brush Brush { get; set; }
        public bool Finished { get; set; }

        public DisplayRanking(IParticipant participant, int place, bool finished)
        {
            Contestor = participant;
            Position = place;
            Name = participant.Name;
            switch (participant.TeamColour)
            {
                case TeamColours.Orange:
                    Brush = new SolidColorBrush(Color.FromRgb(245, 63, 39));
                    break;
                case TeamColours.Red:
                    Brush = new SolidColorBrush(Color.FromRgb(255, 15, 15));
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
            }
            Finished = finished;
            Brush.Freeze();
        }
    }
}
