using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.devices;
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;

namespace NuiDeviceFramework.gestures.implementations
{
    /* Gesture1 -- Swipe Left
         - Behavior: Swift motion of right hand from right to left
         - Begin: WristRight.X > HipCenter.X by difference INIT_RIGHTHAND_DIST
         - End: WristRight.X < HipCenter.X by difference END_RIGHTHAND_DIST
         - Conditions: WristRight in line with HipCenter in Y Direction
       Stream(s) needed: SkeletonData */

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

    public class Gesture1 : Gesture
    {
        private NuiDevice device;
        private Dictionary<int, SkeletonState> skeletons;
        private bool breakLoop = false;
        private NuiSkeleton skeleton;
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
                if (frameTrack != device.SkeletonLastModified)
                {
                    frameTrack = device.SkeletonLastModified;
                }
                else
                {
                    continue;
                }

                for (int i = 0; i < device.SkeletonArray.Length; i++)
                {
                    if (device.SkeletonArray[i].TrackingState != NuiSkeletonTrackingState.Tracked)
                    {
                        continue;
                    }

                    skeleton = device.SkeletonArray[i];
                    if (!this.skeletons.ContainsKey(skeleton.TrackingId))
                    {
                        this.skeletons.Add(skeleton.TrackingId, new SkeletonState(GestureState.Looking, 0.0f, 0.0f));
                    }

                    float hipCenterX = skeleton.Joints[NuiJointType.HipCenter].Position.X;
                    float hipCenterY = skeleton.Joints[NuiJointType.HipCenter].Position.Y;
                    float rightHandPosX = skeleton.Joints[NuiJointType.WristRight].Position.X;
                    float rightHandPosY = skeleton.Joints[NuiJointType.WristRight].Position.Y;
                    float distanceHipX = rightHandPosX - hipCenterX;
                    float distanceHipY = rightHandPosY - hipCenterY;
                    float velocityX;
                    int id = skeleton.TrackingId;

                    // Conditions maintained for all states
                    if (distanceHipY < -RIGHTHAND_Y_THRESHOLD && distanceHipY < RIGHTHAND_Y_THRESHOLD)
                    {
                        skeletons[id].state = GestureState.Looking;
                    }

                    // Conditions per state
                    switch (skeletons[id].state)
                    {
                        case GestureState.Looking:
                            {
                                if (distanceHipX >= RIGHTHAND_END_DIST)
                                {
                                    skeletons[id].state = GestureState.Initial;
                                }
                                break;
                            }
                        case GestureState.Initial:
                            {
                                if ((rightHandPosX - skeletons[i].rightHandLastX) > 0)
                                {
                                    skeletons[id].state = GestureState.MoveLeft;
                                }
                                break;
                            }
                        case GestureState.MoveLeft:
                            {
                                if (distanceHipX <= RIGHTHAND_END_DIST)
                                {
                                    skeletons[id].state = GestureState.Final;
                                }
                                velocityX = skeletons[id].rightHandLastX - rightHandPosX;
                                if (velocityX < RIGHTHAND_MIN_VELOCITY || velocityX > RIGHTHAND_MAX_VELOCITY)
                                {
                                    skeletons[id].state = GestureState.Looking;
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

                    skeletons[id].rightHandLastX = rightHandPosX;
                    skeletons[id].rightHandLastY = rightHandPosY;
                } // for
            } // while
        }

        public Gesture1(NuiDevice d) : base(d)
        {
            device = d;
            if (device.SupportsSkeletonData)
            {
                skeletons = new Dictionary<int, SkeletonState>();
                Start();
            }
        }
    }
}
