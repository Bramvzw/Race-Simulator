using System;

namespace Model
{
    public class Driver : IParticipant
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public IEquipment Equipment { get; set; }
        public TeamColours TeamColour { get; set; }

        public Driver(string name, IEquipment equipment)
        {
            Name = name;
            Equipment = equipment;
        }


        // for testing purposes
        public override bool Equals(object obj)
        {
            if (obj is Driver)
            {
                Driver d = (Driver)obj;
                return d.Name == Name;
            }
            return false;
        }

        public override string ToString()
        {
            return $"Bestuurder {Name}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}