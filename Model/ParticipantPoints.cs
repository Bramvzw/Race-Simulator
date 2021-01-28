using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Int32;

namespace Model
{
    public class ParticipantPoints : IParticipantData
    {
        public string Name { get; set; }
        public IParticipant Contestor { get; set; }
        public int points { get; set; }

        public void Assign(List<IParticipantData> ContestorData)
        {
            IEnumerable<ParticipantPoints> points;
            points = ContestorData.Cast<ParticipantPoints>();
            ParticipantPoints contestor = null;
            foreach (var data in points)
            {
                if (data.Name == this.Name)
                {
                    contestor = data;
                    break;
                }
            }

            if (contestor != null)
            {
                contestor.points += this.points;
            }
            else
            {
                ContestorData.Add(this);
            }
        }


        // Returns the contestor with the most points
        public string GetLeadingContestor(List<IParticipantData> participantData)
        {
            List<ParticipantPoints> ParticipantPointData = new List<ParticipantPoints>();
            foreach (ParticipantPoints points in participantData) ParticipantPointData.Add(points);
            int maxPoints = MinValue;
            string bestParticipant = "";
            foreach (ParticipantPoints participantPoints in ParticipantPointData)
            {
                if (participantPoints.points > maxPoints)
                {
                    maxPoints = participantPoints.points;
                    bestParticipant = participantPoints.Name;
                }
            }
            return bestParticipant;
        }
    }
}