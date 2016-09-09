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
            Common.setWorld(08, 00, 00, true, "CLEAR");
            World.SetBlackout(true);
            if (!Main.debugMode)
            {
                // Remove all player weapons (unless in debug)
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, Game.Player.Character, true);
                // Spawn player next to next to hospital bed (unless in debug)
                Game.Player.Character.Position = new Vector3(351.878f, -593.013f, 43.315f);
            }
            // TODO : Remove city audio ambience
            // TODO : Remove all peds
            // TODO : Remove all vehicles

            // Fade in screen (unless in debug)
            if (!Main.debugMode)
                Common.missionFade("in", 2000);

        }
    }
}
