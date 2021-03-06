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
Copyright � 2012, Cisco Systems, Inc. and UCLA
*************************************************************/

using System;
using System.Collections.Generic;
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.reflection;

namespace NuiDeviceFramework.gestures.implementations
{
    /* Gesture4 -- LowerHands
         - Behavior: Move both hands downwards from shoulder head to chest height
         - Begin: WristRight.X > HipCenter.X by difference INIT_RIGHTHAND_DIST
         - End: WristRight.X < HipCenter.X by difference END_RIGHTHAND_DIST
         - Conditions: WristRight in line with HipCenter in Y Direction
       Stream(s) needed: SkeletonData */

    public class LowerHands : SkeletalGesture
    {
        private bool breakLoop = false;

        private TimeSpan frameTrack = new TimeSpan();
        private int frameNumber = 0;

        private const int FRAME_UPDATE_WAIT = 6; // Check every few frames before analyzing


        // Constants
        
        // Left and right hands start at about head-level
        private const float LEFT_WRIST_INIT_Y_LO = 0.1f; 
        private const float RIGHT_WRIST_INIT_Y_LO = 0.1f;
        private const float LEFT_WRIST_INIT_Y_HI = 0.8f;
        private const float RIGHT_WRIST_INIT_Y_HI = 0.8f;
        
        private const float RIGHTHAND_MIN_VELOCITY = 0.005f;
        
        private const float HANDS_MIN_DISTANCE = 0.5f;
        private const float HANDS_MAX_DISTANCE = 1.0f;

        // Hands lower down past the chest
        private const float HANDS_RATIO_THRESHOLD = 0.5f; 

        private float[] leftWristLast = new float[3];
        private float[] rightWristLast = new float[3];

        private float[] leftWristInitial;
        private float[] rightWristInitial;
        
        private float[] leftWristPosition;
        private float[] rightWristPosition;

        private enum GestureState
        {
            None, Ready, Lowering, Final
        }

        private bool isReady()
        {
            return (
                leftWristPosition[Y] < LEFT_WRIST_INIT_Y_HI && leftWristPosition[Y] > LEFT_WRIST_INIT_Y_LO &&
                rightWristPosition[Y] < RIGHT_WRIST_INIT_Y_HI && rightWristPosition[Y] > RIGHT_WRIST_INIT_Y_LO &&
                Math.Abs(leftWristPosition[X] - rightWristPosition[X]) > HANDS_MIN_DISTANCE &&
                Math.Abs(leftWristPosition[X] - rightWristPosition[X]) < HANDS_MAX_DISTANCE
                );
        }

        private bool isLowering()
        {

            float[] velocityLeft = new float[3];
            float[] velocityRight = new float[3];

            velocityLeft[X] = leftWristPosition[X] - leftWristLast[X];
            velocityLeft[Y] = leftWristPosition[Y] - leftWristLast[Y];
            velocityLeft[Z] = leftWristPosition[Z] - leftWristLast[Z];

            velocityRight[X] = rightWristPosition[X] - rightWristLast[X];
            velocityRight[Y] = rightWristPosition[Y] - rightWristLast[Y];
            velocityRight[Z] = rightWristPosition[Z] - rightWristLast[Z];
            return (
                    velocityLeft[Y] < 0.0f &&
                    velocityRight[Y] < 0.0f &&
                    Math.Abs(leftWristPosition[X] - rightWristPosition[X]) > HANDS_MIN_DISTANCE &&
                    Math.Abs(leftWristPosition[X] - rightWristPosition[X]) < HANDS_MAX_DISTANCE
                );

        }

        private bool isFinal()
        {

            return (
                leftWristPosition[Y] < (leftWristPosition[Y] * HANDS_RATIO_THRESHOLD) &&
                rightWristPosition[Y] < (rightWristPosition[Y] * HANDS_RATIO_THRESHOLD) &&
                Math.Abs(leftWristPosition[X] - rightWristPosition[X]) > HANDS_MIN_DISTANCE &&
                Math.Abs(leftWristPosition[X] - rightWristPosition[X]) < HANDS_MAX_DISTANCE
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

                    leftWristPosition = this.getTrackedPosition((int)NuiJointType.HandLeft);
                    rightWristPosition = this.getTrackedPosition((int)NuiJointType.HandRight);

                    if ((GestureState)this.state() == GestureState.Ready)
                    {

                        if (leftWristInitial == null || rightWristInitial == null)
                        {
                            leftWristInitial = new float[3];
                            SkeletalGesture.copyPositions(leftWristInitial, leftWristPosition);

                            rightWristInitial = new float[3];
                            SkeletalGesture.copyPositions(rightWristInitial, rightWristPosition);
                        }

                    }

                    if (!this.processNextState())
                    {
                        this.gestureDetected = true;
                        breakLoop = true;
                        break;
                    }

                    Console.WriteLine((GestureState)this.state());

                    SkeletalGesture.copyPositions(leftWristLast, leftWristPosition);
                    SkeletalGesture.copyPositions(rightWristLast, rightWristPosition);

                }
                else // skeleton or joints not found
                {
                    this.resetState();
                }



            } // while
        }

        public LowerHands(object d)
            : base(d)
        {
            // add positions to be tracked and saved on each iteration
            this.addTrackedPosition((int)NuiJointType.Head);

            // state machine state and transition implementation

            // initial state
            this.setInitialState((int)GestureState.None);

            // next state and transition function to get to this next state
            this.addState((int)GestureState.Ready, new TransitionDelegate(isReady), true);
            this.addState((int)GestureState.Lowering, new TransitionDelegate(isLowering), true);
            this.addState((int)GestureState.Final, new TransitionDelegate(isFinal), true);

            leftWristInitial = null;
            rightWristPosition = null;

        }

    }
}

