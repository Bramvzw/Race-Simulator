using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class ParticipantBreakDown : IParticipantData {

        public string Name { get; set; }
        public int Count { get; set; }
        public IParticipant Contestor { get; set; }


        public void Assign(List<IParticipantData> participantData) {
            //var participant = participantData.FirstOrDefault(data => data.Contestor.Name == this.Name);
            //if(participant == null) {
            //    participantData.Add(this);
            //    return;
            //}
            //ParticipantBreakDown timesBrokenDown = (ParticipantBreakDown)participant;
            //timesBrokenDown.Count += this.Count;
        }

        // Returns the contestor that has the most times brokendown
        public string GetLeadingContestor(List<IParticipantData> participantData) {
            var BrokenDownParticipantData = participantData.Cast<ParticipantBreakDown>().ToList();
            // Sets max value to 'maxPoints' times brokendown will never be more than that :)
            int maxPoints = Int32.MaxValue;
            string bestParticipant = "";
            foreach (ParticipantBreakDown brokenDown in BrokenDownParticipantData) {
                if (brokenDown.Count < maxPoints) {
                    maxPoints = brokenDown.Count;
                    bestParticipant = brokenDown.Name;
                }
            }
            return bestParticipant;
        }
    }
}