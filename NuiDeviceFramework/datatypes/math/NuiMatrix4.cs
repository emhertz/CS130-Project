/**************************************************************
 This file is part of Kinect Sensor Architecture Development Project.

   Kinect Sensor Architecture Development Project is free software:
   you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   Kinect Sensor Architecture Development Project is distributed in
   the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Kinect Sensor Architecture Development Project.  If
   not, see <http://www.gnu.org/licenses/>.
**************************************************************/
/**************************************************************
The work was done in joint collaboration with Cisco Systems Inc.
Copyright © 2012, Cisco Systems, Inc. and UCLA
*************************************************************/

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
            return this.M11 == aMatrix.M11
                && this.M12 == aMatrix.M12
                && this.M13 == aMatrix.M13
                && this.M14 == aMatrix.M14
                && this.M21 == aMatrix.M21
                && this.M22 == aMatrix.M22
                && this.M23 == aMatrix.M23
                && this.M24 == aMatrix.M24
                && this.M31 == aMatrix.M31
                && this.M32 == aMatrix.M32
                && this.M33 == aMatrix.M33
                && this.M34 == aMatrix.M34
                && this.M41 == aMatrix.M41
                && this.M42 == aMatrix.M42
                && this.M43 == aMatrix.M43
                && this.M44 == aMatrix.M44;
        }

        public override bool Equals(object anObject)
        {
            if (anObject is NuiMatrix4)
            {
                return this.Equals((NuiMatrix4)anObject);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool op_Equality(NuiMatrix4 mat1, NuiMatrix4 mat2)
        {
            return mat1.Equals(mat2);
        }

        public static bool op_Inequality(NuiMatrix4 mat1, NuiMatrix4 mat2)
        {
            return ! mat1.Equals(mat2);
        }
    }
}
