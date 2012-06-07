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

    public class Bow : Gesture
    {
        private Dictionary<int, SkeletonState> skeletons;
        private bool breakLoop = false;
        private object skeleton;
        private TimeSpan frameTrack = new TimeSpan();

        // Constants
        private const float HEAD_INIT_X_LO = -0.1f; // Right hand extended past right side of hip
        private const float HEAD_INIT_X_HI = 0.1f; // Right hand extended past right side of hip
        private const float HEAD_INIT_Y_LO = 0.0f; // Right hand extended past right side of hip
        private const float HEAD_INIT_Y_HI = 0.4f; // Right hand extended past right side of hip

        private const float HEAD_SHOULDER_DIFFERENCE_THRESHOLD = 0.1f;

        private const int X = 0;
        private const int Y = 1;
        private const int Z = 2;

        private enum GestureState
        {
            None, Ready, Bowing, Final
        }


        private bool isBowing(int currentTrackingId, float[] headPosition, float[] shoulderPosition)
        {
            return (
                    headPosition[Y] < skeletons[currentTrackingId].headLast[Y] &&
                    shoulderPosition[Y] < skeletons[currentTrackingId].shoulderLast[Y] &&
                    headPosition[Z] > skeletons[currentTrackingId].headLast[Z] &&
                    shoulderPosition[Z] > skeletons[currentTrackingId].shoulderLast[Z]
                );

        }

        private bool isReady(int currentTrackingId, float[] headPosition, float[] shoulderPosition)
        {
            return (
                headPosition[X] < HEAD_INIT_X_HI && headPosition[X] > HEAD_INIT_X_LO &&
                headPosition[Y] < HEAD_INIT_Y_HI && headPosition[Y] > HEAD_INIT_Y_LO
                );
        }


        private bool isFinal(int currentTrackingId, float[] headPosition, float[] shoulderPosition)
        {
            return (
                Math.Abs(headPosition[Y] - shoulderPosition[Y]) < HEAD_SHOULDER_DIFFERENCE_THRESHOLD
                );

            }

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
                        this.skeletons.Add(currentTrackingId, new SkeletonState());
                    }


                    object[] joints = ReflectionUtilities.InvokeProperty(skeleton, "Joints") as object[];
                    if (joints == null)
                    {
                        continue;
                    }



                    float[] headPosition = Gesture.getPositionOfJoint(joints, (int)NuiJointType.Head);
                    float[] shoulderPosition = Gesture.getPositionOfJoint(joints, (int)NuiJointType.ShoulderCenter);


                    // Conditions per state
                    switch (skeletons[currentTrackingId].state)
                    {
                        case GestureState.None:
                            {
                                if (this.isReady(currentTrackingId, headPosition, shoulderPosition)) {
                                    skeletons[currentTrackingId].state = GestureState.Ready;
                                }
                                break;
                            }
                        case GestureState.Ready:
                            {
                                if (this.isBowing(currentTrackingId, headPosition, shoulderPosition))
                                    // head and shoulders are getting both closer to camera and lower
                                {
                                    skeletons[currentTrackingId].state = GestureState.Bowing;
                                }
                                else if (!this.isReady(currentTrackingId, headPosition, shoulderPosition))
                                {
                                    skeletons[currentTrackingId].state = GestureState.None;
                                }
                                break;
                            }
                        case GestureState.Bowing:
                            {
                                if (this.isFinal(currentTrackingId, headPosition, shoulderPosition))
                                {
                                    skeletons[currentTrackingId].state = GestureState.Final;
                                }
                                else if (!this.isBowing(currentTrackingId, headPosition, shoulderPosition))
                                {
                                    skeletons[currentTrackingId].state = GestureState.Ready;
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


                    skeletons[currentTrackingId].save(headPosition, shoulderPosition);
                } // for
            } // while
        }

        public Bow(object d)
            : base(d)
        {
            device = d;
            if ((bool)ReflectionUtilities.InvokeProperty(device, "SupportsSkeletonData"))
            {
                skeletons = new Dictionary<int, SkeletonState>();
            }
            streams.Add(NuiStreamTypes.SkeletonData);
        }

        private class SkeletonState
        {
            public GestureState state;
            public float[] headLast;
            public float[] shoulderLast;

            public SkeletonState(GestureState s = GestureState.None, float[] hl = null, float[] sl = null)
            {
                hl = hl ?? new float[3] { -100.0f, -100.0f, -100.0f };
                sl = sl ?? new float[3] { -100.0f, -100.0f, -100.0f };

                this.state = s;
                this.headLast = hl;
                this.shoulderLast = sl;
            }
            public void save(float[] headPosition, float[] shoulderPosition)
            {
                this.headLast = headPosition;
                this.shoulderLast = shoulderPosition;
            }

        }
    }
}
