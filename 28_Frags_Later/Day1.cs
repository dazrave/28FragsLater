using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static Vehicle startBike;
        public static Ped guardDog1;
        public static Ped guardDog2;
        public static Blip junkYard;

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
            //wait(200);
            // Remove all player weapons (unless in debug)
            if (!Main.debugMode)
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, Game.Player.Character, true);
            // Give player weapons
            Common.givePlayerWeapon(WeaponHash.StunGun, 20);
            Common.givePlayerWeapon(WeaponHash.Crowbar, 1);
            // Spawn player next to Hippie trailer (unless in debug)
            if (!Main.debugMode)
                Game.Player.Character.Position = new Vector3(2338.347f, 2568.475f, 47.704f);
            // Spawn dirt bike next to Hippie trailer
            startBike = Common.SpawnVehicle(VehicleHash.Sanchez, new Vector3(2343.471f, 2568.407f, 46.634f), 307f);
            // Set all posible spawn locations for traskTruck
            List<float[]> locationList = new List<float[]>
            {
                new[] {2348.587f, 3132.766f, 48.209f, 259.197f}, // Inside shed
                new[] {2408.979f, 3035.146f, 47.623f, 358.612f}, // Near planes
                new[] {2350.768f, 3037.305f, 47.624f, 2.531f} // Next to crane
            };
            // Choose spawn point at random
            Random rnd = new Random();
            int locNum = rnd.Next(0, locationList.Count);
            float[] coords = locationList[locNum];
            // Spawn trashTruck at chosen location
            trashTruck = Common.SpawnVehicle(VehicleHash.Trash, new Vector3(coords[0], coords[1], coords[2]), coords[3]);
            // Lock trashTruck's doors
            Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, trashTruck, 2); // Lock the trashTruck
            //Wait(200);
            // Spawn a blip at junkYard
            junkYard = Common.SpawnBlip(2399.415f, 3086.904f, 47.629f, BlipSprite.BigCircle, 21, true);
            // Fade in screen (unless in debug)
            if (!Main.debugMode)
                Common.missionFade("in", 2000);
            // Display mission objective
            UI.ShowSubtitle("Steal the ~b~Trash truck~s~ from the compound.", 15000);
            if (Main.debugMode)
                UI.Notify("Day[" + Main.currentDay + "] Stage[" + Main.currentStage + "]", true);
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

            /* -------- UNDER TESTING --------*/
            //guardDog1 = Common.SpawnPed(PedHash.Rottweiler, new Vector3(2399.415f, 3086.904f, 47.629f), false); // Spawn guard dog 1
            //Common.SpawnPedBlip(guardDog1, BlipSprite.Standard, 1, false);
            //guardDog1.Task.StandStill(0);
            //guardDog1.Task.FightAgainst(player);
            /* -------- END OF TESTNG ---------*/

            // Display mission objective
            UI.ShowSubtitle("~b~Trash truck~s~ locked. Find the key.", 15000);
            if (Main.debugMode)
                UI.Notify("Day[" + Main.currentDay + "] Stage[" + Main.currentStage + "]", true);
        }
        // Start Day 1 Stage 3
        public static void Day1Stage3()
        {
            // Update Day and Stage
            Main.currentDay = 1;
            Main.currentStage = 3;
            // Remove trashTruck blip
            trashTruck.CurrentBlip.Remove();
            // TODO : Spawn humaneLabs blip

            // Display mission objective
            UI.ShowSubtitle("Make your way to the ~b~Humane Research Facility~s~.", 15000);
            if (Main.debugMode)
                UI.Notify("Day[" + Main.currentDay + "] Stage[" + Main.currentStage + "]", true);
        }
    }
}
