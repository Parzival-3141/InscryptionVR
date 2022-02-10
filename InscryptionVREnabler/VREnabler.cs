using AssetsTools.NET;
using AssetsTools.NET.Extra;
using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SR = System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace InscryptionVREnabler
{
    //  Original class by MrPurple and DrBibop
    //  Adapted by Parzival and Windows10CE

    /// <summary>
    /// A patcher which runs ahead of UnityPlayer to enable VR in the Global Game Manager.
    /// </summary>
    public static class VREnabler
    {
        internal static string VRPatcherPath => Path.GetDirectoryName(SR.Assembly.GetExecutingAssembly().Location);
        internal static string ManagedPath => Paths.ManagedPath;
        internal static string PluginsPath => Path.Combine(ManagedPath, "../Plugins");
        internal static string SteamVRPath => Path.Combine(ManagedPath, "../StreamingAssets/SteamVR");

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("VREnabler");
        private static bool vrEnabled;

        /// <summary>
        /// Called from BepInEx while patching, our entry point for patching.
        /// Do not change the method name as it is identified by BepInEx. Method must remain public.
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static void Initialize()
        {
            #region Patch GGM
            
            vrEnabled = Environment.GetCommandLineArgs().Contains("OpenVR");
            if (!vrEnabled)
            {
                Logger.LogWarning("Launch parameter \"-vrmode\" not set to OpenVR, unloading VR patches!");

                VRPatchGGM(true);
                return;
            }
            else if (!VRPatchGGM())
                return;

            #endregion

            #region Copy Plugins

            Logger.LogInfo("Checking for VR plugins...");

            string[] plugins = new string[]
            {
                "openvr_api.dll",
                "OVRPlugin.dll",
            };

            string[] managedLibraries = new string[]
            {
                "SteamVR.dll",
                "SteamVR_Actions.dll"
            };

            if (CopyFiles(PluginsPath, plugins, "Plugins.") || CopyFiles(ManagedPath, managedLibraries, "Plugins."))
                Logger.LogInfo("Successfully copied VR plugins!");
            else
                Logger.LogInfo("VR plugins already present");

            #endregion

            #region Copy Binds

            Logger.LogInfo("Checking for binding files...");

            if (!Directory.Exists(SteamVRPath))
            {
                try
                {
                    Directory.CreateDirectory(SteamVRPath);
                }
                catch (Exception e)
                {
                    Logger.LogError("Could not create SteamVR folder in StreamingAssets: " + e.Message);
                    Logger.LogError(e.StackTrace);
                    return;
                }
            }

            string[] bindingFiles = new string[]
            {
                "actions.json",
                //"binding_holographic_hmd.json",
                //"binding_index_hmd.json",
                //"binding_rift.json",
                //"binding_vive.json",
                //"binding_vive_cosmos.json",
                //"binding_vive_pro.json",
                //"binding_vive_tracker_camera.json",
                //"bindings_holographic_controller.json",
                "bindings_knuckles.json",
                //"bindings_logitech_stylus.json",
                //"bindings_oculus_touch.json",
                //"bindings_vive_controller.json",
                //"bindings_vive_cosmos_controller.json"
            };

            if (CopyFiles(SteamVRPath, bindingFiles, "Binds.", true))
                Logger.LogInfo("Successfully copied binding files!");
            else
                Logger.LogInfo("Binding files already present");

            #endregion

        }

        /// <param name="unPatch">Set <see langword="true"/> to remove VR patch.</param>
        /// <returns><see langword="true"/> if patching/unpatching is successful.</returns>
        private static bool VRPatchGGM(bool unPatch = false)
        {
            var am = new AssetsManager();

            //  Load ggm asset file
            string path = Path.Combine(ManagedPath, "../globalgamemanagers");
            AssetsFileInstance ggmInst = am.LoadAssetsFile(path, false);

            try
            {
                //  Load class database from .tpk package. classDB contains type info for the game
                am.LoadClassPackage(Path.Combine(VRPatcherPath, "classdata.tpk"));
                am.LoadClassDatabaseFromPackage(ggmInst.file.typeTree.unityVersion);

                //  Get assetInfo for BuildSettings and get baseField (BuildSettings data) 
                //  A bit hacky, but equivalent to expected result of ggmInst.table.GetAssetInfo("BuildSettings")
                AssetFileInfoEx buildSettings = ggmInst.table.GetAssetsOfType((int)AssetClassID.BuildSettings)[0];
                AssetTypeValueField buildSettingsBase = am.GetTypeInstance(ggmInst, buildSettings).GetBaseField();

                Logger.LogArray(buildSettingsBase.GetName(), buildSettingsBase.children, true, LogLevel.Debug, (T) => $"{T.GetName()} | {T.GetFieldType()}");

                //  Create new array for enabledVRDevices
                AssetTypeValueField vrDevicesArray = buildSettingsBase.Get("enabledVRDevices").Get("Array");

                Logger.LogArray("enabledVRDevices Before", vrDevicesArray.children, true, LogLevel.Debug, (T) => $"{T.GetValue().AsString()}");

                AssetTypeValueField defaultVRDeviceField = ValueBuilder.DefaultValueFieldFromArrayTemplate(vrDevicesArray);

                var vrDevice = unPatch ? "None" : "OpenVR";
                defaultVRDeviceField.GetValue().Set(vrDevice);

                var devices = new AssetTypeValueField[] { defaultVRDeviceField };
                vrDevicesArray.SetChildrenList(devices);

                Logger.LogMessage(unPatch ? "Unpatching GGM" : "Patching GGM");

                //  Create AssetsReplacer and overwrite enabledVRDevices
                var repl = new AssetsReplacerFromMemory(0, buildSettings.index, (int)buildSettings.curFileType, 0xffff, buildSettingsBase.WriteToByteArray());
                using(var memoryStream = new MemoryStream())
                {
                    using (AssetsFileWriter writer = new AssetsFileWriter(memoryStream))
                    {
                        ggmInst.file.Write(writer, 0, new List<AssetsReplacer> { repl }, 0);
                    }
                }
                
                Logger.LogArray("enabledVRDevices After", vrDevicesArray.children, true, LogLevel.Debug, (T) => $"{T.GetValue().AsString()}");

                am.UnloadAllAssetsFiles();

                Logger.LogInfo($"GGM {(unPatch ? "un" : "")}patching successful!");

                return true;
            }
            catch(Exception e)
            {
                Logger.LogError("Error with VR Patching!");
                Logger.LogError(e.Message + "\n" + e.StackTrace);
                
                am.UnloadAllAssetsFiles();
                return false;
            }
        }

        /// <param name="itemFormat">A method that returns the <see cref="string"/> that should be logged for each item in <paramref name="array"/>.</param>
        private static void LogArray<T>(this ManualLogSource logger, string logTitle, IEnumerable<T> array, bool logIndex = false, LogLevel level = LogLevel.Info, Func<T, string> itemFormat = null)
        {
            logger.Log(level, $"--- {logTitle} ---");

            int num = 0;
            foreach(T item in array)
            {
                string n = logIndex ? $"[{num++}] " : "";

                if (itemFormat is null)
                    logger.Log(level, n + item.ToString());
                else
                    logger.Log(level, n + itemFormat.Invoke(item));
            }
            
            logger.Log(level, "--- end ---");
        }

        private static bool CopyFiles(string destinationPath, string[] fileNames, string embedFolder, bool replaceIfDifferent = false)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(destinationPath);
            FileInfo[] files = directoryInfo.GetFiles();
            var executingAssembly = SR.Assembly.GetExecutingAssembly();
            string asmName = executingAssembly.GetName().Name;

            bool flag = false;
            for (int i = 0; i < fileNames.Length; i++)
            {
                string fileName = fileNames[i];
                if (!Array.Exists<FileInfo>(files, (FileInfo file) => fileName == file.Name))
                {
                    flag = true;
                    using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(asmName + "." + embedFolder + fileName))
                    {
                        using (FileStream fileStream = new FileStream(Path.Combine(directoryInfo.FullName, fileName), FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
                        {
                            Logger.LogInfo("Copying " + fileName);
                            manifestResourceStream.CopyTo(fileStream);
                        }
                    }
                }
                else if (replaceIfDifferent)
                {
                    string resourceFileContent;
                    using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(asmName + "." + embedFolder + fileName))
                    {
                        using (StreamReader reader = new StreamReader(manifestResourceStream))
                        {
                            resourceFileContent = reader.ReadToEnd();
                        }
                    }

                    FileInfo installedFile = files.First(file => file.Name == fileName);
                    string installedFileContent = File.ReadAllText(@installedFile.FullName);

                    if (resourceFileContent != installedFileContent)
                    {
                        flag = true;
                        Logger.LogInfo("Overwriting " + fileName);
                        File.WriteAllText(installedFile.FullName, resourceFileContent);
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// For BepInEx to identify your patcher as a patcher, it must match the patcher contract as outlined in the BepInEx docs:
        /// https://bepinex.github.io/bepinex_docs/v5.0/articles/dev_guide/preloader_patchers.html#patcher-contract
        /// It must contain a list of managed assemblies to patch as a public static <see cref="IEnumerable{T}"/> property named TargetDLLs
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        /// <summary>
        /// For BepInEx to identify your patcher as a patcher, it must match the patcher contract as outlined in the BepInEx docs:
        /// https://bepinex.github.io/bepinex_docs/v5.0/articles/dev_guide/preloader_patchers.html#patcher-contract
        /// It must contain a public static void method named Patch which receives an <see cref="AssemblyDefinition"/> argument,
        /// which patches each of the target assemblies in the TargetDLLs list.
        /// </summary>
        /// <param name="asm"></param>
        [Obsolete("Should not be used!", true)]
        public static void Patch(AssemblyDefinition asm)
        {
            if (!vrEnabled) return;

            //  RaycastInteractable generic patch
            using (var pluginAsm = AssemblyDefinition.ReadAssembly(Directory.GetFiles(Paths.PluginPath, "InscryptionVRMod.dll", SearchOption.AllDirectories)[0]))
            {
                var rfiReplacement = pluginAsm.MainModule.GetType("InscryptionVR.Modules.HarmonyPatches")
                    .Methods.First(x => x.Name == "RaycastForInteractableReplacement");

                var rfi = asm.MainModule.GetType("DiskCardGame.InteractionCursor")
                    .Methods.First(x => x.Name == "RaycastForInteractable");

                var rfiGeneric = rfi.GenericParameters[0];

                rfi.Body = new MethodBody(rfi);
                var il = rfi.Body.GetILProcessor();

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldtoken, rfiGeneric);
                il.Emit(OpCodes.Call, il.Import(typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))));
                il.Emit(OpCodes.Call, asm.MainModule.ImportReference(rfiReplacement));
                il.Emit(OpCodes.Castclass, rfiGeneric);
                il.Emit(OpCodes.Ret);
            }

        }
    }
}
