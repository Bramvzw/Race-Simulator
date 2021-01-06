using System;
using System.Collections.Generic;
using System.Text;

namespace Controller {
	public class RaceStartedEventArgs : EventArgs{
		public Race Race { get; set; }
	}
}
