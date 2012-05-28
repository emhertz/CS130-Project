using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NuiDeviceFramework.datatypes
{
    [ComVisibleAttribute(false)]
    public class NuiSkeletonPoint
    {
        private float x, y, z;

        public float X { get { return this.x; } set { this.x = value; } }
        public float Y { get { return this.y; } set { this.y = value; } }
        public float Z { get { return this.z; } set { this.z = value; } }

        public NuiSkeletonPoint(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool Equals(NuiSkeletonPoint nsp)
        {
            return false;
        }

        public override bool Equals(Object nsp)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool op_Equality(NuiSkeletonPoint nsp1, NuiSkeletonPoint nsp2)
        {
            return false;
        }

        public static bool op_Inequality(NuiSkeletonPoint nsp1, NuiSkeletonPoint nsp2)
        {
            return false;
        }
    }
}
