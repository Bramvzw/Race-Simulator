using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model
{
    public class ParticipantSectionTime : ParticipantTime
    {

        public Section Section { get; set; }

        public override void Assign(List<IParticipantData> participantData)
        {
            IEnumerable<ParticipantSectionTime> sectionTimeData = participantData.Cast<ParticipantSectionTime>();
            ParticipantSectionTime participant = sectionTimeData.FirstOrDefault(data => data.Contestor.Name == Name && data.Section == Section);
            if (participant != null)
            {
                participant.Time = Time;
            }
            else
            {
                participantData.Add(this);
                return;
            }
        }
    }
}
