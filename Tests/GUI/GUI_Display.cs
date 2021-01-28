using NUnit.Framework;
using RaceSimulatorGUI;

namespace ControllerTest.GUI
{
    [TestFixture]
    public class GUI_Display
    {

        [SetUp]
        public void Setup()
        {
            Display.Initialise();
        }

        [Test]
        public void Gridsquares_NotNull()
        {
            Assert.IsNotNull(Display.GetGridSquares());
        }
    }
}