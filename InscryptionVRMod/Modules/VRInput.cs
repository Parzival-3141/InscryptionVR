using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rewired;
using Rewired.Data;
using Rewired.Data.Mapping;

namespace InscryptionVR.Modules
{
    /*
     *  @Refactor:
     *  Should add vr input to Rewired, so I don't have to patch InputButtons.
     *  I think it might be more work than it's worth though.
     *  
     *  ror2vr uses this but through a publicizer maybe?
     */

    internal static class VRInput
    {
        private static ControllerElementIdentifier NewCEI(int id, string name, string positiveName, string negativeName, ControllerElementType elementType, bool isMappableOnPlatform)
        {
            var asm = Assembly.LoadFrom(BepInEx.Paths.ManagedPath + "Rewired_Core.dll");
            Type cei = asm.GetType("ControllerElementIdentifier");

            ConstructorInfo ceiConstructor = cei.GetConstructor(new Type[]
            {
                typeof(int),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(ControllerElementType),
                typeof(bool)
            });

            return (ControllerElementIdentifier)ceiConstructor.Invoke(new object[]
            {
                id,
                name,
                positiveName,
                negativeName,
                elementType,
                isMappableOnPlatform
            });
        }

        internal static CustomController CreateCustomController()
        {
            //  Construct hcmap using reflection
            var asm = Assembly.LoadFrom(BepInEx.Paths.ManagedPath + "Rewired_Core.dll");
            Type hcm = asm.GetType("HardwareControllerMap_Game");

            ConstructorInfo hcmConstructor = hcm.GetConstructor(new Type[] 
            { 
                typeof(string),
                typeof(ControllerElementIdentifier[]), 
                typeof(int[]), 
                typeof(int[]),
                typeof(AxisCalibrationData[]),
                typeof(AxisRange[]),
                typeof(HardwareAxisInfo[]),
                typeof(HardwareButtonInfo[]),
                typeof(HardwareJoystickMap.CompoundElement[])
            });

            object hcMap = hcmConstructor.Invoke(new object[]
            {
                "VRControllers",
                new ControllerElementIdentifier[]
                {
                    //NewCEI(0,)
                }
            });



            return null;
        }
    }
}
