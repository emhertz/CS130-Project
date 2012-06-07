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
