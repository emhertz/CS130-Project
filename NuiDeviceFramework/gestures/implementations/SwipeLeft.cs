using System;
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
        private Dictionary<int, SkeletonState> skeletons;
        private bool breakLoop = false;
        private object skeleton;
        private TimeSpan frameTrack = new TimeSpan();
        
        // Constants
        private static float RIGHTHAND_INIT_DIST = 0.3f; // Right hand extended past right side of hip
        private static float RIGHTHAND_END_DIST = -0.1f; // Right hand crosses over to left side of hip
        private static float RIGHTHAND_MIN_VELOCITY = 0.05f; // Position units per frame
        private static float RIGHTHAND_MAX_VELOCITY = 0.6f; // Position units per frame
        private static float RIGHTHAND_Y_THRESHOLD = 0.2f;

        public override void Start()
        {
            while (!breakLoop)
            {
                TimeSpan tspan = (TimeSpan)ReflectionUtilities.InvokeProperty(device, "SkeletonLastModified");
                if (frameTrack != tspan)
                {
                    frameTrack = tspan;
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
                for (int i = 0; i < skeletonArrayLength; i++)
                {
                    object currTrackingState = ReflectionUtilities.InvokeProperty(skeletonArray[i], "TrackingState");
                    if ((int)currTrackingState != (int)NuiSkeletonTrackingState.Tracked)
                    {
                        continue;
                    }

                    skeleton = skeletonArray[i];
                    int currentTrackingId = (int)ReflectionUtilities.InvokeProperty(skeleton, "TrackingId");
                    if (!this.skeletons.ContainsKey(currentTrackingId))
                    {
                        this.skeletons.Add(currentTrackingId, new SkeletonState(GestureState.Looking, 0.0f, 0.0f));
                    }

                    object[] joints = ReflectionUtilities.InvokeProperty(skeleton, "Joints") as object[];
                    if (joints == null)
                    {
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

                    // Conditions maintained for all states
                    if (distanceHipY < -RIGHTHAND_Y_THRESHOLD && distanceHipY < RIGHTHAND_Y_THRESHOLD)
                    {
                        skeletons[currentTrackingId].state = GestureState.Looking;
                    }

                    // Conditions per state
                    switch (skeletons[currentTrackingId].state)
                    {
                        case GestureState.Looking:
                            {
                                if (distanceHipX >= RIGHTHAND_END_DIST)
                                {
                                    skeletons[currentTrackingId].state = GestureState.Initial;
                                }
                                break;
                            }
                        case GestureState.Initial:
                            {
                                if ((rightHandPosX - skeletons[i].rightHandLastX) > 0)
                                {
                                    skeletons[currentTrackingId].state = GestureState.MoveLeft;
                                }
                                break;
                            }
                        case GestureState.MoveLeft:
                            {
                                if (distanceHipX <= RIGHTHAND_END_DIST)
                                {
                                    skeletons[currentTrackingId].state = GestureState.Final;
                                }
                                velocityX = skeletons[currentTrackingId].rightHandLastX - rightHandPosX;
                                if (velocityX < RIGHTHAND_MIN_VELOCITY || velocityX > RIGHTHAND_MAX_VELOCITY)
                                {
                                    skeletons[currentTrackingId].state = GestureState.Looking;
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

                    skeletons[currentTrackingId].rightHandLastX = rightHandPosX;
                    skeletons[currentTrackingId].rightHandLastY = rightHandPosY;
                } // for
            } // while
        }

        public SwipeLeft(object d) : base(d)
        {
            device = d;
            if ((bool)ReflectionUtilities.InvokeProperty(device, "SupportsSkeletonData"))
            {
                skeletons = new Dictionary<int, SkeletonState>();
            }
            streams.Add(NuiStreamTypes.SkeletonData);
        }

        private enum GestureState
        {
            Looking, Initial, MoveLeft, Final
        }

        private class SkeletonState
        {
            public GestureState state;
            public float rightHandLastX;
            public float rightHandLastY;

            public SkeletonState(GestureState s, float x, float y)
            {
                this.state = s;
                this.rightHandLastX = x;
                this.rightHandLastY = y;
            }
        }
    }
}
