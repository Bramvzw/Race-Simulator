using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Int32;

namespace Model
{
    public class ParticipantsPassing : IParticipantData
    {
        public string PassedName { get; set; }
        public int Count { get; set; }
        public string PasserName { get; set; }
        public IParticipant Contestor { get; set; }

        public ParticipantsPassing(string PasserName, string PassedName)
        {
            this.PassedName = PassedName;
            this.PasserName = PasserName;
        }

        // For testing purposes
        public void Assign(List<IParticipantData> participantData)
        {
            IEnumerable<ParticipantsPassing> overtakenData = participantData.Cast<ParticipantsPassing>();
            ParticipantsPassing participant = null;
            foreach (var data in overtakenData)
            {
                if (data.PasserName == this.PasserName)
                {
                    participant = data;
                    break;
                }
            }
            if (participant != null)
            {
                participant.Count++;
            }
            else
            {
                this.Count++;
                participantData.Add(this);
            }
        }

        // Returns the name of the contestor that has the most passed other contestors
        public string GetLeadingContestor(List<IParticipantData> participantData)
        {
            IEnumerable<ParticipantsPassing> overtakenData = participantData.Cast<ParticipantsPassing>();
            int maxOvertaken = MaxValue;
            string bestParticipant = "";
            foreach (ParticipantsPassing overtaken in overtakenData)
            {
                if (overtaken.Count < maxOvertaken)
                {
                    maxOvertaken = overtaken.Count;
                    bestParticipant = overtaken.PasserName;
                }
            }
            return bestParticipant;
        }
    }
}
