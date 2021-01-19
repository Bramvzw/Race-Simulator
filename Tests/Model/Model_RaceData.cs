using System;
using System.Collections.Generic;
using Model;
using NUnit.Framework;

namespace ControllerTest
{

    [TestFixture]
    public class Model_RaceData
    {

        private RaceData<ParticipantPoints> _participantPoints;
        private RaceData<ParticipantTime> _participantTime;
        private RaceData<ParticipantsPassing> _participantsOvertaken;
        private RaceData<ParticipantBreakDown> _particpantTimesBrokenDown;
        private RaceData<ParticipantSectionTime> _participantSectionTime;
        private List<IParticipant> _participants;

        [SetUp]
        public void Setup()
        {
            _participantPoints = new RaceData<ParticipantPoints>();
            _participantTime = new RaceData<ParticipantTime>();
            _participantsOvertaken = new RaceData<ParticipantsPassing>();
            _particpantTimesBrokenDown = new RaceData<ParticipantBreakDown>();
            _participantSectionTime = new RaceData<ParticipantSectionTime>();
            _participants = new List<IParticipant> { new Driver("Test1", new Car()), new Driver("Test2", new Car()) };
        }

        [Test]
        public void AddItem_ListShouldContainThatItem()
        {
            var points = new ParticipantPoints() { Name = "Test1", Points = 5, Contestor = _participants[0] };
            var time = new ParticipantTime() { Name = "Test1", Time = DateTime.Now.TimeOfDay, Contestor = _participants[0] };
            var sectionTime = new ParticipantSectionTime()
            {
                Name = "Test1",
                Section = new Section(SectionTypes.StartGrid),
                Contestor = _participants[0],
                Time = DateTime.Now.TimeOfDay
            };
            var overtaken = new ParticipantsPassing("Test1", "Test2") { Contestor = _participants[0] };
            var brokenDown = new ParticipantBreakDown() { Name = "Test1", Count = 1, Contestor = _participants[0] };
            _participantPoints.AssignToList(points);
            _participantTime.AssignToList(time);
            _participantsOvertaken.AssignToList(overtaken);
            _particpantTimesBrokenDown.AssignToList(brokenDown);
            _participantSectionTime.AssignToList(sectionTime);
            int expected = 1;
            int pointsActual = _participantPoints.getSizeList();
            int timeActual = _participantTime.getSizeList();
            int overtakenActual = _participantsOvertaken.getSizeList();
            int brokenDownActual = _particpantTimesBrokenDown.getSizeList();
            int sectionTimeActual = _participantSectionTime.getSizeList();
            Assert.AreEqual(expected, pointsActual);
            Assert.AreEqual(expected, timeActual);
            Assert.AreEqual(expected, overtakenActual);
            Assert.AreEqual(expected, brokenDownActual);
            Assert.AreEqual(expected, sectionTimeActual);
        }

        [Test]
        public void BestParticipant_Points()
        {
            var expected = new ParticipantPoints() { Name = "Test1", Points = 5, Contestor = _participants[0] };
            var test = new ParticipantPoints() { Name = "Test2", Points = 3, Contestor = _participants[1] };
            _participantPoints.AssignToList(expected);
            _participantPoints.AssignToList(test);
            var actual = _participantPoints.GetLeadingContestor();
            Assert.AreEqual(expected.Name, actual);
        }

        [Test]
        public void BestParticipant_Overtaken()
        {
            _participantsOvertaken.AssignToList(new ParticipantsPassing("Test1", "Test2") { Contestor = _participants[0] });
            _participantsOvertaken.AssignToList(new ParticipantsPassing("Test2", "Test1") { Contestor = _participants[1] });
            var expected = _participants[0].Name;
            var actual = _participantsOvertaken.GetLeadingContestor();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BestParticipant_Time()
        {
            _participantTime.AssignToList(new ParticipantTime() { Name = "Test1", Time = new TimeSpan(0, 5, 30), Contestor = _participants[0] });
            _participantTime.AssignToList(new ParticipantTime() { Name = "Test2", Time = new TimeSpan(0, 7, 45), Contestor = _participants[1] });
            var expected = _participants[0].Name;
            var actual = _participantTime.GetLeadingContestor();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BestParticipant_BrokenDown()
        {
            _particpantTimesBrokenDown.AssignToList(new ParticipantBreakDown() { Name = "Test1", Count = 1, Contestor = _participants[0] });
            _particpantTimesBrokenDown.AssignToList(new ParticipantBreakDown() { Name = "Test2", Count = 2, Contestor = _participants[1] });
            _particpantTimesBrokenDown.AssignToList(new ParticipantBreakDown() { Name = "Test2", Count = 1, Contestor = _participants[1] });
            var expected = _participants[0].Name;
            var actual = _particpantTimesBrokenDown.GetLeadingContestor();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetParticipantData()
        {
            _particpantTimesBrokenDown.AssignToList(new ParticipantBreakDown() { Name = "Test1", Count = 1, Contestor = _participants[0] });
            _particpantTimesBrokenDown.AssignToList(new ParticipantBreakDown() { Name = "Test2", Count = 2, Contestor = _participants[1] });
            _particpantTimesBrokenDown.AssignToList(new ParticipantBreakDown() { Name = "Test2", Count = 1, Contestor = _participants[1] });
            var expected = _participants[0].Name;
            var actual = _particpantTimesBrokenDown.GetContestorData(_participants[0]).Contestor.Name;
            Assert.AreEqual(expected, actual);
        }
    }
}