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
