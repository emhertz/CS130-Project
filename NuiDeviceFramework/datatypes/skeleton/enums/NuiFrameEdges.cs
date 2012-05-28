using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NuiDeviceFramework.datatypes
{
    [FlagsAttribute]
    [ComVisibleAttribute(false)]
    public enum NuiFrameEdges
    {
        Bottom, Left, None, Right, Top
    }
}
