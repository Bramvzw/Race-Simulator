using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Int32;

namespace Model
{
    public class ParticipantBreakDown : IParticipantData
    {

        public string Name { get; set; }
        public int Count { get; set; }
        public IParticipant Contestor { get; set; }

        // For testing purposes
        public void Assign(List<IParticipantData> participantData)
        {
            var participant = participantData.FirstOrDefault(data => data.Contestor.Name == this.Name);
            if (participant == null)
            {
                participantData.Add(this);
                return;
            }
            ParticipantBreakDown timesBrokenDown = (ParticipantBreakDown)participant;
            timesBrokenDown.Count += this.Count;
        }

        // Returns the contestor that has the most times brokendown
        public string GetLeadingContestor(List<IParticipantData> participantData)
        {
            List<ParticipantBreakDown> brokenDownParticipantData = new List<ParticipantBreakDown>();
            foreach (ParticipantBreakDown down in participantData) brokenDownParticipantData.Add(down);
            // Sets max value to 'maxPoints' times broken down will never be more than that :)
            int maxPoints = MaxValue;
            string bestParticipant = "";
            foreach (ParticipantBreakDown brokenDown in brokenDownParticipantData)
            {
                if (brokenDown.Count < maxPoints)
                {
                    maxPoints = brokenDown.Count;
                    bestParticipant = brokenDown.Name;
                }
            }
            return bestParticipant;
        }
    }
}