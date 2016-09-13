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
        // Set all posible spawn locations for guardDog
        public static List<float[]> guardDogLocations;
        public static List<float[]> guardDogLocations1 = new List<float[]>
            {
                new[] {2366.19f, 3142.537f, 48.20892f},
                new[] {2362.875f, 3118.148f, 48.20892f},
                new[] {2337f, 3128.009f, 48.2035f},
                new[] {2340.128f, 3148.405f, 48.16832f}
            };
        public static List<float[]> guardDogLocations2 = new List<float[]>
            {
                new[] {2420.758f, 3051.209f, 48.15234f},
                new[] {2393.323f, 3027.323f, 48.15283f},
                new[] {2420.204f, 3096.625f, 48.15293f},
                new[] {2393.492f, 3049.265f, 48.63544f}
            };
        public static List<float[]> guardDogLocations3 = new List<float[]>
            {
                new[] {2380.464f, 3036.307f, 48.15262f},
                new[] {2369.535f, 3072.058f, 48.15285f},
                new[] {2338.327f, 3075.032f, 48.15235f}
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
            //trashTruckKeys = World.CreateProp(-331172978, new Vector3(2351.69f, 3118.53f, 48.19f), false, false);
            // Spawn the keys
            //if (trashTruckLocationID > 1)
            //{
                trashTruckKeys = World.CreateProp(-331172978, new Vector3(2370.5f, 3157.19f, 47.55f), false, false);
                // Patio table
                World.CreateProp(-1682596365, new Vector3(2370.49f, 3157.18f, 47.37f), false, false);
            //}
                

            // Set blip for keys
            Common.SpawnPropBlip(trashTruckKeys, BlipSprite.Key, 28, false);
            // Spawn and setup guard dog 1
            guardDog1 = Common.SpawnGuardDog();
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
            // Unlock the trashTruck
            Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, trashTruck, 0);
            // Delete keys
            var trashTruckKeysExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, trashTruckKeys);
            if (trashTruckKeysExists)
                trashTruckKeys.Delete();
            Main.Wait(200);
            // Delete humane labs blip if exists
            var humaneLabsExists = Function.Call<bool>(Hash.DOES_BLIP_EXIST, humaneLabs);
            if (humaneLabsExists)
                humaneLabs.Remove();
            Main.Wait(200);
            // if guardDog2 has never spawned
            if (!guardDog2Spawned)
            {
                // Spawn and setup guard dog 2
                guardDog2 = Common.SpawnGuardDog();
                // Prevent spawning an additional guard dog
                guardDog2Spawned = true;
                UI.ShowSubtitle("~y~keys~w~ found, get to the ~b~Trash truck~s~.", 15000);
            }
            // Make sure dogs always attack
            if (Day1.guardDog1Spawned && Main.currentStage <= 4)
                Common.runToPlayer(Day1.guardDog1);
            if (Day1.guardDog2Spawned && Main.currentStage <= 4)
                Common.runToPlayer(Day1.guardDog2);
            Common.runDebug();
        }
        // Start Day 1 Stage 4
        public static void Day1Stage4()
        {
            // Update Day and Stage
            Main.currentDay = 1;
            Main.currentStage = 4;
            // Remove trashTruck blip
            var trashTruckBlipExists = Function.Call<bool>(Hash.DOES_BLIP_EXIST, trashTruck.CurrentBlip);
            if (trashTruckBlipExists)
                trashTruck.CurrentBlip.Remove();
            // Spawn a blip at HumaneLabs
            humaneLabs = Common.SpawnBlip(3370.720f, 3695.381f, 37.211f, BlipSprite.BigCircle, 21, true);
            // Tranisition weather to be foggy
            Function.Call(Hash._SET_WEATHER_TYPE_OVER_TIME, "FOGGY", 50000);

            // TODO : Remove default guard from checkpoint

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
