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
    public class Common
    {

        /* Global */
        public static bool noPolice;
        public static bool neverWanted;
        public static bool noPeds;
        public static bool noBlips;

        // Spawn a ped
        public static Ped SpawnPed(Model pedname, Vector3 location, bool friendly)
        {
            // Create ped at location
            Ped ped = World.CreatePed(pedname, location);
            // Should ped be an enemy
            if (!friendly)
                ped.Task.FightAgainst(Game.Player.Character);
            return ped;
        }
        // Spawn gaurd dog
        public static Ped SpawnGuardDog()
        {
            // Choose spawn point at random
            Random rndGuardDog = new Random();
            int guardDoglocNum = rndGuardDog.Next(0, Day1.guardDogLocations.Count);
            float[] guardDogCoords = Day1.guardDogLocations[guardDoglocNum];

            // Spawn the guardDog2
            Ped dog = SpawnPed(PedHash.Rottweiler, new Vector3(guardDogCoords[0], guardDogCoords[1], guardDogCoords[2]), false);
            var playerCoords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, Game.Player.Character, true);
            dog.Task.RunTo(playerCoords);
            dog.Task.FightAgainst(Game.Player.Character);
            Function.Call(Hash.CAN_KNOCK_PED_OFF_VEHICLE, dog);
            Function.Call(Hash.SET_ENTITY_PROOFS, dog, false, false, false, true, true, false, false, false);

            return dog;
        }
        // Spawn blip for Prop
        public static Blip SpawnPropBlip(Prop propName, BlipSprite sprite, int blipColour, bool showroute)
        {
            // Add blip to vehicle
            propName.AddBlip();
            // Give blip an icon
            propName.CurrentBlip.Sprite = sprite;
            // Set GPS route for blip on/off
            propName.CurrentBlip.ShowRoute = showroute;
            // Set colour of blip
            Function.Call(Hash.SET_BLIP_COLOUR, propName.CurrentBlip, blipColour);
            // Set colour of GPS route
            Function.Call(Hash.SET_BLIP_ROUTE_COLOUR, propName.CurrentBlip, blipColour);
            return propName.CurrentBlip;
        }
        // Spawn a location blip
        public static Blip SpawnBlip(float loc1, float loc2, float loc3, BlipSprite sprite, int blipColour, bool showroute)
        {
            // Create blip at location
            Blip blip = World.CreateBlip(new Vector3(loc1, loc2, loc3));
            // Set GPS route for blip on/off
            blip.ShowRoute = showroute;
            // Set colour of blip
            Function.Call(Hash.SET_BLIP_COLOUR, blip, blipColour);
            // Set colour of GPS route
            Function.Call(Hash.SET_BLIP_ROUTE_COLOUR, blip, blipColour);
            return blip;
        }
        // Spawn blip for Ped
        public static Blip SpawnPedBlip(Ped pedName, BlipSprite sprite, int blipColour, bool isEnemy)
        {
            // Add blip to ped
            pedName.AddBlip();
            // Set blip colour
            Function.Call(Hash.SET_BLIP_COLOUR, pedName.CurrentBlip, blipColour);
            // Is ped an enemy
            Function.Call(Hash.SET_BLIP_AS_FRIENDLY, pedName.CurrentBlip, isEnemy);
            return pedName.CurrentBlip;
        }
        // Spawn blip for vehicle
        public static Blip SpawnVehicleBlip(Vehicle vehName, BlipSprite sprite, int blipColour, bool showroute)
        {
            // Add blip to vehicle
            vehName.AddBlip();
            // Give blip an icon
            vehName.CurrentBlip.Sprite = sprite;
            // Set GPS route for blip on/off
            vehName.CurrentBlip.ShowRoute = showroute;
            // Set colour of blip
            Function.Call(Hash.SET_BLIP_COLOUR, vehName.CurrentBlip, blipColour); 
            // Set colour of GPS route
            Function.Call(Hash.SET_BLIP_ROUTE_COLOUR, vehName.CurrentBlip, blipColour);
            return vehName.CurrentBlip;
        }
        // Spawn vehicle
        public static Vehicle SpawnVehicle(Model vehName, Vector3 location, float heading)
        {
            // Create vehicle at location
            Vehicle vehicle = World.CreateVehicle(vehName, location, heading);
            // Place vehicle at ground level
            vehicle.PlaceOnGround();
            return vehicle;
        }
        // Set weather and time
        public static void setWorld(int hour, int min, int sec, bool pauseClock, string weather)
        {
            // Set the time of day
            Function.Call(Hash.SET_CLOCK_TIME, hour, min, sec);
            // Stop/Start the time
            Function.Call(Hash.PAUSE_CLOCK, pauseClock);
            // Set weather type
            Function.Call(Hash.SET_WEATHER_TYPE_NOW, weather);
        }
        // Fade missions in/out
        public static void missionFade(string direction, int time)
        {
            if (direction == "out")
                Game.FadeScreenOut(time);
            Script.Wait(time);
            if (direction == "in")
                Game.FadeScreenIn(time);
        }
        // Remove players wanted level
        public static void wantedLevel(bool neverWanted)
        {
            if (neverWanted)
                Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player); 
            Function.Call(Hash.SET_POLICE_IGNORE_PLAYER, Game.Player, neverWanted);
        }
        // Give player weapon
        public static void givePlayerWeapon(WeaponHash weaponHash, int ammo)
        {
            Game.Player.Character.Weapons.Give(weaponHash, ammo, true, true);
        }
        public static void removeAllPeds()
        {
            // Detect and delete all peds
            
        }
        // Run a hard reset of the world
        public static void hardReset()
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
            // Detect and delete all props
            //foreach (Prop o in World.GetAllProps())
            //    o.Delete();
            // Reset Day and Stage values (do this last)
            Main.currentDay = 0;
            Main.currentStage = 0;
            if (Main.debugMode)
                UI.Notify("~y~DEBUG MODE~w~\nHard Reset", true);
        }

        // Run a soft reset of the world
        public static void softReset()
        {
            // Global reset elements
            neverWanted = false;
            noPolice = false;
            noBlips = false;
            // Reset to midday, make sure the clock isn't paused and clear the weather
            Common.setWorld(12, 00, 00, false, "CLEAR");
            World.SetBlackout(false);
            // Remove any wanted stars from player
            Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);
            // Reset all Day 1 elements
            if (Main.currentDay == 1)
            {
                // Day1.trashTruck vehicle
                var trashTruckExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, Day1.trashTruck);
                if (trashTruckExists)
                    Day1.trashTruck.Delete();
                // Delete keys
                var trashTruckKeysExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, Day1.trashTruckKeys);
                if (trashTruckKeysExists)
                    Day1.trashTruckKeys.Delete();
                // Day1.startBike vehicle
                var startBikeExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, Day1.startBike);
                if (startBikeExists)
                    Day1.startBike.Delete();
                // junkYard blip
                var junkYardExists = Function.Call<bool>(Hash.DOES_BLIP_EXIST, Day1.junkYard);
                if (junkYardExists)
                    Day1.junkYard.Remove();
                var guardDog1Exists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, Day1.guardDog1);
                if (guardDog1Exists)
                    Day1.guardDog1.Delete();
                var guardDog2Exists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, Day1.guardDog2);
                if (guardDog2Exists)
                    Day1.guardDog2.Delete();
            }

            /* -------- UNDER TESTING --------*/
            Function.Call(Hash.SET_VEHICLE_POPULATION_BUDGET, true);
            Function.Call(Hash.SET_PED_POPULATION_BUDGET, true);
            Function.Call(Hash.SET_PED_IS_DRUNK, Game.Player.Character, false);
            /* -------- END OF TESTNG ---------*/

            if (Main.debugMode)
                UI.Notify("~y~DEBUG MODE~w~\nSoft Reset", true);
        }
        // Debug mode script
        public static void runDebug()
        {
            if (Main.debugMode)
            {
                UI.Notify("~y~DEBUG MODE~w~\nDay: " + Main.currentDay + ", Stage: " + Main.currentStage + "", true);
                //UI.Notify("~y~DEBUG~w~ isDogDead: " + Main.debugBoolMessage, true);
            }
        }
    }
}
