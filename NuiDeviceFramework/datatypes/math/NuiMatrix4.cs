using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuiDeviceFramework.datatypes
{
    public class NuiMatrix4
    {
        private float[,] data = new float[4,4];

        public float M11 { get { return data[0, 0]; } set { data[0, 0] = value; } }
        public float M12 { get { return data[0, 1]; } set { data[0, 1] = value; } }
        public float M13 { get { return data[0, 2]; } set { data[0, 2] = value; } }
        public float M14 { get { return data[0, 3]; } set { data[0, 3] = value; } }
        public float M21 { get { return data[1, 0]; } set { data[1, 0] = value; } }
        public float M22 { get { return data[1, 1]; } set { data[1, 1] = value; } }
        public float M23 { get { return data[1, 2]; } set { data[1, 2] = value; } }
        public float M24 { get { return data[1, 3]; } set { data[1, 3] = value; } }
        public float M31 { get { return data[2, 0]; } set { data[2, 0] = value; } }
        public float M32 { get { return data[2, 1]; } set { data[2, 1] = value; } }
        public float M33 { get { return data[2, 2]; } set { data[2, 2] = value; } }
        public float M34 { get { return data[2, 3]; } set { data[2, 3] = value; } }
        public float M41 { get { return data[3, 0]; } set { data[3, 0] = value; } }
        public float M42 { get { return data[3, 1]; } set { data[3, 1] = value; } }
        public float M43 { get { return data[3, 2]; } set { data[3, 2] = value; } }
        public float M44 { get { return data[3, 3]; } set { data[3, 3] = value; } }

        public static NuiMatrix4 Identity
        {
            get
            {
                NuiMatrix4 result = new NuiMatrix4();
                result.M11 = 1.0f;
                result.M22 = 1.0f;
                result.M33 = 1.0f;
                result.M44 = 1.0f;
                return result;
            }
        }

        public bool Equals(NuiMatrix4 aMatrix)
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

        public static bool op_Equality(NuiMatrix4 mat1, NuiMatrix4 mat2)
        {
            return false;
        }

        public static bool op_Inequality(NuiMatrix4 mat1, NuiMatrix4 mat2)
        {
            return false;
        }
    }
}
