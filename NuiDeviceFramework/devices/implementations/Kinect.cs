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
Copyright � 2012, Cisco Systems, Inc. and UCLA
*************************************************************/

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using NuiDeviceFramework.datatypes;
using NuiDeviceFramework.datatypes.skeleton.enums;

namespace NuiDeviceFramework.devices
{
    public class Kinect : NuiDevice
    {
        private KinectSensor sensor;

        // Skeleton data constants
        private Skeleton[] mySkeletonData;

        // Audio constants
        private KinectAudioSource audioSource;

        private DepthImageFormat lastImageFormat; 
        private ColorImageFormat lastColorImageFormat;

        // color divisors for tinting depth pixels
        private static readonly int[] IntensityShiftByPlayerR = { 1, 2, 0, 2, 0, 0, 2, 0 };
        private static readonly int[] IntensityShiftByPlayerG = { 1, 2, 2, 0, 2, 0, 0, 1 };
        private static readonly int[] IntensityShiftByPlayerB = { 1, 0, 2, 2, 0, 2, 0, 2 };

        private const int RedIndex = 2;
        private const int GreenIndex = 1;
        private const int BlueIndex = 0;
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        public Kinect() : base()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if(potentialSensor.Status == KinectStatus.Connected) 
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            if (this.sensor != null)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                this.sensor.SkeletonStream.Enable();

                this.supportsSkeletonData = this.sensor.SkeletonStream.IsEnabled;
                this.streams[(int)NuiStreamTypes.SkeletonData] = this.supportsSkeletonData;
                this.supportsColorData = this.sensor.ColorStream.IsEnabled;
                this.streams[(int)NuiStreamTypes.ColorData] = this.supportsColorData;
                this.supportsDepthData = this.sensor.DepthStream.IsEnabled;
                this.streams[(int)NuiStreamTypes.DepthData] = this.supportsDepthData;

                this.sensor.DepthFrameReady += this.DepthImageReady;
                this.sensor.ColorFrameReady += this.ColorImageReady;
                this.sensor.SkeletonFrameReady += this.SkeletonImageReady;

                this.pixelData = new short[this.sensor.DepthStream.FramePixelDataLength];
                this.depthFrame32 = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];

                this.mySkeletonData = new Skeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
                this.myNuiSkeletonData = new NuiSkeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
                this.colorPixelData = new byte[this.sensor.ColorStream.FramePixelDataLength];

                this.sensor.Start();

                this.audioSource = this.sensor.AudioSource;
                audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
                this.audioStream = audioSource.Start();
                this.supportsAudioData = true;
                this.streams[(int)NuiStreamTypes.AudioData] = this.supportsAudioData;

