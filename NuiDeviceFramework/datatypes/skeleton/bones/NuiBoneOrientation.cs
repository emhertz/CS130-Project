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
    public class NuiBoneOrientation
    {
        private NuiJointType endJoint;
        private NuiJointType startJoint;

        private NuiBoneRotation absoluteRotation;
        private NuiBoneRotation hierarchicalRotation;

        public NuiBoneRotation AbsoluteRotation { get { return this.absoluteRotation; } set { this.absoluteRotation = value; } }
        public NuiJointType EndJoint { get { return this.endJoint; } }
        public NuiBoneRotation HierarchicalRotation { get { return this.hierarchicalRotation; } set { this.hierarchicalRotation = value; } }
        public NuiJointType StartJoint { get { return this.startJoint; } }

        public NuiBoneOrientation(NuiJointType s, NuiJointType e, NuiBoneRotation a, NuiBoneRotation h)
        {
            startJoint = s;
            endJoint = e;
            absoluteRotation = a;
            hierarchicalRotation = h;
        }
    }
}
