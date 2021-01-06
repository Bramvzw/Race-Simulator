using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest {
	
	[TestFixture]
	public class Controller_Data_InitialisationTest {
		
		[SetUp]
		public void Setup() {
			
		}

		[Test]
		public void Initialise_CompetitionSet() {
			Competition expected = new Competition();
			Data.Initialise(expected);
			Competition result = Data.Competition;
			Assert.AreSame(expected, result);
		}
		
		[Test]
		public void Initialise_Competition_ParticipantsAdded() {
			Competition competition = new Competition();
			Data.Initialise(competition);
			Assert.AreEqual(5, Data.Competition.Contestors.Count);
		}

		[Test]
		public void Initialise_Competition_TracksAdded() {
			Competition competition = new Competition();
			Data.Initialise(competition);
			Assert.AreEqual(4, Data.Competition.Tracks.Count);
		}
	}
}