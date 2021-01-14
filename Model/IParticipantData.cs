using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public interface IParticipantData
    {

        public IParticipant Contestor { get; set; }

        public void Assign(List<IParticipantData> contestorData);

        public string GetLeadingContestor(List<IParticipantData> participantData);
    }
}
