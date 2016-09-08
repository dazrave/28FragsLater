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
        /* Day 1 items */
        public Vehicle trashTruck;
        public Vehicle startBike;
        public Ped guardDog1;
        public Ped guardDog2;
        public Blip junkYard;
        public Entity keys;
        /* Global */
        public Ped player = Game.Player.Character;
        public bool noPolice;
        public bool neverWanted;

        public Main()
        {
            /* Make sure all custom global features are switched off by default */
            neverWanted = false;
            noPolice = false;

            /* Link in ticks and key press tracking */
            Tick += onTick;
            KeyDown += onKeyDown;
        }

        void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9) // If F9 is pressed lets...
                startDay(); // Start the method to determin what day it is

            if (e.KeyCode == Keys.F10)
                resetWorld(); // Trigger reset

            if (e.KeyCode == Keys.F12)
                hardReset(); // Trigger hard reset
        }

        void startDay()
        {
            //if (currentDay == 1)
            Day1Stage1();
            // TODO : Track and start the next uncompleted day in the series
        }

        /* Hard reset of world removing everything */
        public void hardReset()
        {
            foreach (Vehicle v in World.GetAllVehicles())
                v.Delete();
            foreach (Ped p in World.GetAllPeds())
                p.Delete();
            currentDay = 0;
            currentStage = 0;
            resetWorld(); // Run usual reset
        }

        /* Soft reset removing mission items */
        public void resetWorld()
        {
            /* Global resets */
            neverWanted = false;
            noPolice = false;
            Common.setWorld(12, 00, 00, false, "CLEAR"); // KEY: Time of day, pause clock, weather type
            Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);

            /* Day 1 reset */
            if (currentDay == 1)
            {
                trashTruck.Delete();
                startBike.Delete();
                /* ---------- UNDER TESTING ----------*/
                guardDog1.Delete();
                junkYard.Remove();
                Function.Call(Hash.SET_BLIP_SPRITE, junkYard, 50);
                //guardDog2.Delete();
                /* ----------- END TESTING -----------*/
            }

            /* Lastly, reset day and stage */
            currentDay = 0;
            currentStage = 0;

            UI.ShowSubtitle("The world has been reset", 5000);
        }

        /* List of days and stages:
         * 1.1 - Spawn > trashTruck / Spawn > key > trashTruck
         * 1.2 - trashTruck > Humane Labs > Gaurd Hut > Park
         * 1.3 - Park > sneak > change clothes > enter Labs
         * 1.4 - Enter labs > find animals > fight scientists
         * 1.5 - Cutscene: Animals break free
         * 1.6 - Evacuate labs > fake objective > die > day completed 
         */

        public void Day1Stage1()
        {
            /* Setup world & player */
            Common.missionFade("out", 2000);
            resetWorld(); // Make sure we've got a clean world first
            currentDay = 1; // Set day
            currentStage = 1; // Set stage
            Common.setWorld(01, 00, 00, true, "CLEAR"); // KEY: Time of day, pause clock, weather type
            Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, Game.Player.Character, true); // Remove all weapon
            Common.givePlayerWeapon(WeaponHash.StunGun, 20);
            Common.givePlayerWeapon(WeaponHash.Crowbar, 1);

            /* Setup (hippie) trailer park start scene */
            //Game.Player.Character.Position = new Vector3(2338.347f, 2568.475f, 47.704f); // Next to hippie tralier
            startBike = Common.SpawnVehicle(VehicleHash.Sanchez, new Vector3(2343.471f, 2568.407f, 46.634f), 307f); // Spawn Sanchez next to trailer

            /* Setup trash truck scene */
            List<float[]> locationList = new List<float[]> // List of potential trashTruck spawn points
            {
                new[] {2348.587f, 3132.766f, 48.209f, 259.197f}, // Inside shed
                new[] {2408.979f, 3035.146f, 47.623f, 358.612f}, // Near planes
                new[] {2350.768f, 3037.305f, 47.624f, 2.531f} // Next to crane
            };
            Random rnd = new Random();
            int locNum = rnd.Next(0, locationList.Count); // Choose random spawn point
            float[] coords = locationList[locNum]; // assign name to choosen spawn point
            trashTruck = Common.SpawnVehicle(VehicleHash.Trash, new Vector3(coords[0], coords[1], coords[2]), coords[3]); // Create and spawn the trashTruck
            Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, trashTruck, 2); // Lock the trashTruck
            Wait(200);
            //Common.SpawnVehicleBlip(trashTruck, BlipSprite.GarbageTruck, 3, true); // Give trashTruck a blue Garbage truck icon on minimap

            
            junkYard = Common.SpawnBlip(2399.415f, 3086.904f, 47.629f, BlipSprite.BigCircle, 3, true);
            


            //Function.Call(Hash.SETTIMERA, 0); // Set the timer to 0, start counting up.

            

            // TODO : Spawn trashTruck randomly in 1 of 3 places around yard
            // TODO : Lock trashTruck if player gets there within 1.5 minutes - SET_VEHICLE_DOORS_LOCKED & SETTIMERA(0)
            // TODO : Spawn keys at set location if trashTruck is locked
            // TODO : Unlock trashTruck if keys are found

            /* Start this stage */
            Common.missionFade("in", 2000);
            UI.ShowSubtitle("Steal the ~b~Trash truck~s~ from the compound.", 15000); // Let player know the current objective
        }
        public void Day1Stage2()
        {
            currentDay = 1;
            currentStage = 2;
            /* ---------- UNDER TESTING ----------*/
            //Function.Call(Hash.REMOVE_BLIP, junkYard);
            /* ----------- END TESTING -----------*/
            Common.SpawnVehicleBlip(trashTruck, BlipSprite.GarbageTruck, 3, false);
            UI.ShowSubtitle("~b~Trash truck~s~ locked. Find the key.", 15000); // Let player know the current objective

            guardDog1 = Common.SpawnPed(PedHash.Rottweiler, trashTruck.GetOffsetInWorldCoords(new Vector3(0.120f, 0.120f, 0.10f)), false); // Spawn guard dog 1
            Common.SpawnPedBlip(guardDog1, BlipSprite.Standard, 1, false);
            //guardDog1.Task.StandStill(1);
            guardDog1.Task.FightAgainst(player);
        }
        public void Day1Stage3()
        {
            currentDay = 1;
            currentStage = 3;
            trashTruck.CurrentBlip.Remove();
            UI.ShowSubtitle("Make your way to the ~b~Humane Research Facility~s~.", 15000); // Let player know the current objective
        }

        void onTick(object sender, EventArgs e) // Ticks are on going (kind of like milliseconds). Therfore these rules loop over and over during the game.
        {
            /* Global tick options */
            Common.wantedLevel(neverWanted); // If neverWanted is set to 'true', the police will leave the player alone
            var playerArrested = Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, player, 0);
            if (player.IsDead.Equals(true) || playerArrested) // reset if player is dead or arrested
                resetWorld();

            /* Day 1 Stage 1 ticks */
            var trashTruckDestroyed = Function.Call<bool>(Hash.IS_ENTITY_DEAD, trashTruck);
            if (trashTruckDestroyed && currentDay == 1 && currentStage <= 2) // If trashTruck is destroyed, Mission over
                resetWorld(); // THIS NEEDS TO END MISSION
            // TODO : Mission End function

            var trashTruckFound = Function.Call<bool>(Hash.IS_PED_TRYING_TO_ENTER_A_LOCKED_VEHICLE, player);
            if (trashTruckFound)
                UI.ShowSubtitle("~b~Trash truck~s~ locked. Find the key.", 15000); // Let player know the current objective
            //Day1Stage2();

            var isDogDead = Function.Call<bool>(Hash.IS_PED_DEAD_OR_DYING, guardDog1, true);
            if (isDogDead && currentDay == 1)
                guardDog1.CurrentBlip.Remove();
            
            if (player.CurrentVehicle != trashTruck && currentDay == 1 && currentStage == 2) // Has player left the trashTruck?
            {
                currentStage = 1;
                Common.SpawnVehicleBlip(trashTruck, BlipSprite.GarbageTruck, 3, true);
                UI.ShowSubtitle("Get back into the ~b~Trash truck~s~.", 15000); // Let player know the current objective
            }
            /* Day 1 Stage 3 ticks */
            if (player.CurrentVehicle == trashTruck && currentDay == 1 && currentStage == 2)
                Day1Stage3();
        }
    }
}
