using NUnit.Framework;
using LockLib.Log;

namespace Test.LogTest
{
    [TestFixture]
    public class LogTest
    {
        [Test]
        public void testLog()
        {
            Debug.Log("aadd");
        }
    }
}