// ExternalDockingPose.cs
// Resolves a station's manually authored docking-arm transform against a ship's explicit docking hardpoint.

using GoF2Remake.Data;
using GoF2Remake.Modding;
using UnityEngine;

namespace GoF2Remake.World
{
    /// <summary>Pure pose calculation shared by external docking and the Mod Builder's external-spawn preview.</summary>
    public static class ExternalDockingPose
    {
        /// <summary>
        /// Aligns the ship so its authored hardpoint exactly matches the station's authored arm/berth point.
        /// Station position is in station-local game units; ship position is in ship-local position_engine axes
        /// (the same convention as weapon mounts). Both rotations are local Euler degrees.
        /// </summary>
        public static bool TryAlign(Transform station, ModWorld.ExternalDockPointSpec stationPoint,
                                    ShipData ship, float metersPerUnit, out Pose shipPose)
        {
            shipPose = default;
            if (station == null || stationPoint == null || ship?.externalDockPosition == null
                || metersPerUnit <= 0f) return false;

            Vector3 stationPosition = station.TransformPoint(stationPoint.position * metersPerUnit);
            Quaternion stationRotation = station.rotation * Quaternion.Euler(stationPoint.rotation);

            Vector3 shipHardpointPosition = ModShipView.ToLocal(ship.externalDockPosition.Value, metersPerUnit);
            Quaternion shipHardpointRotation = Quaternion.Euler(ship.externalDockRotation);

            Quaternion shipRotation = stationRotation * Quaternion.Inverse(shipHardpointRotation);
            Vector3 shipPosition = stationPosition - shipRotation * shipHardpointPosition;
            shipPose = new Pose(shipPosition, shipRotation);
            return true;
        }
    }
}
