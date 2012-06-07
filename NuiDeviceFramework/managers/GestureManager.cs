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
using System.Threading;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.devices;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.reflection;

namespace NuiDeviceFramework.managers
{
    public class GestureManager
    {
        private List<Gesture> gestures = new List<Gesture>();
        private List<Boolean> gesturesCompleted = new List<Boolean>();
        private List<Thread> gestureThreads = new List<Thread>();
        private object device;
        private Boolean running;

        public Boolean IsRunning
        {
            get
            {
                return this.running;
            }
        }

        public GestureManager(object d)
        {
            this.device = d;
        }

        public Boolean Add(Gesture g)
        {
            if (!checkDeviceSupportsFeatures(g))
            {
                return false;
            }
            gestures.Add(g);
            gesturesCompleted.Add(false);
            return true;
        }

        public Boolean checkDeviceSupportsFeatures(Gesture g)
        {
            foreach (NuiStreamTypes s in g.getNecessaryStreams())
            {
                object val = ReflectionUtilities.InvokeMethod(device, "supportsStreamType", new object[]{s});
                if (val is bool && (! (bool)val) )
                {
                    return false;
                }
            }
            return true;
        }

        public void Start()
        {
            running = true;
            foreach (Gesture g in gestures)
            {
                Thread t = new Thread(new ThreadStart(g.Start));
                t.Start();
                gestureThreads.Add(t);
            }
        }

        public List<Gesture> getCompletedGestures()
        {
            List<Gesture> result = new List<Gesture>();

            foreach (Gesture g in gestures)
            {
                if (g.GestureDetected)
                {
                    result.Add(g);
                }
                g.GestureDetected = false;
            }

            return result;
        }

        public void Stop()
        {
            running = false;
            foreach (Thread t in gestureThreads)
            {
                if (t.IsAlive)
                {
                    t.Abort();
                }
            }
        }
    }
}
