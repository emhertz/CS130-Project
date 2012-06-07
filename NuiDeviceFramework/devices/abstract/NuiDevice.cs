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
