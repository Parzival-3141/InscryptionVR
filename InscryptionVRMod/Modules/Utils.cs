using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;
using Valve.VR;

namespace InscryptionVR.Modules
{
    public static class Utils
    {
        public static void LogArray<T>(this ManualLogSource logger, string logTitle, IEnumerable<T> array, bool logIndex = false, LogLevel level = LogLevel.Info, Func<T, string> itemFormat = null)
        {
            logger.Log(level, $"--- {logTitle} ---");

            int num = 0;
            foreach (T item in array)
            {
                string n = logIndex ? $"[{num++}] " : "";

                if (itemFormat is null)
                    logger.Log(level, n + item.ToString());
                else
                    logger.Log(level, n + itemFormat.Invoke(item));
            }

            logger.Log(level, "--- end ---");
        }

        public static bool GetVRInput(this Button button, Func<SteamVR_Action_Boolean_Source, bool> checkMethod)
        {
            switch (button)
            {
                case Button.Select:
                    return checkMethod.Invoke(SteamVR_Actions._default.TriggerClick[VRController.PrimaryHand.InputSource]);

                case Button.AltSelect:
                    return checkMethod.Invoke(SteamVR_Actions._default.TriggerClick[VRController.SecondaryHand.InputSource]);

                case Button.LookUp:
                case Button.DirUp:
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadUp[SteamVR_Input_Sources.LeftHand]);

                case Button.LookDown:
                case Button.DirDown:
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadDown[SteamVR_Input_Sources.LeftHand]);

                case Button.LookLeft:
                case Button.DirLeft:
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadLeft[SteamVR_Input_Sources.LeftHand]);

                case Button.LookRight:
                case Button.DirRight:
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadRight[SteamVR_Input_Sources.LeftHand]);

                case Button.AltMenu:
                case Button.Menu:
                    return checkMethod.Invoke(SteamVR_Actions._default.BClick[SteamVR_Input_Sources.RightHand]);

                default:
                    return false;
            }
        }
    }
}
