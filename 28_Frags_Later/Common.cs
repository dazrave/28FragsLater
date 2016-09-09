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
    }
}
