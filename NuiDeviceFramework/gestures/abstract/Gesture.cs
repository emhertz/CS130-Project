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
        protected List<NuiStreamTypes> streams;
        protected Boolean gestureDetected = false;
        protected object device;

        public Boolean GestureDetected
        {
            get
            {
                return this.gestureDetected;
            }
            set
            {
                this.gestureDetected = value;
            }
        }

        public List<NuiStreamTypes> getNecessaryStreams()
        {
            return streams;
        }

        public void ResetGesture()
        {
            gestureDetected = false;
        }

        protected Gesture(object d)
        {
            this.device = d;
            streams = new List<NuiStreamTypes>();
        }

        public abstract void Start();
    }
}
