using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public interface IParticipantData
    {

        public IParticipant Contestor { get; set; }
        public string GetLeadingContestor(List<IParticipantData> participantData);

        public void Assign(List<IParticipantData> contestorData);

    }
}
