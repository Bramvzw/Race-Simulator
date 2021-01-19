using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class SectionData
    {
        public IParticipant Left { get; set; }
        public int IntervalLeft { get; set; }
        public IParticipant Right { get; set; }
        public int IntervalRight { get; set; }
    }
}
