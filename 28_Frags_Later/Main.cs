/* 
28 Frags Later ALPHA
by DazRave
www.28fragslater.com

DISCLAIMER
I've never scripted anything in C# before, in fact I've never really used
Visual Studio before...always been a Sublime Text 3 sort of person with
fairly average PHP skills. This script is also the very first time I've 
attempted to mod GTA, ever.

Therefore it's probably quite roughly cut code. I take no responsibility for
your game crashing. If you're having trouble installing the mod, I will try
my hardest to help with the very limited knowledge I have.

Feel free to use any of my code where ever you please. Rip it apart, rebuild
it. Make more missions to share with others.Enjoy the missions I have made.
So long as my work is providing more life to the ever amazing GTA franchise
of games, I'm a happy modder. 

To get in touch, email DazRave@28fragslater.com

Special mention and credits:
Alexander Blade for Scripthook
Rugz007 for Left4Santos
sollaholla for Tonya Tow Jobs
*/

/* Lets include some stuff that we'll need to call */
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

// For the record...
// I've never scripted in C# before now however I have
// some PHP, JavaScript and other web development
// knowledge. I've tried to comment as much as I 
// could in order to break the script down
// into easy to understand chunks. I can't confirm
// that my comments/explinations are correct though!

namespace _28_Frags_Later
{
    public class Main : Script
    {
        /* Setup */
        public int currentDay;
        public int currentStage;
        public int lastDay;
        public int lastStage;
        /* Day 1 items */
        public Vehicle trashTruck;
        public Vehicle startBike;
        public Ped guardDog1;
        public Ped guardDog2;
        public Blip junkYard;
        /* Global */
        public Ped player = Game.Player.Character;
        public bool noPolice;
        public bool neverWanted;
        public bool debugMode;

        public Main()
        {
            // Make sure all custom global features are switched off by default
            neverWanted = false;
            noPolice = false;
            debugMode = true;
            // Link in ticks and key press tracking
            Tick += onTick;
            KeyDown += onKeyDown;
        }

        // On 'key pressed down' events
        void onKeyDown(object sender, KeyEventArgs e)
        {
            // Start the mod with F9
            if (e.KeyCode == Keys.F9) 
                startDay();
            // Debug mode on/off with F10
            if (e.KeyCode == Keys.F10)
            {
                // Toggle debugMode true/false
                debugMode = !debugMode;
                Wait(100);
                // Set info and trigger notification
                var debugModeResult = debugMode ? "ON" : "OFF";
                UI.Notify("Debug mode " + debugModeResult, true);
            }
            // Debug options only
            if (debugMode)
            {
                // Trigger a soft reset with F11
                if (e.KeyCode == Keys.F11)
                    softReset();   
                // Trigger a hard reset with F12
                if (e.KeyCode == Keys.F12)
                    hardReset();
            } 
        }

        void startDay()
        {
            // Run the Day 1 script
            Day1Stage1();
            // TODO : Track and start the next uncompleted day in the series!
        }

        // Run a hard reset of the world
        public void hardReset()
        {
            // Run a soft reset first, just to be sure!
            softReset();
            // Detect and delete all vehicles
            foreach (Vehicle v in World.GetAllVehicles())
                v.Delete();
            // Detect and delete all peds
            foreach (Ped p in World.GetAllPeds())
                p.Delete();
            // Detect and delete all active blips
            foreach (Blip b in World.GetActiveBlips())
                b.Remove();
            Wait(200);
            // Reset Day and Stage values (do this last)
            currentDay = 0;
            currentStage = 0;
            if (debugMode)
                UI.Notify("Hard Reset", true);
        }

