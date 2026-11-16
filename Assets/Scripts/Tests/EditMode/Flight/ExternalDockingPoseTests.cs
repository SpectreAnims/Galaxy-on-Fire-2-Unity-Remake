using GoF2Remake.Data;
using GoF2Remake.Modding;
using GoF2Remake.World;
using NUnit.Framework;
using UnityEngine;

namespace GoF2Remake.Tests
{
    public class ExternalDockingPoseTests
    {
        [Test]
        public void AlignMatchesShipHardpointToStationDockingArm()
        {
            var stationObject = new GameObject("station test");
            try
            {
                stationObject.transform.SetPositionAndRotation(new Vector3(12f, -3f, 8f), Quaternion.Euler(0f, 35f, 0f));
                stationObject.transform.localScale = Vector3.one * 1.5f;
                var stationPoint = new ModWorld.ExternalDockPointSpec
                {
                    id = "arm",
                    position = new Vector3(240f, 80f, -160f),
                    rotation = new Vector3(0f, 70f, 0f)
                };
                var ship = new ShipData
                {
                    externalDockPosition = new Vector3(40f, 25f, -90f),
                    externalDockRotation = new Vector3(0f, -25f, 0f)
                };

                Assert.IsTrue(ExternalDockingPose.TryAlign(stationObject.transform, stationPoint, ship, 0.05f, out var pose));

                var expectedPosition = stationObject.transform.TransformPoint(stationPoint.position * 0.05f);
                var expectedRotation = stationObject.transform.rotation * Quaternion.Euler(stationPoint.rotation);
                var actualPosition = pose.position + pose.rotation * ModShipView.ToLocal(ship.externalDockPosition.Value, 0.05f);
                var actualRotation = pose.rotation * Quaternion.Euler(ship.externalDockRotation);
                Assert.Less(Vector3.Distance(expectedPosition, actualPosition), 0.0001f);
                Assert.Less(Quaternion.Angle(expectedRotation, actualRotation), 0.001f);
            }
            finally { Object.DestroyImmediate(stationObject); }
        }

        [Test]
        public void MissingShipHardpointCannotBeAligned()
        {
            var station = new GameObject("station test");
            try
            {
                Assert.IsFalse(ExternalDockingPose.TryAlign(station.transform,
                    new ModWorld.ExternalDockPointSpec { id = "arm" }, new ShipData(), 0.05f, out _));
            }
            finally { Object.DestroyImmediate(station); }
        }
    }
}
