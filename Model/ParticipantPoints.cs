using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class ParticipantPoints : IParticipantData
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public IParticipant Contestor { get; set; }

        public void Assign(List<IParticipantData> ContestorData)
        {
            var pointData = ContestorData.Cast<ParticipantPoints>();
            var participant = pointData.FirstOrDefault(data => data.Name == this.Name);
            if (participant == null)
            {
                ContestorData.Add(this);
                return;
            }
            participant.Points += this.Points;
        }


        // Returns the conterstor with the most points
        public string GetLeadingContestor(List<IParticipantData> participantData)
        {
            var ParticipantPointData = participantData.Cast<ParticipantPoints>().ToList();
            int maxPoints = Int32.MinValue;
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