                this.supportsObjectData = false;
                this.streams[(int)NuiStreamTypes.ObjectData] = this.supportsObjectData;
            }
        }

        private void ColorImageReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    bool haveNewFormat = this.lastColorImageFormat != imageFrame.Format;

                    if (haveNewFormat)
                    {
                        this.colorPixelData = new byte[imageFrame.PixelDataLength];
                        this.colorFrame32 = new byte[imageFrame.Width * imageFrame.Height * Bgr32BytesPerPixel];
                    }

                    imageFrame.CopyPixelDataTo(this.colorPixelData);

                    byte[] convertedDepthBits = this.ConvertDepthFrame(this.pixelData, ((KinectSensor)sender).DepthStream);

                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    if (haveNewFormat)
                    {
                        this.colorOutputBitmap = new WriteableBitmap(
                            imageFrame.Width,
                            imageFrame.Height,
                            96,  // DpiX
                            96,  // DpiY
                            PixelFormats.Bgr32,
                            null);
                    }

                    this.colorOutputBitmap.WritePixels(
                        new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height),
                        convertedDepthBits,
                        imageFrame.Width * Bgr32BytesPerPixel,
                        0);

                    this.lastColorImageFormat = imageFrame.Format;
                }
                else
                {
                    //no data to receive
                }
            }
        }

        private void SkeletonImageReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletonFrame.CopySkeletonDataTo(this.mySkeletonData);
                    for(int i = 0; i < mySkeletonData.Length; i++)
                    {
                        Skeleton s = mySkeletonData[i];
                        NuiBoneOrientationCollection nboc = new NuiBoneOrientationCollection();
                        /*
                         * note re: BoneOrientation
                         * Apparently we're supposed to be able to access bone orientation info through the Skeleton.BoneOrientation
                         * property, but I can't seem to do it. When you can do this, uncomment the following code
                         * 
                         * BEGIN CODE
                         * 
                         * for(JointType j = 0; j < (JointType)s.BoneOrientation.Count; j++)
                         * {
                         *  BoneOrientation b = s.BoneOrientation[j];
                         *  NuiBoneOrientation nb = new NuiBoneOrientation(b.StartJoint, b.EndJoint, b.AbsoluteRotation, b.HierarchicalRotation);
                         *  nboc[(NuiJointType)j] = nb;
                         * }
                         * 
                         * END CODE
                         */
                        NuiFrameEdges fe = (NuiFrameEdges)s.ClippedEdges;
                        NuiJointCollection njc = new NuiJointCollection();
                        for (JointType j = 0; j < (JointType)s.Joints.Count; j++)
                        {
                            Joint jt = s.Joints[j];
                            NuiJoint nj = new NuiJoint((NuiJointType)jt.JointType, new NuiSkeletonPoint(jt.Position.X, jt.Position.Y, jt.Position.Z), (NuiJointTrackingState)jt.TrackingState);
                            njc[(NuiJointType)j] = nj;
                        }
                        NuiSkeletonPoint nsp = new NuiSkeletonPoint(s.Position.X, s.Position.Y, s.Position.Z);
                        int trackingId = s.TrackingId;
                        NuiSkeletonTrackingState nts = (NuiSkeletonTrackingState)s.TrackingState;
                        myNuiSkeletonData[i] = new NuiSkeleton(nboc, fe, nsp, trackingId, nts, njc);
                    }

                    skeletonLastModified = System.DateTime.Now.TimeOfDay;
                }
            }
        }

        private void DepthImageReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame imageFrame = e.OpenDepthImageFrame())
            {
                if (imageFrame != null)
                {
                    bool haveNewFormat = this.lastImageFormat != imageFrame.Format;

                    if (haveNewFormat)
                    {
                        this.pixelData = new short[imageFrame.PixelDataLength];
                        this.depthFrame32 = new byte[imageFrame.Width * imageFrame.Height * Bgr32BytesPerPixel];
                    }

                    imageFrame.CopyPixelDataTo(this.pixelData);

                    byte[] convertedDepthBits = this.ConvertDepthFrame(this.pixelData, ((KinectSensor)sender).DepthStream);

                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    if (haveNewFormat)
                    {
                        this.outputBitmap = new WriteableBitmap(
                            imageFrame.Width, 
                            imageFrame.Height, 
                            96,  // DpiX
                            96,  // DpiY
                            PixelFormats.Bgr32, 
                            null);
                    }

                    this.outputBitmap.WritePixels(
                        new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height), 
                        convertedDepthBits,
                        imageFrame.Width * Bgr32BytesPerPixel,
                        0);

                    this.lastImageFormat = imageFrame.Format;
                }
                else
                {
                    //no data to receive
                }
            }
        }

        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        private byte[] ConvertDepthFrame(short[] depthFrame, DepthImageStream depthStream)
        {
            int tooNearDepth = depthStream.TooNearDepth;
            int tooFarDepth = depthStream.TooFarDepth;
            int unknownDepth = depthStream.UnknownDepth;

            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < this.depthFrame32.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = depthFrame[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(~(realDepth >> 4));

                if (player == 0 && realDepth == 0)
                {
                    // white 
                    this.depthFrame32[i32 + RedIndex] = 255;
                    this.depthFrame32[i32 + GreenIndex] = 255;
                    this.depthFrame32[i32 + BlueIndex] = 255;
                }
                else if (player == 0 && realDepth == tooFarDepth)
                {
                    // dark purple
                    this.depthFrame32[i32 + RedIndex] = 66;
                    this.depthFrame32[i32 + GreenIndex] = 0;
                    this.depthFrame32[i32 + BlueIndex] = 66;
                }
                else if (player == 0 && realDepth == unknownDepth)
                {
                    // dark brown
                    this.depthFrame32[i32 + RedIndex] = 66;
                    this.depthFrame32[i32 + GreenIndex] = 66;
                    this.depthFrame32[i32 + BlueIndex] = 33;
                }
                else
                {
                    // tint the intensity by dividing by per-player values
                    this.depthFrame32[i32 + RedIndex] = (byte)(intensity >> IntensityShiftByPlayerR[player]);
                    this.depthFrame32[i32 + GreenIndex] = (byte)(intensity >> IntensityShiftByPlayerG[player]);
                    this.depthFrame32[i32 + BlueIndex] = (byte)(intensity >> IntensityShiftByPlayerB[player]);
                }
            }

            return this.depthFrame32;
        } 
    }
}
