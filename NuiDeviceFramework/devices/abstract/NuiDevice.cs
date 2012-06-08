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
using System.Windows.Media.Imaging;
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.datatypes.constants;
using System.IO;

namespace NuiDeviceFramework.devices
{
    public abstract class NuiDevice
    {
        // Skeleton data constants
        protected NuiSkeleton[] myNuiSkeletonData;
        protected TimeSpan skeletonLastModified;

        protected Boolean supportsColorData = false;
        protected Boolean supportsDepthData = false;
        protected Boolean supportsSkeletonData = false;
        protected Boolean supportsAudioData = false;
        protected Boolean supportsObjectData = false;

        protected Boolean[] streams = new Boolean[NuiConstants.NUM_STREAMS];

        // Audio data contants
        protected Stream audioStream;

        // Depth image data constants
        protected short[] pixelData;
        protected byte[] depthFrame32;
        protected WriteableBitmap outputBitmap;

        // Color image data constants
        protected byte[] colorPixelData;
        protected byte[] colorFrame32;
        protected WriteableBitmap colorOutputBitmap;

        // object data
        protected object[] objectData;



        public Boolean supportsStreamType(NuiStreamTypes s)
        {
            return streams[(int)s];
        }

        public Boolean SupportsObjectData
        {
            get
            {
                return this.supportsObjectData;
            }
        }

        public Boolean SupportsColorData
        {
            get
            {
                return this.supportsColorData;
            }
        }

        public Boolean SupportsAudioData
        {
            get
            {
                return this.supportsAudioData;
            }
        }

        public Boolean SupportsSkeletonData
        {
            get
            {
                return this.supportsSkeletonData;
            }
        }

        public Boolean SupportsDepthData
        {
            get
            {
                return this.supportsDepthData;
            }
        }

        public object[] ObjectData
        {
            get
            {
                return this.objectData;
            }
        }

        public WriteableBitmap ColorBitmap
        {
            get
            {
                return this.colorOutputBitmap;
            }
        }

        public NuiSkeleton[] SkeletonArray
        {
            get
            {
                return this.myNuiSkeletonData;
            }
        }

        public TimeSpan SkeletonLastModified
        {
            get
            {
                return this.skeletonLastModified;
            }
        }

        public Stream AudioStream
        {
            get
            {
                return this.audioStream;
            }
        }

        public WriteableBitmap DepthBitmap
        {
            get
            {
                return this.outputBitmap;
            }
        }

    }
}