        // Run a soft reset of the world
        public void softReset()
        {
            // Global reset elements
            neverWanted = false;
            noPolice = false;
            // Reset to midday, make sure the clock isn't paused and clear the weather
            Common.setWorld(12, 00, 00, false, "CLEAR");
            // Remove any wanted stars from player
            Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);
            // Reset all Day 1 elements
            if (currentDay == 1)
            {
                // trashTruck vehicle
                var trashTruckExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, trashTruck);
                if (trashTruckExists)
                    trashTruck.Delete();
                // startBike vehicle
                var startBikeExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, startBike);
                if (startBikeExists)
                    startBike.Delete();
                // junkYard blip
                var junkYardExists = Function.Call<bool>(Hash.DOES_BLIP_EXIST, junkYard);
                if (junkYardExists)
                    junkYard.Remove();
                /* -------- UNDER TESTING --------*/
                /*var guardDog1Exists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, guardDog1);
                if (guardDog1Exists)
                    guardDog1.Delete();*/
                /* -------- END OF TESTNG ---------*/
            } 
            if(debugMode)
                UI.Notify("Soft Reset", true);
        }
        
        public void missionFailed (string message, int day, int stage)
        {
            // Display message
            UI.ShowSubtitle(message, 15000);
            // Set last day/stage
            lastDay = day;
            lastStage = stage;
            // Clear mission
            softReset();
        }

        // Start Day 1 Stage 1
        public void Day1Stage1()
        {
            // Fade out screen (unless in debug)
            if (!debugMode)
                Common.missionFade("out", 2000);
            // Reset world and set day/stage values
            softReset();
            currentDay = 1;
            currentStage = 1;
            Common.setWorld(01, 00, 00, true, "CLEAR"); // KEY: Time of day, pause clock, weather type  
            Wait(200);
            // Remove all player weapons (unless in debug)
            if (!debugMode)                                                                            
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, Game.Player.Character, true);
            // Give player weapons
            Common.givePlayerWeapon(WeaponHash.StunGun, 20);
            Common.givePlayerWeapon(WeaponHash.Crowbar, 1);
            // Spawn player next to Hippie trailer (unless in debug)
            if (!debugMode)
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
            Wait(200);
            // Spawn a blip at junkYard
            junkYard = Common.SpawnBlip(2399.415f, 3086.904f, 47.629f, BlipSprite.BigCircle, 21, true);
            // Fade in screen (unless in debug)
            if (!debugMode)
                Common.missionFade("in", 2000);
            // Display mission objective
            UI.ShowSubtitle("Steal the ~b~Trash truck~s~ from the compound.", 15000);
            if (debugMode)
                UI.Notify("Day[" + currentDay + "] Stage[" + currentStage + "]", true);
        }
        // Start Day 1 Stage 2
        public void Day1Stage2()
        {
            // Update Day and Stage
            currentDay = 1;
            currentStage = 2;
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
            if (debugMode)
                UI.Notify("Day[" + currentDay + "] Stage[" + currentStage + "]", true);
        }
        // Start Day 1 Stage 3
        public void Day1Stage3()
        {
            // Update Day and Stage
            currentDay = 1;
            currentStage = 3;
            // Remove trashTruck blip
            trashTruck.CurrentBlip.Remove();
            // TODO : Spawn humaneLabs blip

            // Display mission objective
            UI.ShowSubtitle("Make your way to the ~b~Humane Research Facility~s~.", 15000);
            if (debugMode)
                UI.Notify("Day[" + currentDay + "] Stage[" + currentStage + "]", true);
        }

        // Start tick process
        void onTick(object sender, EventArgs e) 
        {
            // Check wantedLevel settings
            Common.wantedLevel(neverWanted);

            // Day 1 options
            if (currentDay == 1)
            {
                // Is player trying to enter the locked trashTruck
                var trashTruckFound = Function.Call<bool>(Hash.IS_PED_TRYING_TO_ENTER_A_LOCKED_VEHICLE, Game.Player.Character);
                if (trashTruckFound && currentStage == 1)
                    Day1Stage2();
                // Has the trashTruck been destroyed
                var trashTruckDestroyed = Function.Call<bool>(Hash.IS_ENTITY_DEAD, trashTruck);
                if (trashTruckDestroyed && currentStage <= 2)
                    missionFailed("The ~b~Trash truck~s~ has been ~r~destroyed~s~!", currentDay, 0);
                // Is player in trashTruck
                if (player.CurrentVehicle == trashTruck && currentStage == 2)
                    Day1Stage3();
                // Has player left trashTruck
                if (player.CurrentVehicle != trashTruck && currentStage == 3)
                {
                    // Reset to stage 2
                    currentStage = 2;
                    // Spawn trashTruck blip
                    Common.SpawnVehicleBlip(trashTruck, BlipSprite.GarbageTruck, 21, true);
                    // Display new mission objective
                    UI.ShowSubtitle("Get back into the ~b~Trash truck~s~.", 15000);
                    if (debugMode)
                        UI.Notify("Day[" + currentDay + "] Stage[" + currentStage + "]", true);
                }
                
            }
            /* -------- UNDER TESTING --------*/
            /*
            var isDogDead = Function.Call<bool>(Hash.IS_PED_DEAD_OR_DYING, guardDog1, true);
            if (isDogDead && currentDay == 1)
                guardDog1.CurrentBlip.Remove();
            */
            /* -------- END OF TESTNG ---------*/

        }
    }
}
