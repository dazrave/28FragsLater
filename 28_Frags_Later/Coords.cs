using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using GTA;
using GTA.Math;
using GTA.Native;

public struct coordStruct
{
    public Vector3 Location;
    public string StreetName;
}

namespace _28_Frags_Later
{
    public class cCoordinateCollector : Script
    {
        private int iCurrentGameTime;
        private int iElapsedGameTime;
        private int iLastGameTime;

        private Dictionary<string, coordStruct> coordinateDictionary = new Dictionary<string, coordStruct>();
        private int coordCounter = 0;

        private ScriptSettings ModSettings;
        const string modSettingsFile = "scripts\\CoordinateCollector.ini";
        private Keys keyCoordsKey;
        private Keys keySaveFileKey;

        public cCoordinateCollector()
        {
            this.Tick += onTick;
            this.KeyUp += onKeyUp;
            Interval = 0;

            Initialise();
        }

        // Initialise the mod here
        private void Initialise()
        {
            LoadSettingsFile();
            UI.Notify("~y~Coordinate Collector: ~w~v1");
        }

        private void LoadSettingsFile()
        {
            bool loadSettingsSuccess = true;

            try
            {
                ModSettings = ScriptSettings.Load(modSettingsFile);
            }
            catch (Exception ex)
            {
                loadSettingsSuccess = false;
                UI.Notify(ex.Message);
            }

            /*if (loadSettingsSuccess)
            {
                keyCoordsKey = ModSettings.GetValue<Keys>("KEYS", "COORDKEY", Keys.F9);
                keySaveFileKey = ModSettings.GetValue<Keys>("KEYS", "SAVEKEY", Keys.F10);
            }
            else
            {*/
                keyCoordsKey = Keys.I;
                keySaveFileKey = Keys.O;
            //}
        }

        // OnTick event fires every frame
        private void onTick(object sender, EventArgs e)
        {
            iCurrentGameTime = Game.GameTime;
            iElapsedGameTime = iCurrentGameTime - iLastGameTime;
            iLastGameTime = iCurrentGameTime;

            DisableControls();
        }

        // Disable or enable as required, givees access to all function keys without passing control to the game afterwards. F4, F11 and F12 seem to be available already
        private void DisableControls()
        {
            // Blocks F9 and F10 from being passed to the game.
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)keyCoordsKey);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)keySaveFileKey);
        }

        // Process key presses using key up rather than key down.
        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyCoordsKey)
            {
                // Get the player position
                Vector3 _coord = Game.Player.Character.Position;

                // Create a new coordinate struct
                coordStruct _cs = new coordStruct();
                _cs.Location = _coord;
                _cs.StreetName = World.GetStreetName(_coord);

                // Add the struct to the dictionary using the counter as the key. Values are stored in the dictionary as ("Key", coordStruct)
                coordinateDictionary.Add(coordCounter.ToString(), _cs);
                UI.Notify("Coord: " + coordCounter.ToString() + " added.");
                UI.Notify(coordCounter.ToString() + ": Position: " + _cs.Location.ToString());
                UI.Notify(coordCounter.ToString() + ": Street: " + _cs.StreetName);

                coordCounter++;
            }

            if (e.KeyCode == keySaveFileKey)
            {
                UI.Notify("Writing coords data...");
                string path_pre = "scripts\\Coordinate-Collection";
                string path_ext = ".txt";
                int counter = 0;
                int filecounter = 1;
                string fullpath = path_pre + filecounter.ToString() + path_ext;

                // This checks to see if the file exists and if it does, it increments the counter until it gets a file that doesn't
                while (File.Exists(fullpath))
                {
                    filecounter++;
                    fullpath = path_pre + filecounter.ToString() + path_ext;
                }

                // Streamwriters allow you to write information to disk, just like any normal C# application
                using (StreamWriter sw = File.CreateText(fullpath))
                {
                    // Get the value property of each coord struct in turn
                    foreach (coordStruct _coStruct in coordinateDictionary.Values)
                    {
                        // Build the string information
                        string _header = String.Format("", counter);
                        string _vector3 = "new[] {" + _coStruct.Location.X.ToString() + ", " + _coStruct.Location.Y.ToString() + ", " + _coStruct.Location.Z.ToString() + "}, ";
                        string _streetName = _coStruct.StreetName;
                        string bstring = _header + _vector3 + " // {0} - " + _streetName;
                        sw.WriteLine(bstring);
                        counter++;
                    }
                }
                UI.ShowSubtitle("Writing File: " + fullpath + " complete.");
            }
        }
    }
}
