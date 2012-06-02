using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.devices;

namespace NuiDeviceFramework.Gestures
{
    public abstract class Gesture 
    {
        protected NuiStreamTypes[] streams;
        protected Boolean gestureDetected = false;
        protected NuiDevice device;

        public Boolean GestureDetected
        {
            get
            {
                return this.gestureDetected;
            }
        }

        public NuiStreamTypes[] getNecessaryStreams()
        {
            return streams;
        }

        public void ResetGesture()
        {
            gestureDetected = false;
        }

        protected Gesture(NuiDevice d)
        {
            this.device = d;
        }

        public abstract void Start();
    }
}
