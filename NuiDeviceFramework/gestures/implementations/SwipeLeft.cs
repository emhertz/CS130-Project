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
Copyright ｩ 2012, Cisco Systems, Inc. and UCLA
*************************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.reflection;

namespace NuiDeviceFramework.gestures.implementations
{
    /* Gesture1 -- Swipe Left
         - Behavior: Swift motion of right hand from right to left
         - Begin: WristRight.X > HipCenter.X by difference INIT_RIGHTHAND_DIST
         - End: WristRight.X < HipCenter.X by difference END_RIGHTHAND_DIST
         - Conditions: WristRight in line with HipCenter in Y Direction
       Stream(s) needed: SkeletonData */

    public class SwipeLeft : SkeletalGesture
    {
        private bool breakLoop = false;
        private TimeSpan frameTrack = new TimeSpan();
        private int frameNumber = 0;

        // Constants
        private static float RIGHTHAND_INIT_DIST = 0.25f; // Right hand extended past right side of hip
        private static float RIGHTHAND_END_DIST = -0.05f; // Right hand crosses over to left side of hip
        private static float RIGHTHAND_MIN_VELOCITY = 0.005f;
        private static float RIGHTHAND_Y_THRESHOLD = 0.3f;

        private static int FRAME_UPDATE_WAIT = 12; // Check every few frames before analyzing



        private float[] hipPosition;

        private float[] rightHandPosition;
        private float[] rightHandLast = new float[3];

        private bool requiredCondition()
        {
            float distanceHipX = rightHandPosition[X] - hipPosition[X];
            float distanceHipY = rightHandPosition[Y] - hipPosition[Y];
            return (distanceHipY < -RIGHTHAND_Y_THRESHOLD || distanceHipY > RIGHTHAND_Y_THRESHOLD);
        }

        private bool isInitial()
        {
            float distanceHipX = rightHandPosition[X] - hipPosition[X];
            return (
                distanceHipX >= RIGHTHAND_INIT_DIST
            );
        }

        private bool isMoveLeft()
        {
            float velocityX = rightHandLast[X] - rightHandPosition[X];
            
            return (
                velocityX > RIGHTHAND_MIN_VELOCITY
            );

        }
        private bool isFinal()
        {
            float distanceHipX = rightHandPosition[X] - hipPosition[X];
            return (
                distanceHipX <= RIGHTHAND_END_DIST
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
                    if (frameNumber == FRAME_UPDATE_WAIT)
                    {
                        frameTrack = tspan;
                        frameNumber = 0;
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
                    rightHandPosition = this.getTrackedPosition((int)NuiJointType.HandRight);
                    hipPosition = this.getTrackedPosition((int)NuiJointType.HipCenter);

                    Console.WriteLine((GestureState)this.state());

                    if (!this.processNextState())
                    {
                        this.gestureDetected = true;
                        breakLoop = true;
                        break;
                    }

                    SkeletalGesture.copyPositions(rightHandLast, rightHandPosition);
                } // for
            } // while
        }

        public SwipeLeft(object d) : base(d)
        {
            device = d;

            this.addTrackedPosition((int)NuiJointType.HandRight);
            this.addTrackedPosition((int)NuiJointType.HipCenter);

            this.setInitialState((int)GestureState.Looking);
            this.setRequiredCheck(new TransitionDelegate(requiredCondition), (int)GestureState.Looking);
            this.addState((int)GestureState.Initial, new TransitionDelegate(isInitial), true);
            this.addState((int)GestureState.MoveLeft, new TransitionDelegate(isMoveLeft), true);
            this.addState((int)GestureState.Final, new TransitionDelegate(isFinal), true);
        }

        private enum GestureState
        {
            Looking, Initial, MoveLeft, Final
        }
    }
}
