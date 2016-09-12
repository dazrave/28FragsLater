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
    class Day28
    {
        // Start Day 28 Stage 1
        public static void Day28Stage1()
        {
            // Fade out screen (unless in debug)
            if (!Main.debugMode)
                Common.missionFade("out", 2000);
            // Reset world and set day/stage values
            Common.softReset();
            Main.currentDay = 28;
            Main.currentStage = 1;
            Common.setWorld(07, 00, 00, true, "FOGGY");
            World.SetBlackout(true);
            if (!Main.debugMode)
            {
                // Remove all player weapons (unless in debug)
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, Game.Player.Character, true);
                // Spawn player next to next to hospital bed (unless in debug)
                Game.Player.Character.Position = new Vector3(351.878f, -593.013f, 43.315f);
            }

            // Prevent game from spawning vehicles
            Function.Call(Hash.SET_VEHICLE_POPULATION_BUDGET, false);
            // Prevent game from spawning peds
            Function.Call(Hash.SET_PED_POPULATION_BUDGET, false);
            // Clear area of current peds and vehicles
            Function.Call(Hash.CLEAR_AREA_OF_PEDS, 0, 0, 0, 10000, true);
            Function.Call(Hash.CLEAR_AREA_OF_VEHICLES, 0, 0, 0, 10000, true, true, true, true, true);

            // TODO : Remove city audio ambience

            // Fade in screen (unless in debug)
            if (!Main.debugMode)
                Common.missionFade("in", 2000);

            Function.Call(Hash.SET_PED_IS_DRUNK, Game.Player.Character, true);

            Common.runDebug();

        }
    }
}
