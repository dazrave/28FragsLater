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
using System.IO;
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
        public static Ped player = Game.Player.Character;
        public static bool debugMode;
        public static bool debugBoolMessage;
        public static string debugStringMessage;
        public static int currentDay;
        public static int currentStage;
        public static int lastDay;
        public static int lastStage;
        public static string modVersion = "0.3";
        public static int iCurrentGameTime;
        public static int iElapsedGameTime;
        public static int iLastGameTime;

        public Main()
        {
            // Make sure all custom global features are switched off by default
            Common.neverWanted = false;
            Common.noPolice = false;
            debugMode = false;
            // Link in ticks and key press tracking
            Tick += onTick;
            KeyDown += onKeyDown;
            //
            DisableControls();

            UI.Notify("~r~28 Frags Later~w~ v" + modVersion + "", true);
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
                setDebugMode();
            }
            // Debug options only
            if (debugMode)
            {
                // Trigger a soft reset with F11
                if (e.KeyCode == Keys.F11)
                    Common.softReset();   
                // Trigger a hard reset with F12
                if (e.KeyCode == Keys.F12)
                    Common.hardReset();
                // Trigger debug message
                if (e.KeyCode == Keys.End)
                    Common.runDebug();
            } 
        }

        // Disable or enable as required, gives access to all function keys without passing control to the game afterwards. F4, F11 and F12 seem to be available already
        private void DisableControls()
        {
            // Blocks F9 and F10 from being passed to the game.
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, "F9");
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, "F10");
            if (debugMode)
            {
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, "F11");
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, "F12");
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, "END");
            } 
        }

        public static void setDebugMode()
        {
            // Toggle debugMode true/false
            debugMode = !debugMode;
            Wait(100);
            // Set info and trigger notification
            var debugModeResult = debugMode ? "Turned On" : "Switched Off";
            UI.Notify("~y~DEBUG MODE~w~\n" + debugModeResult, true);
            if (debugMode)
            {
                Common.givePlayerWeapon(WeaponHash.Pistol, 500);
                Common.givePlayerWeapon(WeaponHash.Molotov, 500);
                Common.givePlayerWeapon(WeaponHash.Unarmed, 1);
            }
            
        }

        public static void startDay()
        {
            // Run the Day 1 script
            Day1.Day1Stage1();
            // TODO : Track and start the next uncompleted day in the series!
        }

        public static void missionFailed (string message, int day, int stage)
        {
            // Display message
            UI.ShowSubtitle(message, 15000);
            // Set last day/stage
            lastDay = day;
            lastStage = stage;
            // Clear mission
            Common.softReset();
        }

        // Start tick process
        public static void onTick(object sender, EventArgs e) 
        {
            iCurrentGameTime = Game.GameTime;
            iElapsedGameTime = iCurrentGameTime - iLastGameTime;
            iLastGameTime = iCurrentGameTime;

            Game.Player.Character.Euphoria.StaggerFall.AlwaysBendForwards = true;

            // Check wantedLevel settings
            Common.wantedLevel(Common.neverWanted);

            // Day 1 options
            if (currentDay == 1)
            {
                // TODO: Has player arrived at junkYard
                // TODO: Trigger objective message (find trashTruck)

                // Is player trying to enter the locked trashTruck
                var trashTruckFound = Function.Call<bool>(Hash.IS_PED_TRYING_TO_ENTER_A_LOCKED_VEHICLE, Game.Player.Character);
                if (trashTruckFound && currentStage == 1)
                    Day1.Day1Stage2();
                // Have the trashTruckKeys been found?
                var trashTruckKeysExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, Day1.trashTruckKeys);
                if (trashTruckKeysExists && currentStage == 2)
                    if (Game.Player.Character.IsNearEntity(Day1.trashTruckKeys, new Vector3(1, 1, 1)))
                        Day1.Day1Stage3();
                // Has the trashTruck been destroyed
                var trashTruckDestroyed = Function.Call<bool>(Hash.IS_ENTITY_DEAD, Day1.trashTruck);
                if (trashTruckDestroyed && currentStage <= 5)
                    missionFailed("The ~b~Trash truck~s~ has been ~r~destroyed~s~!", currentDay, 0);
                // Is player in trashTruck
                if (player.CurrentVehicle == Day1.trashTruck && currentStage == 3)
                    Day1.Day1Stage4();
                // Has player left trashTruck
                if (player.CurrentVehicle != Day1.trashTruck && currentStage == 4)
                {
                    // Reset to stage 3
                    Day1.Day1Stage3();
                    // Spawn trashTruck blip
                    Common.SpawnVehicleBlip(Day1.trashTruck, BlipSprite.GarbageTruck, 21, true);
                    // Display new mission objective
                    UI.ShowSubtitle("Get back into the ~b~Trash truck~s~.", 15000);
                    Common.runDebug();
                }
                // Make sure dogs always attack
                if (Day1.guardDog1Spawned && currentStage <= 4)
                    Common.runToPlayer(Day1.guardDog1);
                if (Day1.guardDog2Spawned && currentStage <= 4)
                    Common.runToPlayer(Day1.guardDog2);

            }
        }
    }
}