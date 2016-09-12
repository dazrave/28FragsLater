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
    class Day3
    {
        public static Ped gateGuard1;

        // Start Day 3 Stage 1
        public static void Day3Stage1()
        {
            /* ------------------------------------------ TESTING --------------------- */

            //Game.Player.Character.Position = new Vector3(3390.720f, 3695.381f, 37.211f);

            Common.setWorld(01, 00, 00, true, "FOGGY");

            // Spawn security
            gateGuard1 = World.CreatePed(PedHash.Security01SMM, new Vector3(3427.052f, 3762.446f, 30.83205f));
            // Give gateGuard1 a gun that's holstered
            gateGuard1.Weapons.Give(WeaponHash.CombatPistol, 500, false, true);
            Function.Call(Hash.TASK_TURN_PED_TO_FACE_ENTITY, gateGuard1, Game.Player.Character, -1);
            Main.Wait(10000);
            gateGuard1.Task.GoTo(new Vector3(3429.082f, 3759.931f, 30.49263f));
            Main.Wait(4000);
            Function.Call(Hash.TASK_TURN_PED_TO_FACE_ENTITY, gateGuard1, Game.Player.Character, -1);
            Main.Wait(1000);
            gateGuard1.Task.HandsUp(3000);
            Main.Wait(3000);
            gateGuard1.Task.GoTo(new Vector3(3428.536f, 3761.504f, 30.64255f));
            Main.Wait(2000);
            Function.Call(Hash.TASK_TURN_PED_TO_FACE_ENTITY, gateGuard1, Game.Player.Character, -1);
            // Give gateGuard1 a clipboard
            Main.Wait(2000);
            Function.Call(Hash.CLEAR_PED_TASKS, gateGuard1);
            Main.Wait(2000);
            Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, gateGuard1, "WORLD_HUMAN_CLIPBOARD", -1, false);
            Main.Wait(10000);
            

            // WORLD_HUMAN_SECURITY_SHINE_TORCH
            // WORLD_HUMAN_GUARD_PATROL
            // WORLD_HUMAN_GUARD_STAND

            /* ------------------------------------------ TESTING END --------------------- */


            Common.runDebug();
        }
    }
}
