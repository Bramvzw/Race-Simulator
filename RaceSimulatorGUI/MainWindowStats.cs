using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;

namespace RaceSimulatorGUI
{
    public class MainWindowStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowStats()
        {
            Race.RaceStarted += OnRaceStarted;
            if (Data.CurrentRace != null)
            {
                Data.CurrentRace.DriversChanged += OnDriversChanged;
            }
        }

        public void OnDriversChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public void OnRaceStarted(object sender, EventArgs e)
        {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.DriversChanged += OnDriversChanged;
        }
    }
}
