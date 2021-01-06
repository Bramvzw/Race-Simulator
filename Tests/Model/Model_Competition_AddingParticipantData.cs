using System;
using System.Collections.Generic;
using Model;
using NUnit.Framework;

namespace ControllerTest {

	[TestFixture]
	public class Model_Competition_AddingParticipantData {

		private Competition _competition;

		[SetUp]
		public void Setup() {
			_competition = new Competition();
			_competition.Contestors.Add(new Driver("Tester1", new Car()));
			_competition.Contestors.Add(new Driver("Tester2", new Car()));
		}

		[Test]
		public void AddPoints() {
			Dictionary<int, IParticipant> participants =
				new Dictionary<int, IParticipant> {
					{1, _competition.Contestors[0]}, {2, _competition.Contestors[1]}
				};
			_competition.AssignPoints(participants);
			var expected = _competition.Contestors[0].Name;
			var actual = _competition.ContestorPoints.GetLeadingContestor();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddSectionTime() {
			Section s = new Section(SectionTypes.StartGrid);
			_competition.SetSectionTime(_competition.Contestors[0], TimeSpan.FromSeconds(450), s);
			_competition.SetSectionTime(_competition.Contestors[1], TimeSpan.FromSeconds(750), s);
			var expected = _competition.Contestors[0].Name;
			var actual = _competition.ContestorSectionTime.GetLeadingContestor();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddLapTime() {
			Track t = new Track("test", new[] { SectionTypes.StartGrid, SectionTypes.Finish });
			_competition.AddLapTime(_competition.Contestors[0], t, TimeSpan.FromSeconds(450));
			_competition.AddLapTime(_competition.Contestors[1], t, TimeSpan.FromSeconds(750));
			var expected = _competition.Contestors[0].Name;
			var actual = _competition.ContestorTime.GetLeadingContestor();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddOvertaken() {
			_competition.ContestorPassed(_competition.Contestors[0], _competition.Contestors[1]);
			_competition.ContestorPassed(_competition.Contestors[1], _competition.Contestors[0]);
			_competition.ContestorPassed(_competition.Contestors[0], _competition.Contestors[1]);
			var expected = _competition.Contestors[0].Name;
			var actual = _competition.ContestorOvertaken.GetLeadingContestor();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddBrokenDown() {
			_competition.ContestorBrokenCount(_competition.Contestors[0], 4);
			_competition.ContestorBrokenCount(_competition.Contestors[1], 7);
			var expected = _competition.Contestors[0].Name;
			var actual = _competition.ContestorCountBroken.GetLeadingContestor();
			Assert.AreEqual(expected, actual);
		}
	}
}