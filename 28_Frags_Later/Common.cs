// 28 Frags Later 
// by DazRave
// www.28fragslater.com
// 
// Here we have all of the functions used
// globally throughout the script. Things that
// are commonly used like spawning peds & vehicles
// 
// Feel free to use these in your own scripts,
// most of what I have learnt has come from
// hacking at other peoples scripts to create
// my own!
//
// Questions? 
// Email: GTA@dazrave.uk

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
    class Common
    {
        /* Ped spawner */
        public static Ped SpawnPed(Model pedname, Vector3 location, bool friendly)
        {
            Ped ped = World.CreatePed(pedname, location); // Create ped and place at location
            if (!friendly) // Check if friendly and if not...
                ped.Task.FightAgainst(Game.Player.Character); // Set as enemy
            return ped;
        }
        /* Blip spawner */
        public static Blip SpawnBlip(float loc1, float loc2, float loc3, BlipSprite sprite, int blipColour, bool showroute)
        {
            Blip blip = World.CreateBlip(new Vector3(loc1, loc2, loc3)); // Create blip at location
            //blip.Sprite = sprite; // Type of sprite (icon) to show on map
            blip.ShowRoute = showroute; // GPS true or false
            Function.Call(Hash.SET_BLIP_COLOUR, blip, blipColour); // Set blip colour
            Function.Call(Hash.SET_BLIP_ROUTE_COLOUR, blip, blipColour);
            return blip;
        }
        /* Ped blip spawner */
        public static Blip SpawnPedBlip(Ped pedName, BlipSprite sprite, int blipColour, bool isEnemy)
        {
            pedName.AddBlip();
            Function.Call(Hash.SET_BLIP_COLOUR, pedName.CurrentBlip, blipColour); // Set blip colour
            Function.Call(Hash.SET_BLIP_AS_FRIENDLY, pedName.CurrentBlip, isEnemy);
            //pedName.CurrentBlip.ShowRoute = showroute; // GPS true or false NOT USED HERE
            //Function.Call(Hash.SET_BLIP_ROUTE_COLOUR, pedName.CurrentBlip, blipColour); // Set the GPS colour
            return pedName.CurrentBlip;
        }
        /* Vehicle blip spawner */
        public static Blip SpawnVehicleBlip(Vehicle vehName, BlipSprite sprite, int blipColour, bool showroute)
        {
            vehName.AddBlip();
            vehName.CurrentBlip.Sprite = sprite; // Type of sprite (icon) to show on map
            vehName.CurrentBlip.ShowRoute = showroute; // GPS true or false
            Function.Call(Hash.SET_BLIP_COLOUR, vehName.CurrentBlip, blipColour); 
            Function.Call(Hash.SET_BLIP_ROUTE_COLOUR, vehName.CurrentBlip, blipColour);
            return vehName.CurrentBlip;
        }
        /* Vehicle spawner */
        public static Vehicle SpawnVehicle(Model vehName, Vector3 location, float heading)
        {
            Vehicle vehicle = World.CreateVehicle(vehName, location, heading);
            vehicle.PlaceOnGround();
            return vehicle;
        }
        /* Set world weather and time */
        public static void setWorld(int hour, int min, int sec, bool pauseClock, string weather)
        {
            Function.Call(Hash.SET_CLOCK_TIME, hour, min, sec); // set the time
            Function.Call(Hash.PAUSE_CLOCK, pauseClock); // Should the clock be paused true/false
            Function.Call(Hash.SET_WEATHER_TYPE_NOW, weather); // set the weather
        }
        /* Fade mission in and out */
        public static void missionFade(string direction, int time)
        {
            if (direction == "out")
                Game.FadeScreenOut(time);
            Script.Wait(time);
            if (direction == "in")
                Game.FadeScreenIn(time);
        }
        /* Remove wanted level */
        public static void wantedLevel(bool neverWanted)
        {
            if (neverWanted)
                Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player); // Prevent police stars
            Function.Call(Hash.SET_POLICE_IGNORE_PLAYER, Game.Player, neverWanted); // If no police, prevent being noticed
        }
        /* Give weapons to player */
        public static void givePlayerWeapon(WeaponHash weaponHash, int ammo)
        {
            Game.Player.Character.Weapons.Give(weaponHash, ammo, true, true);
        }
    }
}
