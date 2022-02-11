using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace InscryptionVR.Modules
{
    internal static class Resources
    {
        private static Assembly CurrentAsm => Assembly.GetExecutingAssembly();
        
        public static GameObject VRRigPrefab { get; private set; }
        public static Shader HandDitherShader { get; private set; }

        public static void Init()
        {
            VRPlugin.Logger.LogInfo("Loading AssetBundle");
            VRPlugin.Logger.LogMessage("Ignore the script reference warnings");
            AssetBundle dataBundle = GetAssetBundle("data");
            if(dataBundle == null)
            {
                VRPlugin.Logger.LogError("Could not load AssetBundle!");
                return;
            }

            VRRigPrefab = dataBundle.LoadAssetWithHF<GameObject>("Assets/AssetBundleData/VR/VRRig.prefab");
            HandDitherShader = dataBundle.LoadAssetWithHF<Shader>("Assets/AssetBundleData/VR/Art/Shaders/HandDither.shader");

            var right = VRController.SetupHand(Hand.Right, VRRigPrefab);
            var left  = VRController.SetupHand(Hand.Left, VRRigPrefab);
            VRController.SetupVRRig(VRRigPrefab, left, right);

            //  Unload AssetBundle(?)
            dataBundle.Unload(false);
        }


        private static AssetBundle GetAssetBundle(string name)
        {
            MemoryStream memoryStream;
            using (Stream stream = CurrentAsm.GetManifestResourceStream("InscryptionVR.Resources." + name))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, buffer.Length);
            }
            return AssetBundle.LoadFromMemory(memoryStream.ToArray());
        }

        private static T LoadAssetWithHF<T>(this AssetBundle bundle, string path) where T : UnityEngine.Object
        {
            T asset = bundle.LoadAsset<T>(path);
            asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return asset;
        }
    }
}
