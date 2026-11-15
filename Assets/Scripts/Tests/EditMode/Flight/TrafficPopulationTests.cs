using NUnit.Framework;
using GoF2Remake.Flight;

namespace GoF2Remake.Tests
{
    public class TrafficPopulationTests
    {
        [TestCase(7, 1f, 7)]
        [TestCase(7, 0f, 0)]
        [TestCase(8, 0.5f, 4)]
        [TestCase(5, 2f, 10)]
        [TestCase(5, 4f, 20)]
        [TestCase(5, 10f, 20)]
        [TestCase(0, 2f, 0)]
        public void ScaleAmbientCount_UsesConfiguredMultiplier(int count, float multiplier, int expected)
        {
            Assert.AreEqual(expected, TrafficPlan.ScaleAmbientCount(count, multiplier));
        }
    }
}
