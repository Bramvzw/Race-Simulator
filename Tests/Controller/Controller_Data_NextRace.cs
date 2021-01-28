using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest
{

    [TestFixture]
    public class Controller_Data_NextRace
    {

        [SetUp]
        public void Setup()
        {
           
        }

        [Test]
        public void NextRace_NoNextTrack_CurrentRaceNull()
        {
            Data.Competition = new Competition();
            Data.NextRace();
            Race result = Data.CurrentRace;
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_OneTrack_NewRace()
        {
            Data.Competition = new Competition();
            Data.Competition.Tracks.Enqueue(new Track("Test track", new[] { SectionTypes.StartGrid, SectionTypes.Finish }));
            Data.NextRace();
            Race result = Data.CurrentRace;
            Assert.NotNull(result);
        }


    }
}