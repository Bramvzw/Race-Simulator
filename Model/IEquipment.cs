using System;

namespace Model
{
    public interface IEquipment
    {

        public int Performance { get; set; }
        public bool Broken { get; set; }
        public int Quality { get; set; }
        public int Speed { get; set; }


    }
}
