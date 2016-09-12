using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;

namespace _28_Frags_Later
{
    public class Day1
    {
        /* Day 1 items */
        public static Vehicle trashTruck;
        public static Prop trashTruckKeys;
        public static int trashTruckLocationID;
        public static Vehicle startBike;
        // Set all posible spawn locations for guardDog2
        public static List<float[]> guardDogLocations = new List<float[]>
            {
                new[] {2433.019f, 3102.601f, 48.15314f},
                new[] {2420.096f, 3116.658f, 48.22654f},
                new[] {2398.185f, 3092.568f, 48.15303f},
                new[] {2380.436f, 3103.357f, 48.15226f},
                new[] {2359.383f, 3146.506f, 48.21078f},
                new[] {2335.761f, 3130.27f, 48.19604f},
                new[] {2431.36f, 3152.774f, 48.19345f},
                new[] {2398.923f, 3080.626f, 49.08483f},
                new[] {2385.458f, 3030.105f, 48.15287f},
                new[] {2332.521f, 3072.917f, 48.15255f}
            };
        public static Ped guardDog1;
        public static bool guardDog1Spawned;
        public static Ped guardDog2;
        public static bool guardDog2Spawned;
        public static Blip junkYard;
        public static Blip humaneLabs;

        // Start Day 1 Stage 1
        public static void Day1Stage1()
        {
            // Fade out screen (unless in debug)
            if (!Main.debugMode)
                Common.missionFade("out", 2000);
            // Reset world and set day/stage values
            Common.softReset();
            Main.currentDay = 1;
            Main.currentStage = 1;
            Common.setWorld(01, 00, 00, true, "CLEAR"); // KEY: Time of day, pause clock, weather type  
            // Remove all player weapons (unless in debug)
            if (!Main.debugMode)
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, Game.Player.Character, true);
            // Give player weapons
            Common.givePlayerWeapon(WeaponHash.StunGun, 20);
            Common.givePlayerWeapon(WeaponHash.Crowbar, 1);
            Common.givePlayerWeapon(WeaponHash.Flashlight, 1);
            Common.givePlayerWeapon(WeaponHash.Unarmed, 1);
            // Spawn player next to Hippie trailer (unless in debug)
            if (!Main.debugMode)
                Game.Player.Character.Position = new Vector3(2338.347f, 2568.475f, 47.704f);
            // Spawn dirt bike next to Hippie trailer
            startBike = Common.SpawnVehicle(VehicleHash.Sanchez, new Vector3(2343.471f, 2568.407f, 46.634f), 307f);
            // Fade in screen (unless in debug)
            if (!Main.debugMode)
                Common.missionFade("in", 2000);
            // Set all posible spawn locations for traskTruck
            List<float[]> trashTruckLocations = new List<float[]>
            {
                new[] {2348.587f, 3132.766f, 48.209f, 259.197f, 1}, // Inside shed
                new[] {2408.979f, 3035.146f, 47.623f, 358.612f, 2}, // Near planes
                new[] {2350.768f, 3037.305f, 47.624f, 2.531f, 3} // Next to crane
            };
            // Choose spawn point at random
            Random rndTrashTuck = new Random();
            int trashTrucklocNum = rndTrashTuck.Next(0, trashTruckLocations.Count);
            float[] coords = trashTruckLocations[trashTrucklocNum];
            // Spawn trashTruck at chosen location
            trashTruck = Common.SpawnVehicle(VehicleHash.Trash, new Vector3(coords[0], coords[1], coords[2]), coords[3]);
            // Set location ID
            trashTruckLocationID = (int) coords[4];
            // Lock trashTruck's doors
            Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, trashTruck, 2);
            // Spawn a blip at junkYard
            junkYard = Common.SpawnBlip(2399.415f, 3086.904f, 47.629f, BlipSprite.BigCircle, 21, true);
            // Display mission objective
            UI.ShowSubtitle("Steal the ~b~Trash truck~s~ from the ~b~Junk Yard~s~.", 15000);
            Common.runDebug();
        }
        // Start Day 1 Stage 2
        public static void Day1Stage2()
        {
            // Update Day and Stage
            Main.currentDay = 1;
            Main.currentStage = 2;
            // Remove junkYard blip
            junkYard.Remove();
            // Add blip to trashTruck
            Common.SpawnVehicleBlip(trashTruck, BlipSprite.GarbageTruck, 21, false);
            // Spawn the keys
            trashTruckKeys = World.CreateProp(-331172978, new Vector3(2351.69f, 3118.53f, 48.19f), false, false);
            // Set blip for keys
            Common.SpawnPropBlip(trashTruckKeys, BlipSprite.Key, 28, false);

            /* ------------------------------------------ TESTING --------------------- */
            // Spawn and setup guard dog 1
            guardDog1 = Common.SpawnGuardDog();
            /* ------------------------------------------ TESTING --------------------- */

            // Prevent spawning an additional guard dog
            guardDog1Spawned = true;
            // Display mission objective
            UI.ShowSubtitle("~b~Trash truck~s~ locked. Get the ~y~keys~w~.", 15000);
            Common.runDebug();
        }
        public static void Day1Stage3()
        {
            Main.currentDay = 1;
            Main.currentStage = 3;
            // Set guardDog1 as dead (so script doesn't rerun!)
            guardDog1Spawned = false;
            // Unlock the trashTruck
            Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, trashTruck, 0);
            // Delete keys
            var trashTruckKeysExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, trashTruckKeys);
            if (trashTruckKeysExists)
                trashTruckKeys.Delete();
            // Delete humane labs blip if exists
            var humaneLabsExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, humaneLabs);
            if (humaneLabsExists)
                humaneLabs.Remove();
            // if guardDog2 has never spawned
            if (!guardDog2Spawned)
            {
                /* ------------------------------------------ TESTING --------------------- */
                // Spawn and setup guard dog 2
                guardDog2 = Common.SpawnGuardDog();
                /* ------------------------------------------ TESTING --------------------- */

                // Prevent spawning an additional guard dog
                guardDog2Spawned = true;
                UI.ShowSubtitle("~y~keys~w~ found, get to the ~b~Trash truck~s~.", 15000);
            }
            Common.runDebug();
        }
        // Start Day 1 Stage 4
        public static void Day1Stage4()
        {
            // Update Day and Stage
            Main.currentDay = 1;
            Main.currentStage = 4;

            /* ------------------------------------------ TESTING --------------------- */
            // Remove trashTruck blip
            var trashTruckBlipExists = Function.Call<bool>(Hash.DOES_BLIP_EXIST, trashTruck.CurrentBlip);
            if (trashTruckBlipExists)
                trashTruck.CurrentBlip.Remove();
            /* ------------------------------------------ TESTING END --------------------- */

            // Spawn a blip at HumaneLabs
            humaneLabs = Common.SpawnBlip(3370.720f, 3695.381f, 37.211f, BlipSprite.BigCircle, 21, true);

            // Tranisition weather to be foggy
            Function.Call(Hash._SET_WEATHER_TYPE_OVER_TIME, "FOGGY", 50000);

            // TODO : Remove default guard from checkpoint

            /* ------------------------------------------ TESTING --------------------- */
            // Spawn security
            var gateGuard1 = World.CreatePed(PedHash.Security01SMM, new Vector3(3370.720f, 3695.381f, 37.211f));
            // Give gateGuard1 a gun that's holstered
            gateGuard1.Weapons.Give(WeaponHash.CombatPistol, 500, false, true);
            // Give gateGuard1 a clipboard
            Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, gateGuard1, "WORLD_HUMAN_CLIPBOARD", 0, true);

            // WORLD_HUMAN_SECURITY_SHINE_TORCH
            // WORLD_HUMAN_GUARD_PATROL
            // WORLD_HUMAN_GUARD_STAND

            /* ------------------------------------------ TESTING END --------------------- */

            // Display mission objective
            UI.ShowSubtitle("Drive to the ~b~Humane Research Facility~s~.", 15000);
            Common.runDebug();
        }
        // Start Day 1 Stage 5
        public static void Day1Stage5()
        {
            // Update Day and Stage
            Main.currentDay = 1;
            Main.currentStage = 5;
            // Delete humane labs blip if exists
            var humaneLabsExists = Function.Call<bool>(Hash.DOES_BLIP_EXIST, humaneLabs);
            if (humaneLabsExists)
                humaneLabs.Remove();

            // TODO : Add parking hotspot + blip
            // TODO : Make guard wave truck through checkpoint?

            // Display mission objective
            UI.ShowSubtitle("Park the ~b~Trash truck~w~ in the service area", 15000);
            Common.runDebug();
        }
    }
}
