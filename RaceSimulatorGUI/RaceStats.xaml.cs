using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RaceSimulatorGUI {
    /// <summary>
    /// Interaction logic for RaceStats.xaml
    /// </summary>
    public partial class RaceStats : Window {

        private RaceStatsWindowDataContext _dataContext;
        public RaceStats() {
            _dataContext = new RaceStatsWindowDataContext();
            InitializeComponent();
            Closed += RaceStats_Closed;
        }

        private void RaceStats_Closed(object sender, EventArgs e) {
            _dataContext = null;
        }
    }
}
