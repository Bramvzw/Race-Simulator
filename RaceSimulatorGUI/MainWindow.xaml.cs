using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RaceSimulatorGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private RaceStats _raceStats;
        private CompetitionStats _competitionStats;
        private MainWindowStats _dataContext;

        public MainWindow()
        {
            Data.Initialise(new Competition());
            Race.RaceStarted += OnRaceStarted;
            Data.NextRace();
            Display.Initialise();

            Data.CurrentRace.DriversChanged += OnDriversChanged;
            _dataContext = new MainWindowStats();

            InitializeComponent();
        }

        public void OnDriversChanged(object sender, EventArgs e)
        {
            DriversChangedEventArgs e1 = (DriversChangedEventArgs)e;
            this.TrackImage.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                this.TrackImage.Source = null;
                this.TrackImage.Source = Display.DrawTrack(e1.Track);
            }));
        }

        public void OnRaceStarted(object sender, EventArgs e)
        {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.DriversChanged += OnDriversChanged;
            SetAssets.ClearCache();
            this.TrackImage?.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                this.TrackImage.Source = null;
                this.TrackImage.Source = Display.DrawTrack(e1.Race.Track);
            }));
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_ShowCompetitionStats_Click(object sender, RoutedEventArgs e)
        {
            _competitionStats = new CompetitionStats();
            _competitionStats.Show();
        }

        private void MenuItem_ShowRaceStats_Click(object sender, RoutedEventArgs e)
        {
            _raceStats = new RaceStats();
            _raceStats.Show();
        }
    }
}
