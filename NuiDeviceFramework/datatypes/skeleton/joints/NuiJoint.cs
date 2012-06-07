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
using NuiDeviceFramework.datatypes.skeleton.enums;

namespace NuiDeviceFramework.datatypes
{
    public struct NuiJoint
    {
        private NuiJointType jointType;
        private NuiSkeletonPoint position;
        private NuiJointTrackingState trackingState;

        public NuiJointType JointType { get { return jointType; } }
        public NuiSkeletonPoint Position { get { return this.position; } set { this.position = value; } }
        public NuiJointTrackingState TrackingState { get { return this.trackingState; } set { this.trackingState = value; } }

        public NuiJoint(NuiJointType jt, NuiSkeletonPoint p, NuiJointTrackingState ts)
        {
            position = p;
            trackingState = ts;
            jointType = jt;
        }

        public bool Equals(NuiJoint joint)
        {
            return false;
        }

        public override bool Equals(object joint)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool op_Equality(NuiJoint joint1, NuiJoint joint2)
        {
            return false;
        }

        public static bool op_Inequality(NuiJoint joint1, NuiJoint joint2)
        {
            return false;
        }
    }
}
