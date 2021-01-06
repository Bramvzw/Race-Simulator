using Model;
using NUnit.Framework;
using RaceSimulator;

namespace ControllerTest {
	
	[TestFixture]
	public class RaceSimulatorConsole_GridSquare {

		private string[] s = new[] {"", "", "", ""};
		private SectionData sectionData;
		private GridSquare square;

		[SetUp]
		public void Setup() {
			sectionData = new SectionData();
			square = new GridSquare(-4, -3, s, sectionData);
		}

		[Test]
		public void LowestCoordinatesTest() {
			int expectedX = -4;
			int expectedY = -3;
			Assert.AreEqual(expectedX, GridSquare.LowestX);
			Assert.AreEqual(expectedY, GridSquare.LowestY);
		}
	}
}