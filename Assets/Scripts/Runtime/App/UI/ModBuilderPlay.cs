// ModBuilderPlay.cs
// Remake-only: the Mod Builder's play tests (Modding.ModBuilder.PlayTesting): the game as a player has it with the project on,
// from the builder's station: plainly (to fly to the project's systems and stations, meet its ships, take its bar missions),
// with one of its quests started (EventRunner.StartQuest), or from its campaign's start (ModCampaigns.Start). The station
// menu, the flight HUD and the pause menu work as in a game; their Back to Mod Builder (and a death, a failed quest) ends it
// (End: the builder's game as the project opened, its hangar). Plain C#.

using GoF2Remake.Data;
using System.Linq;
using GoF2Remake.Modding;
using GoF2Remake.World;
using UnityEngine.SceneManagement;

namespace GoF2Remake.UI
{
    public static class ModBuilderPlay
    {
        static int externalDockTestShip = -1, externalDockTestStation = -1;
        static string externalDockTestPoint;

        /// <summary>Starts a Mod Builder test directly in space with the chosen project ship attached to a station arm.</summary>
        public static string StartExternalDock(int shipIndex, int stationIndex, string pointId)
        {
            var db = Database.Load();
            var ship = db.Ship(shipIndex);
            var point = ModWorld.ExternalDockPoints(stationIndex).FirstOrDefault(p => p.id == pointId);
            if (ship?.externalDockPosition == null)
                return Localization.Extra("mbExternalDockShipMissing", "This ship has no external docking hardpoint. Add one on its Hardpoints page first.");
            if (point == null)
                return Localization.Extra("mbExternalDockStationMissing", "That station docking point is no longer available. Check the station's orbit settings.");
            if (!PlayerHull.Big)
                return Localization.Extra("mbExternalDockShipSmall", "External docking is for large ships. This ship's hull is not classified as large.");

            var projectShip = ModBuilder.Project?.Ships.FirstOrDefault(p => ModBuilder.IndexOf(p) == shipIndex);
            if (projectShip == null)
                return Localization.Extra("mbExternalDockProjectShipMissing", "The selected ship is no longer part of this project.");

            ModBuilder.BeginPlayTest();
            var previous = ModBuilder.Ship;
            ModBuilder.Ship = projectShip;
            ModBuilder.ApplyShip();
            ModBuilder.Ship = previous;
            Session.StationIndex = stationIndex;
            Session.ArrivedByTravel = false;
            Session.LaunchedFromStation = false;
            Session.DockedFromSpace = false;
            externalDockTestShip = shipIndex;
            externalDockTestStation = stationIndex;
            externalDockTestPoint = pointId;
            SceneManager.LoadScene("Space");
            return null;
        }

        /// <summary>Consumed once by SpaceLevel while creating the requested test ship.</summary>
        public static bool TryConsumeExternalDockSpawn(int station, int ship, out string pointId)
        {
            pointId = null;
            if (externalDockTestShip != ship || externalDockTestStation != station || string.IsNullOrEmpty(externalDockTestPoint))
                return false;
            pointId = externalDockTestPoint;
            externalDockTestShip = externalDockTestStation = -1;
            externalDockTestPoint = null;
            return true;
        }
        /// <summary>The play test of the project as it is (the station's menu at the builder's station).</summary>
        public static void Start()
        {
            ModBuilder.BeginPlayTest();
            SceneManager.LoadScene("Station");
        }

        /// <summary>A play test with the quest 'name' started (its "Starts when" skipped); a bar mission is offered in the
        /// Space Lounge instead, at the builder's station (or any other), to be taken like a player takes it. The reason when
        /// it can't start.</summary>
        public static string StartQuest(string name)
        {
            Events.EventMissions.PlayTestOffer = null;
            if (Events.EventRunner.IsBarMission(name))
            {
                ModBuilder.BeginPlayTest();
                Events.EventMissions.PlayTestOffer = name;
                SceneManager.LoadScene("Station");
                return null;
            }
            ModBuilder.BeginPlayTest();
            string why = Events.EventRunner.StartQuest(name);
            if (why != null) { ModBuilder.EndPlayTest(); return why; }
            SceneManager.LoadScene("Station");
            return null;
        }

        /// <summary>A play test from the project's campaign's start (where it docks, its ship, money and quest). The reason when
        /// it can't.</summary>
        public static string StartCampaign()
        {
            var c = ModCampaigns.All().Find(x => x.mod == ModManager.BuilderProject);
            if (c == null) return Localization.Extra("mbNoCampaignToPlay", "The game hasn't read the campaign yet (or it has an error): check the messages.");
            ModBuilder.BeginPlayTest();
            string scene = ModCampaigns.Start(Database.Shared, c, out string error);
            if (scene == null) { ModBuilder.EndPlayTest(); return error; }
            SceneManager.LoadScene(scene);
            return null;
        }

        /// <summary>Back to the builder.</summary>
        public static void End() => ModBuilder.EndPlayTest();
    }
}
