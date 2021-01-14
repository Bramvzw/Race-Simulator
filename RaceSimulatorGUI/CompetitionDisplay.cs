using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RaceSimulatorGUI
{
    public class CompetitionDisplay : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public List<DisplayPoints> ParticipantPoints { get; set; }

        public void OnRaceFinished(object sender, EventArgs e)
        {
            DetermineRanking();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public void UpdateList()
        {
            DetermineRanking();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
        public CompetitionDisplay()
        {
            ParticipantPoints = new List<DisplayPoints>();
            DetermineRanking();
            Race.RaceStarted += OnRaceStarted;
            if (Data.CurrentRace != null)
                Data.CurrentRace.RaceFinished += OnRaceFinished;
        }

        public void OnRaceStarted(object sender, EventArgs e)
        {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.RaceFinished += OnRaceFinished;
        }

        private void DetermineRanking()
        {
            ParticipantPoints.Clear();
            Data.Competition.Contestors.ForEach(participant => ParticipantPoints.Add(new DisplayPoints(participant, ParticipantPoints.Count + 1)));
            ParticipantPoints = ParticipantPoints.OrderByDescending(p => p.Points).ToList();
            for (int i = 1; i <= ParticipantPoints.Count; i++)
                ParticipantPoints[i - 1].Position = i;
        }
    }
}
