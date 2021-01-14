using System.Collections.Generic;
using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class Controller_Data_ParticipantsOrderFinish
    {

        private Competition _competition;

        [SetUp]
        public void Setup()
        {
            _competition = new Competition();
            Data.Initialise(_competition);
        }

        [Test]
        public void PutParticipantsInOrderOfFinish_NoCurrentRace_ListStaysSame()
        {
            Data.CurrentRace = null;
            List<IParticipant> before = new List<IParticipant>(_competition.Contestors);
            Data.PutParticipantsInOrderOfFinish();
            List<IParticipant> after = new List<IParticipant>(_competition.Contestors);
            CollectionAssert.AreEqual(before, after);
        }

        [Test]
        public void PutParticipantsInOrderOfFinish_CurrentRace_DifferentOrder()
        {
            List<IParticipant> before = new List<IParticipant>(_competition.Contestors);
            Data.NextRace();
            Dictionary<int, IParticipant> finalRanking = new Dictionary<int, IParticipant>();
            Data.CurrentRace.Contestors.Reverse();
            for (int i = 0; i < Data.CurrentRace.Contestors.Count; i++)
            {
                finalRanking.Add(i + 1, Data.CurrentRace.Contestors[i]);
            }
            Data.CurrentRace.SetFinalRanking(finalRanking);
            Data.PutParticipantsInOrderOfFinish();
            List<IParticipant> after = new List<IParticipant>(_competition.Contestors);
            CollectionAssert.AreNotEqual(before, after);
        }
    }
}