using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class ParticipantTime : IParticipantData
    {
        public IParticipant Contestor { get; set; }
        public string Name { get; set; }
        public TimeSpan Time { get; set; }
        public Track Track { get; set; }

        public virtual void Assign(List<IParticipantData> participantData)
        {
            var timeData = participantData.Cast<ParticipantTime>();
            var participant = timeData.FirstOrDefault(data => data.Name == Name && data.Track?.Name == Track?.Name);
            if (participant == null)
            {
                participantData.Add(this);
                return;
            }
            if (Time < participant.Time)
                participant.Time = Time;
        }

        // Returns the contestor with the lowest time
        public string GetLeadingContestor(List<IParticipantData> participantData)
        {
            var timeData = participantData.Cast<ParticipantTime>();
            TimeSpan maxTime = TimeSpan.MaxValue;
            string bestParticipant = "";
            foreach (ParticipantTime time in timeData)
            {
                if (time.Track?.Name != Track?.Name) continue;
                // When the difference between t1 and t2 is lower then zero maxtime is new t1 and best is new p1
                if (TimeSpan.Compare(time.Time, maxTime) < 0)
                {
                    maxTime = time.Time;
                    bestParticipant = time.Name;
                }
            }
            return bestParticipant;
        }
    }
}