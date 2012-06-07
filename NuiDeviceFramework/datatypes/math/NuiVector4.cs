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
