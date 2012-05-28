using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuiDeviceFramework.datatypes.skeleton.enums;

namespace NuiDeviceFramework.datatypes
{
    public struct NuiJoint
    {
        private NuiJointType jointType;
        private NuiSkeletonPoint position;
        private NuiJointTrackingState trackingState;

        public NuiJointType JointType { get { return jointType; } }
        public NuiSkeletonPoint Position { get { return this.position; } set { this.position = value; } }
        public NuiJointTrackingState TrackingState { get { return this.trackingState; } set { this.trackingState = value; } }

        public NuiJoint(NuiJointType jt, NuiSkeletonPoint p, NuiJointTrackingState ts)
        {
            position = p;
            trackingState = ts;
            jointType = jt;
        }

        public bool Equals(NuiJoint joint)
        {
            return false;
        }

        public override bool Equals(object joint)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool op_Equality(NuiJoint joint1, NuiJoint joint2)
        {
            return false;
        }

        public static bool op_Inequality(NuiJoint joint1, NuiJoint joint2)
        {
            return false;
        }
    }
}
