using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public interface IParticipant
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public TeamColours TeamColour { get; set; }
        public IEquipment Equipment { get; set; }

    }
}
