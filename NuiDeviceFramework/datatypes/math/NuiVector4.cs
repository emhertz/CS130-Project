using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuiDeviceFramework.datatypes
{
    public class NuiVector4
    {
        private float w, x, y, z;

        public float W 
        { 
            get
            {
                return w;
            }
            set
            {
                this.w = value;
            }
        }

        public float X 
        {
            get
            {
                return x;
            }
            set
            {
                this.x = value;
            }
        }

        public float Y 
        {
            get
            {
                return y;
            }
            set
            {
                this.y = value;
            }
        }

        public float Z 
        {
            get
            {
                return z;
            }
            set
            {
                this.z = value;
            }
        }

        public bool Equals(NuiVector4 aMatrix)
        {
            // your code here
            return false;
        }

        public override bool Equals(object anObject)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool op_Equality(NuiVector4 mat1, NuiVector4 mat2)
        {
            return false;
        }

        public static bool op_Inequality(NuiVector4 mat1, NuiVector4 mat2)
        {
            return false;
        }
    }
}
