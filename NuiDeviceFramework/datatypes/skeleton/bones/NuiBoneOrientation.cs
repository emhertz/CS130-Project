using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuiDeviceFramework.datatypes.skeleton.enums;

namespace NuiDeviceFramework.datatypes
{
    public class NuiBoneOrientation
    {
        private NuiJointType endJoint;
        private NuiJointType startJoint;

        private NuiBoneRotation absoluteRotation;
        private NuiBoneRotation hierarchicalRotation;

        public NuiBoneRotation AbsoluteRotation { get { return this.absoluteRotation; } set { this.absoluteRotation = value; } }
        public NuiJointType EndJoint { get { return this.endJoint; } }
        public NuiBoneRotation HierarchicalRotation { get { return this.hierarchicalRotation; } set { this.hierarchicalRotation = value; } }
        public NuiJointType StartJoint { get { return this.startJoint; } }

        public NuiBoneOrientation(NuiJointType s, NuiJointType e, NuiBoneRotation a, NuiBoneRotation h)
        {
            startJoint = s;
            endJoint = e;
            absoluteRotation = a;
            hierarchicalRotation = h;
        }
    }
}
