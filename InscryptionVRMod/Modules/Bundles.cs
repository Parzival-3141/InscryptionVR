using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using UnityEngine;
using Valve.VR;
using System.Linq;

namespace InscryptionVR.Modules
{
    internal static class Bundles
    {
        public static GameObject VRRigPrefab { get; private set; }
        public static Shader HandDitherShader { get; private set; }

        private static AssetBundle dataBundle;

        public static void Init()
        {
            VRPlugin.Logger.LogMessage("Ignore the script reference warnings");
            dataBundle = EmebeddedAssetBundle.LoadFromAssembly(Assembly.GetExecutingAssembly(), "InscryptionVRMod.Resources.rigdata.bundle");
            if(dataBundle == null)
            {
                VRPlugin.Logger.LogError("InscryptionVR bundle is null! Did you forget to compile the bundle into the dll?");
                return;
            }

            VRRigPrefab = dataBundle.LoadAssetWithHF<GameObject>("Assets/AssetBundleData/VR/VRRig.prefab");
            HandDitherShader = dataBundle.LoadAssetWithHF<Shader>("Assets/AssetBundleData/VR/Art/Shaders/HandDither.shader");

            var right = VRController.SetupHand(Hand.Right, VRRigPrefab);
            var left  = VRController.SetupHand(Hand.Left, VRRigPrefab);
            VRController.SetupVRRig(VRRigPrefab, left, right);

            dataBundle.Unload(false);
        }

        private static T LoadAssetWithHF<T>(this AssetBundle bundle, string path) where T : UnityEngine.Object
        {
            T asset = bundle.LoadAsset<T>(path);
            asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return asset;
        }

        // Credit to Lakatrazz
        public static class EmebeddedAssetBundle
        {
            public static AssetBundle LoadFromAssembly(Assembly assembly, string name)
            {
                if (assembly.GetManifestResourceNames().Contains(name))
                {
                    VRPlugin.Logger.LogInfo("Loading embedded bundle " + name + "...");
                    byte[] array;
                    using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            manifestResourceStream.CopyTo(memoryStream);
                            array = memoryStream.ToArray();
                        }
                    }
                    VRPlugin.Logger.LogInfo("Done!");
                    return AssetBundle.LoadFromMemory(array);
                }

                VRPlugin.Logger.LogWarning("Missing embedded bundle " + name);
                return null;
            }
        }
    }
}
