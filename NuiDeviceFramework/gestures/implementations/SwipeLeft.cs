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

    public class SwipeLeft : Gesture
    {
        private bool breakLoop = false;
        private object skeleton;
        private TimeSpan frameTrack = new TimeSpan();
        private int frameNumber = 0;
        private int skeletonIndex = 0;
        private bool skeletonFound;

        private GestureState state = GestureState.Looking;
        private float rightHandLastX = 0.0f;
        private float rightHandLastY = 0.0f;
        
        // Constants
        private static float RIGHTHAND_INIT_DIST = 0.25f; // Right hand extended past right side of hip
        private static float RIGHTHAND_END_DIST = -0.05f; // Right hand crosses over to left side of hip
        private static float RIGHTHAND_MIN_VELOCITY = 0.005f;
        private static float RIGHTHAND_Y_THRESHOLD = 0.3f;

        private static int FRAME_UPDATE_WAIT = 12; // Check every few frames before analyzing

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

                object[] skeletonArray = ReflectionUtilities.InvokeProperty(device, "SkeletonArray") as object[];
                if (skeletonArray == null)
                {
                    
                    continue;
                }
                int skeletonArrayLength = (int)ReflectionUtilities.InvokeProperty(skeletonArray, "Length");
                if (skeletonArrayLength > 0)
                {
                    skeletonFound = false;
                    for (int i = 0; i < skeletonArrayLength; i++)
                    {
                        object currTrackingState = ReflectionUtilities.InvokeProperty(skeletonArray[i], "TrackingState");
                        if ((int)currTrackingState == (int)NuiSkeletonTrackingState.Tracked)
                        {
                            skeletonIndex = i;
                            skeletonFound = true;
                            break;
                        }
                    }
                    if (skeletonFound == false)
                    {
                        continue;
                    }

                    skeleton = skeletonArray[skeletonIndex];

                   
                    object joints = ReflectionUtilities.InvokeProperty(skeleton, "Joints") as object;
                    if (joints == null)
                    {
                        continue;
                    }

                    // Joint calculations
                    object hipJoint = ReflectionUtilities.InvokeMethod(joints, "GetJoint", new object[] { NuiJointType.HipCenter });
                    object hipPosition = ReflectionUtilities.InvokeProperty(hipJoint, "Position");
                    object rightHandJoint = ReflectionUtilities.InvokeMethod(joints, "GetJoint", new object[] { NuiJointType.WristRight });
                    object rightHandPosition = ReflectionUtilities.InvokeProperty(rightHandJoint, "Position");
                    float hipCenterX = (float)ReflectionUtilities.InvokeProperty(hipPosition, "X");
                    float hipCenterY = (float)ReflectionUtilities.InvokeProperty(hipPosition, "Y");
                    float rightHandPosX = (float)ReflectionUtilities.InvokeProperty(rightHandPosition, "X");
                    float rightHandPosY = (float)ReflectionUtilities.InvokeProperty(rightHandPosition, "Y");
                    float distanceHipX = rightHandPosX - hipCenterX;
                    float distanceHipY = rightHandPosY - hipCenterY;
                    float velocityX = rightHandLastX - rightHandPosX;

                    Console.WriteLine("State: " + (int)state + " DX: " + distanceHipX + " DY: " + distanceHipY + " Vel: " + velocityX);

                    // Conditions maintained for all states
                    if (distanceHipY < -RIGHTHAND_Y_THRESHOLD || distanceHipY > RIGHTHAND_Y_THRESHOLD)
                    {
                        Console.WriteLine("FAILURE: Out Of Y");
                        state = GestureState.Looking;
                    }

                    // Conditions per state
                    switch (state)
                    {
                        case GestureState.Looking:
                            {
                                if (distanceHipX >= RIGHTHAND_INIT_DIST)
                                {
                                    state = GestureState.Initial;
                                }
                                break;
                            }
                        case GestureState.Initial:
                            {
                                if (velocityX > RIGHTHAND_MIN_VELOCITY)
                                {
                                    state = GestureState.MoveLeft;
                                }
                                break;
                            }
                        case GestureState.MoveLeft:
                            {
                                if (velocityX < -RIGHTHAND_MIN_VELOCITY)
                                {
                                    state = GestureState.Looking;
                                    Console.WriteLine("FAILURE: Reverse Direction");
                                }
                                if (distanceHipX <= RIGHTHAND_END_DIST)
                                {
                                    state = GestureState.Final;
                                }
                                break;
                            }
                        case GestureState.Final:
                            {
                                this.gestureDetected = true;
                                // Exit loop for now
                                breakLoop = true;
                                break;
                            }
                    } // switch

                    rightHandLastX = rightHandPosX;
                    rightHandLastY = rightHandPosY;
                } // for
            } // while
        }

        public SwipeLeft(object d) : base(d)
        {
            device = d;
            if ((bool)ReflectionUtilities.InvokeProperty(device, "SupportsSkeletonData"))
            {
                streams.Add(NuiStreamTypes.SkeletonData);
            }
            else
            {
                Console.WriteLine(
                    @"Error in adding gesture: skeleton data not supported by device.");
            }
        }

        private enum GestureState
        {
            Looking, Initial, MoveLeft, Final
        }
    }
}
