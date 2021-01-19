using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class RaceData<T> where T : IParticipantData
    {

        private List<IParticipantData> ContestorDataList;

        public RaceData()
        {
            ContestorDataList = new List<IParticipantData>();
        }

        public string GetLeadingContestor()
        {
            if (ContestorDataList.Count == 0)
            {
                return "";
            }
            return ContestorDataList[0].GetLeadingContestor(ContestorDataList);
        }

        public int getSizeList()
        {
            return ContestorDataList.Count;
        }

        public void ClearList()
        {
            ContestorDataList.Clear();
        }

        public IParticipantData GetContestorData(IParticipant participant)
        {
            return ContestorDataList.FirstOrDefault(data => data.Contestor.Name == participant.Name);
        }

        public void AssignToList(T item)
        {
            item.Assign(ContestorDataList);
        }
    }
}
