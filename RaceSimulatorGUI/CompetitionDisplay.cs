using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Controller.Data;
using static Controller.Race;

namespace RaceSimulatorGUI
{
    public class CompetitionDisplay : INotifyPropertyChanged
    {
        public CompetitionDisplay()
        {
            ParticipantPoints = new List<DisplayPoints>();
            DetermineRanking();
            RaceStarted += StartedRace;
            if (CurrentRace != null)
            {
                CurrentRace.RaceFinished += FinishedRace;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public List<DisplayPoints> ParticipantPoints { get; set; }

        // In use when a race is finished
        public void FinishedRace(object sender, EventArgs e)
        {
            DetermineRanking();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        // When a update is needed during a race
        public void UpdateList()
        {
            DetermineRanking();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        // In use when a race is started
        public void StartedRace(object sender, EventArgs e)
        {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.RaceFinished += FinishedRace;
        }
        // 
        private void DetermineRanking()
        {
            ParticipantPoints.Clear();
            Competition.Contestors.ForEach(participant => ParticipantPoints.Add(new DisplayPoints(participant, ParticipantPoints.Count + 1)));
            ParticipantPoints = ParticipantPoints.OrderByDescending(p => p.points).ToList();
            for (int i = 1; i <= ParticipantPoints.Count; i++)
            {
                ParticipantPoints[i - 1].Position = i;
            }
        }
    }
}
