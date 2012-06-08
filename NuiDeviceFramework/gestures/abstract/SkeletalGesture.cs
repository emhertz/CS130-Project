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
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.devices;
using NuiDeviceFramework.reflection;


namespace NuiDeviceFramework.Gestures
{



    public abstract class SkeletalGesture : Gesture
    {
        protected object skeleton;

        protected object joints;

        const int X = 0;
        const int Y = 1;
        const int Z = 2;

        private Dictionary<int, float[]> trackedPositions = new Dictionary<int, float[]>();

        public delegate bool TransitionDelegate();

        protected class Transition
        {
            public TransitionDelegate del;
            public bool reversible;
            public int prevState;
            public int nextState;
        }
        private List<Transition> transitions = new List<Transition>();
        private List<int> states = new List<int>();
        private int curState = 0;

        protected void setInitialState(int state) {
            states.Clear();
            states.Add(state);
            curState = state;
        }
        protected int state()
        {
            return this.curState;
        }
        protected bool processNextState()
        {

            int stateIdx = states.FindIndex(s => s == curState);
            if (stateIdx >= states.Count() - 1)
            {
                return true;
            }
            else if (stateIdx < 0)
            {
                return false;
            }
            
            Transition t = this.transitions[stateIdx];
            TransitionDelegate td = t.del;
            if (td())
            {
                curState = t.nextState;
            }
            else if (t.reversible && stateIdx > 0 && !this.transitions[stateIdx - 1].del())
            {
                curState = this.transitions[stateIdx-1].prevState;
            }

            return curState != states.Last();
        }
        protected void resetState()
        {
            curState = states.First();
        }
        protected void addState(int nextState, TransitionDelegate transitionFunction, bool reversible)
        {
            Transition t = new Transition();
            t.del = transitionFunction;
            t.reversible = reversible;
            t.nextState = nextState;
            t.prevState = states.Last();
            transitions.Add(t);
            states.Add(nextState);
        }


        public SkeletalGesture(object d)
            : base(d)
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

        protected bool getSkeleton()
        {
            object[] skeletonArray = ReflectionUtilities.InvokeProperty(device, "SkeletonArray") as object[];
            if (skeletonArray == null)
                return false;

            int skeletonArrayLength = (int)ReflectionUtilities.InvokeProperty(skeletonArray, "Length");
            if (skeletonArrayLength <= 0)
                return false;

            bool skeletonFound = false;
            int skeletonIndex = -1;

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
            if (skeletonFound)
            {
                skeleton = skeletonArray[skeletonIndex];
                return true;
            }
            else
            {
                return false;
            }

        }
        protected bool getJoints()
        {
            joints = ReflectionUtilities.InvokeProperty(skeleton, "Joints") as object;
            
            if (joints == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected void addTrackedPosition(int nuiJointTypeId)
        {
            float[] initial = new float[3] { -100.0f, -100.0f, -100.0f };
            trackedPositions[nuiJointTypeId] = initial;
        }

        protected void loadTrackedPositions()
        {
            foreach (KeyValuePair<int, float[]> entry in trackedPositions)
            {
                float[] pos = this.getPositionOfJoint(entry.Key);
                entry.Value[X] = pos[X];
                entry.Value[Y] = pos[Y];
                entry.Value[Z] = pos[Z];
            }
        }

        protected float[] getTrackedPosition(int nuiJointTypeId)
        {
            float[] ret = new float[3];

            if (trackedPositions.ContainsKey(nuiJointTypeId))
            {
                return trackedPositions[nuiJointTypeId];
            }
            else
            {
                ret[X] = -100.0f;
                ret[Y] = -100.0f;
                ret[Z] = -100.0f;
            }
            return ret;
        }

        private float[] getPositionOfJoint(int nuiJointTypeId)
        {
            float[] ret = new float[3];
            object joint = ReflectionUtilities.InvokeMethod(joints, "GetJoint", new object[] { nuiJointTypeId });
            object posProp = ReflectionUtilities.InvokeProperty(joint, "Position");
            float x = (float)ReflectionUtilities.InvokeProperty(posProp, "X");
            float y = (float)ReflectionUtilities.InvokeProperty(posProp, "Y");
            float z = (float)ReflectionUtilities.InvokeProperty(posProp, "Z");

            ret[0] = x;
            ret[1] = y;
            ret[2] = z;

            return ret;

        }

        public static void copyPositions(float[] dst, float[] src)
        {
            dst[X] = src[X];
            dst[Y] = src[Y];
            dst[Z] = src[Z];
        }

    }
}
