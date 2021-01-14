using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest
{

    [TestFixture]
    public class Controller_Race_HelperMethods
    {

        private Race race;
        private Track track;
        private List<IParticipant> participants;
        private Competition _competition;

        [SetUp]
        public void Setup()
        {
            track = new Track("Baby park", new[] { SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner });
            participants = new List<IParticipant>() { new Driver("Bob", new Car()), new Driver("Terry", new Car()) };
            race = new Race(track, participants);
            _competition = new Competition();
            Data.Competition = _competition;
        }

        [Test]
        public void AllPlayersFinished_PlayersNotFinished()
        {
            bool actual = race.AllPlayersFinished();
            Assert.IsFalse(actual);
        }

        [Test]
        public void AllPlayersFinished_PlayersFinished()
        {
            race.Contestors.ForEach(_participant => _participant.Points = 3);
            bool actual = race.AllPlayersFinished();
            Assert.IsTrue(actual);
        }

        [Test]
        public void GetSectionDataByParticipant_ParticipantNotOnTrack_ReturnNull()
        {
            participants.Add(new Driver("Unknown", new Car()));
            SectionData sectionData = race.GetSectionDataByParticipant(participants[2]);
            Assert.IsNull(sectionData);
        }

        [Test]
        public void GetSectionDataByParticipant_ParticipantOnTrack_ReturnData()
        {
            SectionData expected = race.GetSectionData(track.Sections?.First?.Value);
            SectionData actual = race.GetSectionDataByParticipant(participants[0]);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetSectionData_KnownKey_ReturnData()
        {
            SectionData result = race.GetSectionData(track.Sections.First?.Value);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetSectionData_NewKey_ReturnData()
        {
            Section newSection = new Section(SectionTypes.Straight);
            SectionData result = race.GetSectionData(newSection);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetSectionBySectionData_UnknownData_ReturnNull()
        {
            SectionData data = new SectionData();
            Section result = race.GetSectionBySectionData(data);
            Assert.IsNull(result);
        }

        [Test]
        public void GetSectionBySectionData_KnownData_ReturnSection()
        {
            Section expected = track.Sections.First?.Value;
            SectionData data = race.GetSectionData(expected);
            Section actual = race.GetSectionBySectionData(data);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MoveParticipant_NextSection_ReturnTrue()
        {
            bool expected = true;
            bool actual = race.MoveParticipants(participants[0], race.GetSectionDataByParticipant(participants[0]), 300,
                true, false, track.Sections.First?.Value, DateTime.Now);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MoveParticipant_SameSection_ReturnFalse()
        {
            bool expected = false;
            bool actual = race.MoveParticipants(participants[0], race.GetSectionDataByParticipant(participants[0]), 100,
                true, false, track.Sections.First?.Value, DateTime.Now);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DetermineRanking_ProperOrder()
        {
            race.DetermineRanking();
            var rankingBefore = new Dictionary<int, IParticipant>(race.GetRanking());
            SectionData data = race.GetSectionDataByParticipant(participants[0]);
            data.Right = null;
            data.Left = null;
            data = race.GetSectionData(
                track.Sections.FirstOrDefault(_section => _section.SectionType == SectionTypes.RightCorner));
            data.Left = participants[0];
            SectionData data1 = race.GetSectionData(track.Sections.Last?.Value);
            data1.Left = participants[1];
            race.DetermineRanking();
            var rankingAfter = new Dictionary<int, IParticipant>(race.GetRanking());
            foreach (KeyValuePair<int, IParticipant> pair in rankingBefore)
            {
                Console.WriteLine($"[{pair.Key}] {pair.Value.Name}");
            }
            foreach (KeyValuePair<int, IParticipant> pair in rankingAfter)
            {
                Console.WriteLine($"[{pair.Key}] {pair.Value.Name}");
            }
            CollectionAssert.AreNotEqual(rankingBefore, rankingAfter);
        }

        [Test]
        public void MoveParticipants_NoErros()
        {
            race.ContestorData(DateTime.Now);
            Assert.Pass();
        }
    }
}