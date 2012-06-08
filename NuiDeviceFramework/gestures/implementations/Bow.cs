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
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.reflection;

namespace NuiDeviceFramework.gestures.implementations
{
    /* Gesture3 -- Bow
         - Behavior: Bowing motion of head and upper body
         - Begin: WristRight.X > HipCenter.X by difference INIT_RIGHTHAND_DIST
         - End: WristRight.X < HipCenter.X by difference END_RIGHTHAND_DIST
         - Conditions: WristRight in line with HipCenter in Y Direction
       Stream(s) needed: SkeletonData */

    public class Bow : SkeletalGesture
    {
        private bool breakLoop = false;

        private TimeSpan frameTrack = new TimeSpan();
        private int frameNumber = 0;

        private const int FRAME_UPDATE_WAIT = 6; // Check every few frames before analyzing


        // Constants
        private const float HEAD_INIT_X_LO = -1.0f; // Right hand extended past right side of hip
        private const float HEAD_INIT_X_HI = 1.0f; // Right hand extended past right side of hip
        private const float HEAD_INIT_Y_LO = 0.1f; // Right hand extended past right side of hip
        private const float HEAD_INIT_Y_HI = 0.8f; // Right hand extended past right side of hip

        private const float HEAD_RATIO_THRESHOLD = 0.5f;

        private float[] headLast = new float[3];

        private float[] headInitial;
        private float[] headPosition;

        private enum GestureState
        {
            None, Ready, Bowing, Final
        }

        private bool isReady()
        {
            return (
                headPosition[X] < HEAD_INIT_X_HI && headPosition[X] > HEAD_INIT_X_LO &&
                headPosition[Y] < HEAD_INIT_Y_HI && headPosition[Y] > HEAD_INIT_Y_LO
                );
        }

        private bool isBowing()
        {

            float[] velocity = new float[3];
            velocity[X] = headPosition[X] - headLast[X];
            velocity[Y] = headPosition[Y] - headLast[Y];
            velocity[Z] = headPosition[Z] - headLast[Z];
            return (
                    velocity[Y] < 0.0f &&
                    velocity[Z] < 0.0f
                );

        }

        private bool isFinal()
        {

            return (
                headPosition[Y] < (headInitial[Y] * HEAD_RATIO_THRESHOLD)
                );

        }

        public override void Start()
        {
            while (!breakLoop)
            {
                TimeSpan tspan = (TimeSpan)ReflectionUtilities.InvokeProperty(device, "SkeletonLastModified");
                if (frameTrack != tspan)
                {
                    frameNumber++;
                    if (frameNumber >= FRAME_UPDATE_WAIT)
                    {
                        frameNumber = 0;
                        frameTrack = tspan;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                if (this.getSkeleton() && this.getJoints())
                {
                    this.loadTrackedPositions();
                    headPosition = this.getTrackedPosition((int)NuiJointType.Head);

                    if ((GestureState)this.state() == GestureState.Ready)
                    {
                        if (headInitial == null)
                        {
                            headInitial = new float[3];
                            SkeletalGesture.copyPositions(headInitial, headPosition);
                        }
                    }

                    if (!this.processNextState())
                    {
                        this.gestureDetected = true;
                        breakLoop = true;
                        break;
                    }

                    Console.WriteLine((GestureState)this.state());
                    SkeletalGesture.copyPositions(headLast, headPosition);

                }
                else // skeleton or joints not found
                {
                    this.resetState();
                }



            } // while
        }

        public Bow(object d)
            : base(d)
        {
            this.addTrackedPosition((int)NuiJointType.Head);
            this.setInitialState((int)GestureState.None);
            this.addState((int)GestureState.Ready, new TransitionDelegate(isReady), true);
            this.addState((int)GestureState.Bowing, new TransitionDelegate(isBowing), true);
            this.addState((int)GestureState.Final, new TransitionDelegate(isFinal), true);
            headInitial = null;

        }

    }
}
