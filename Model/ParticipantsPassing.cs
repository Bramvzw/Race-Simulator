using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class ParticipantsPassing : IParticipantData {

        public string PasserName { get; set; }
        public string PassedName { get; set; }
        public int Count { get; set; }
        public IParticipant Contestor { get; set; }

        public ParticipantsPassing(string PasserName, string PassedName) {
            this.PassedName = PassedName;
            this.PasserName = PasserName;
        }

        public void Assign(List<IParticipantData> participantData) {
            var overtakenData = participantData.Cast<ParticipantsPassing>();
            var participant = overtakenData.FirstOrDefault(data => data.PasserName == this.PasserName);
            if (participant == null) {
                this.Count++;
                participantData.Add(this);
            } else {
                participant.Count++;
            }
        }

        // Returns the name of the contestor that has the most pasted other contestors
        public string GetLeadingContestor(List<IParticipantData> participantData) {
            var overtakenData = participantData.Cast<ParticipantsPassing>();
            int maxOvertaken = Int32.MaxValue;
            string bestParticipant = "";
            foreach (ParticipantsPassing overtaken in overtakenData) {
                if(overtaken.Count < maxOvertaken) {
                    maxOvertaken = overtaken.Count;
                    bestParticipant = overtaken.PasserName;
                }
            }
            return bestParticipant;
        }
    }
}
