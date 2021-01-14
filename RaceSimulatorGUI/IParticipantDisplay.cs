using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace RaceSimulatorGUI
{
    public interface IParticipantDisplay
    {
        public IParticipant Contestor { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public Brush Brush { get; set; }

    }
}
