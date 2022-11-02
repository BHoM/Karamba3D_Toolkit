namespace Karamba3D_ToolkitTests
{
    using BH.Engine.Adapter.Karamba3D;
    using NUnit.Framework;

    [TestFixture]
    public class BaseTest
    {
        [TearDown]
        public void TestTearDown()
        {
            // The logger will be reset after each test to avoid dependency between tests.
            K3dLogger.Clean();
        }
    }
}