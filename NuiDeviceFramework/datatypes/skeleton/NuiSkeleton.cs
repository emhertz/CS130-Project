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
    public class NuiSkeleton
    {
        private NuiBoneOrientationCollection boneOrientations;
        private NuiFrameEdges clippedEdges;
        private NuiSkeletonPoint position;
        private int trackingId;
        private NuiSkeletonTrackingState trackingState;
        private NuiJointCollection joints;

        public NuiBoneOrientationCollection BoneOrientations { get { return this.boneOrientations; } }
        public NuiFrameEdges ClippedEdges { get { return this.clippedEdges; } set { this.clippedEdges = value; } }
        public NuiSkeletonPoint Position { get { return this.position; } set { this.position = value; } }
        public int TrackingId { get { return this.trackingId; } set { this.trackingId = value; } }
        public NuiSkeletonTrackingState TrackingState { get { return this.trackingState; } set { this.trackingState = value; } }
        public NuiJointCollection Joints { get { return this.joints; } set { this.joints = value; } }

        public NuiSkeleton(
            NuiBoneOrientationCollection nboc,
            NuiFrameEdges ce,
            NuiSkeletonPoint p,
            int tid,
            NuiSkeletonTrackingState ts,
            NuiJointCollection j)
        {
            this.boneOrientations = nboc;
            this.ClippedEdges = ce;
            this.Position = p;
            this.TrackingId = tid;
            this.TrackingState = ts;
            this.Joints = j;
        }
    }
}
