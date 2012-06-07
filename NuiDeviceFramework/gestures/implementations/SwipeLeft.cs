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
        private static float RIGHTHAND_INIT_DIST = 0.3f; // Right hand extended past right side of hip
        private static float RIGHTHAND_END_DIST = -0.1f; // Right hand crosses over to left side of hip
        private static float RIGHTHAND_MIN_VELOCITY = 0.05f; // Position units per frame
        private static float RIGHTHAND_MAX_VELOCITY = 0.6f; // Position units per frame
        private static float RIGHTHAND_Y_THRESHOLD = 0.2f;

        private static int FRAME_UPDATE_WAIT = 3; // Check every few frames before analyzing

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

                    // Type cast error occurs here
                    object[] joints = ReflectionUtilities.InvokeProperty(skeleton, "Joints") as object[];
                    if (joints == null)
                    {
                        // This always executes
                        continue;
                    }

                    object hipPosition = ReflectionUtilities.InvokeProperty(joints[(int)NuiJointType.HipCenter], "Position");
                    object rightHandPosition = ReflectionUtilities.InvokeProperty(joints[(int)NuiJointType.WristRight], "Position");
                    float hipCenterX = (float)ReflectionUtilities.InvokeProperty(hipPosition, "X");
                    float hipCenterY = (float)ReflectionUtilities.InvokeProperty(hipPosition, "Y");
                    float rightHandPosX = (float)ReflectionUtilities.InvokeProperty(rightHandPosition, "X");
                    float rightHandPosY = (float)ReflectionUtilities.InvokeProperty(rightHandPosition, "Y");
                    float distanceHipX = rightHandPosX - hipCenterX;
                    float distanceHipY = rightHandPosY - hipCenterY;
                    float velocityX;

                    Console.WriteLine("Distance X: " + distanceHipX + " , Distance Y: " + distanceHipY);

                    // Conditions maintained for all states
                    if (distanceHipY < -RIGHTHAND_Y_THRESHOLD && distanceHipY < RIGHTHAND_Y_THRESHOLD)
                    {
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
                                if ((rightHandPosX - rightHandLastX) > 0)
                                {
                                    state = GestureState.MoveLeft;
                                }
                                break;
                            }
                        case GestureState.MoveLeft:
                            {
                                if (distanceHipX <= RIGHTHAND_END_DIST)
                                {
                                    state = GestureState.Final;
                                }
                                velocityX = rightHandLastX - rightHandPosX;
                                if (velocityX < RIGHTHAND_MIN_VELOCITY || velocityX > RIGHTHAND_MAX_VELOCITY)
                                {
                                    state = GestureState.Looking;
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
                    @"Error in adding gesture:
skeleton data not supported by device.");
            }
        }

        private enum GestureState
        {
            Looking, Initial, MoveLeft, Final
        }
    }
}
