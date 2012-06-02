using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using NuiDeviceFramework.datatypes.skeleton.enums;

namespace NuiDeviceFramework.datatypes
{
    [ComVisibleAttribute(false)]
    public class NuiJointCollection : IEnumerable
    {
        private List<NuiJoint> joints;

        public NuiJointCollection()
        {
            joints = new List<NuiJoint>(500);
        }

        public NuiJoint this[NuiJointType jointType] { get { return this.joints[(int)jointType]; } set { this.joints[(int)jointType] = value;} }
        public int Count
        {
            get { return this.joints.Count; }
        }
        public IEnumerator GetEnumerator()
        {
            return new NuiJointEnumerator(this);
        }

        private class NuiJointEnumerator : IEnumerator
        {
            private NuiJointCollection collection;
            private NuiJointType position = NuiJointType.AnkleLeft;

            public NuiJointEnumerator(NuiJointCollection coll)
            {
                this.collection = coll;
            }

            public bool MoveNext()
            {
                if (position < NuiJointType.WristRight)
                {
                    position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                position = NuiJointType.AnkleLeft;
            }

            public object Current
            {
                get
                {
                    return collection[position];
                }
            }
        }
    }
}
