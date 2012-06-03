using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NuiDeviceFramework.datatypes.skeleton.enums;

namespace NuiDeviceFramework.datatypes
{
    public class NuiBoneOrientationCollection : IEnumerable
    {
        private List<NuiBoneOrientation> orientations;

        public NuiBoneOrientationCollection()
        {
            orientations = new List<NuiBoneOrientation>(500);
        }

        public int Count { get { return this.orientations.Count;} }
        public NuiBoneOrientation this[NuiJointType jointType] 
        { 
            get 
            { 
                return this.orientations[(int)jointType]; 
            }
            set
            {
                if ((int)jointType < this.orientations.Capacity)
                {
                    this.orientations.Insert(((int)jointType), value);
                }
            } 
        }

        public IEnumerator GetEnumerator()
        {
            return new NuiBoneOrientationEnumerator(this);
        }

        private class NuiBoneOrientationEnumerator : IEnumerator
        {
            private NuiBoneOrientationCollection collection;
            private NuiJointType position = NuiJointType.AnkleLeft;

            public NuiBoneOrientationEnumerator(NuiBoneOrientationCollection coll)
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
