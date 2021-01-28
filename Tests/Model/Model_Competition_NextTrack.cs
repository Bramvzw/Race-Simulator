using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Competition_NextTrack
    {
        private Competition _competition;

        [SetUp]
        public void SetUp()
        {
            _competition = new Competition();

        }

        [Test]
        public void NextTrack_EmptyQueue_ReturnNull()
        {
            Track result = _competition.NextTrack();
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_OneInQueue_ReturnTrack()
        {
            Track track = new Track("Test track", new[] { SectionTypes.StartGrid, SectionTypes.Finish });
            _competition.Tracks.Enqueue(track);
            Track result = _competition.NextTrack();
            Assert.AreEqual(track, result);
        }

        [Test]
        public void NextTrack_OneInQueue_RemoveTrackFromQueue()
        {
            Track track = new Track("Test track", new[] { SectionTypes.StartGrid, SectionTypes.Finish });
            _competition.Tracks.Enqueue(track);
            Track result = _competition.NextTrack();
            result = _competition.NextTrack();
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_TwoInQueue_ReturnNextTrack()
        {
            Track track = new Track("Test track", new[] { SectionTypes.StartGrid, SectionTypes.Finish });
            _competition.Tracks.Enqueue(track);
            Track track2 = new Track("Second test track", new[] { SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Finish });
            _competition.Tracks.Enqueue(track2);
            Track result = _competition.NextTrack();
            Assert.AreNotEqual(track2, result);
        }
    }
}