using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuiDeviceFramework.datatypes
{
    public class NuiBoneRotation
    {
        private NuiMatrix4 matrix;
        private NuiVector4 quat;

        public NuiMatrix4 Matrix { get { return this.matrix; } set { this.matrix = value; } }
        public NuiVector4 Quaternion { get { return this.quat; } set { this.quat = value; } }

        public NuiBoneRotation(NuiMatrix4 m, NuiVector4 q)
        {
            matrix = m;
            quat = q;
        }
    }
}
