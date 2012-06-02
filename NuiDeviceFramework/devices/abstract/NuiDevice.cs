using System;
using System.Windows.Media.Imaging;
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.datatypes.constants;

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

        protected Boolean[] streams = new Boolean[NuiConstants.NUM_STREAMS];

        // Depth image data constants
        protected short[] pixelData;
        protected byte[] depthFrame32;
        protected WriteableBitmap outputBitmap;

        // Color image data constants
        protected byte[] colorPixelData;
        protected byte[] colorFrame32;
        protected WriteableBitmap colorOutputBitmap;

        public Boolean supportsStreamType(NuiStreamTypes s)
        {
            return streams[(int)s];
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

        public WriteableBitmap DepthBitmap
        {
            get
            {
                return this.outputBitmap;
            }
        }

    }
}
