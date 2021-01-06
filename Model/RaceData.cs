using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class RaceData<T> where T : IParticipantData
    {

        private List<IParticipantData> _list;

        public RaceData()
        {
            _list = new List<IParticipantData>();
        }

        public string GetLeadingContestor()
        {
            if (_list.Count == 0) return "";
            return _list[0].GetLeadingContestor(_list);
        }

        public IParticipantData GetContestorData(IParticipant participant)
        {
            return _list.FirstOrDefault(data => data.Contestor.Name == participant.Name);
        }

        public int CountList()
        {
            return _list.Count;
        }

        public void ClearList()
        {
            _list.Clear();
        }

        public void AddToList(T item)
        {
            item.Assign(_list);
        }
    }
}
