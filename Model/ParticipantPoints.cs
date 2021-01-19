using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class ParticipantPoints : IParticipantData
    {

        public string Name { get; set; }
        public IParticipant Contestor { get; set; }
        public int Points { get; set; }


        public void Assign(List<IParticipantData> ContestorData)
        {
            IEnumerable<ParticipantPoints> points = ContestorData.Cast<ParticipantPoints>();
            ParticipantPoints contestor = points.FirstOrDefault(data => data.Name == this.Name);
            if (contestor != null)
            {
                contestor.Points += this.Points;
            }
            else
            {
                ContestorData.Add(this);
                return;
            }
        }


        // Returns the contestor with the most points
        public string GetLeadingContestor(List<IParticipantData> participantData)
        {
            List<ParticipantPoints> ParticipantPointData = participantData.Cast<ParticipantPoints>().ToList();
            int maxPoints = int.MinValue;
            string bestParticipant = "";
            foreach (ParticipantPoints participantPoints in ParticipantPointData)
            {
                if (participantPoints.Points > maxPoints)
                {
                    maxPoints = participantPoints.Points;
                    bestParticipant = participantPoints.Name;
                }
            }
            return bestParticipant;
        }
    }
}

/* Functies herschrijven */