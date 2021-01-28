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
            ParticipantSectionTime participant = null;
            foreach (var data in sectionTimeData)
            {
                if (data.Contestor.Name == Name && data.Section == Section)
                {
                    participant = data;
                    break;
                }
            }

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
