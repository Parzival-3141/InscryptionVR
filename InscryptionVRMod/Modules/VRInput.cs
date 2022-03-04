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
        internal static CustomController CreateCustomController()
        {
            var hcm = new HardwareControllerMap_Game
            (
                "VRControllers",
                new ControllerElementIdentifier[]
                {
                    new ControllerElementIdentifier()
                },
                new int[0],
                new int[0],
                new AxisCalibrationData[]
                {

                },
                new AxisRange[]
                {

                },
                new HardwareAxisInfo[]
                {

                },
                new HardwareButtonInfo[]
                {

                },
                new HardwareJoystickMap.CompoundElement[]
                {

                }
            );


            return null;
        }
    }
}
