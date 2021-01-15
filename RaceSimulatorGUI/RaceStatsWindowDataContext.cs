using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RaceSimulatorGUI
{
    public class RaceStatsWindowDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string TrackName { get => $"{Data.CurrentRace.Track.Name}"; }

        public List<DisplayTime> LapTimeDisplay { get => DetermineLapTime(_lapTimeDisplay); set => _lapTimeDisplay = value; }

        private List<DisplayRanking> _rankingDisplay;
        public List<DisplayRanking> RankingDisplay { get => DetermineRanking(_rankingDisplay); set => _rankingDisplay = value; }
        private List<DisplayTime> _lapTimeDisplay;

        public string BestOvertaker { get => $"{Data.Competition.ContestorOvertaken.GetLeadingContestor()} heeft het vaakst ingehaald"; }

        public RaceStatsWindowDataContext()
        {
            RankingDisplay = new List<DisplayRanking>();
            LapTimeDisplay = new List<DisplayTime>();
            Race.RaceStarted += OnRaceStarted;
            if (Data.CurrentRace != null)
                Data.CurrentRace.DriversChanged += OnDriversChanged;
        }

        ~RaceStatsWindowDataContext()
        {
            Race.RaceStarted -= OnRaceStarted;
            Data.CurrentRace.DriversChanged -= OnDriversChanged;
        }

        public void OnDriversChanged(object sender, EventArgs e)
        {
            if (LapTimeDisplay != null)
            {
                DetermineRanking(RankingDisplay);
                DetermineLapTime(LapTimeDisplay);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            }
        }

        private List<DisplayRanking> DetermineRanking(List<DisplayRanking> display)
        {
            display.Clear();
            Data.Competition.Contestors.ForEach(participant => display.Add(new DisplayRanking(participant, Data.CurrentRace.GetRankingOfParticipant(participant), Data.CurrentRace.GetIsFinished(participant))));
            display = display.OrderBy(p => p.Position).ToList();
            for (int i = 1; i <= display.Count; i++)
                display[i - 1].Position = i;
            return display;
        }

        private List<DisplayTime> DetermineLapTime(List<DisplayTime> display)
        {
            display.Clear();
            Data.Competition.Contestors.ForEach(participant => display.Add(new DisplayTime(participant, display.Count + 1, Data.CurrentRace.GetIsFinished(participant))));
            display = display.OrderBy(p => p.TimeSpan).ToList();
            for (int i = 1; i <= display.Count; i++)
                display[i - 1].Position = i;
            return display;
        }

        public void OnRaceStarted(object sender, EventArgs e)
        {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.DriversChanged += OnDriversChanged;
        }
    }
}

