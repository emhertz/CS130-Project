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

        public bool Equals(NuiVector4 aVector)
        {
            return this.X == aVector.X
                && this.Y == aVector.Y
                && this.Z == aVector.Z
                && this.W == aVector.W;
        }

        public override bool Equals(object anObject)
        {
            if (anObject is NuiVector4)
            {
                return this.Equals((NuiVector4)anObject);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool op_Equality(NuiVector4 vec1, NuiVector4 vec2)
        {
            return vec1.Equals(vec2);
        }

        public static bool op_Inequality(NuiVector4 vec1, NuiVector4 vec2)
        {
            return ! vec1.Equals(vec2);
        }
    }
}
