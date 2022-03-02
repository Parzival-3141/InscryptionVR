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

                case Button.DirUp or Button.LookUp:
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadUp[SteamVR_Input_Sources.LeftHand]);

                case Button.DirDown or Button.LookDown:
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadDown[SteamVR_Input_Sources.LeftHand]);

                case Button.DirLeft or Button.LookLeft:
                    if (button == Button.DirLeft && DiskCardGame.FirstPersonController.Instance.MoveLocked)
                        return false;
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadLeft[SteamVR_Input_Sources.LeftHand]);

                case Button.DirRight or Button.LookRight:
                    if (button == Button.DirRight && DiskCardGame.FirstPersonController.Instance.MoveLocked)
                        return false;
                    return checkMethod.Invoke(SteamVR_Actions._default.DPadRight[SteamVR_Input_Sources.LeftHand]);

                case Button.Menu or Button.AltMenu:
                    return checkMethod.Invoke(SteamVR_Actions._default.BClick[SteamVR_Input_Sources.RightHand]);

                default:
                    return false;
            }
        }
    }
}
