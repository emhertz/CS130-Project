using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuiDeviceFramework.datatypes
{
    public class NuiSkeleton
    {
        private NuiBoneOrientationCollection boneOrientations;
        private NuiFrameEdges clippedEdges;
        private NuiSkeletonPoint position;
        private int trackingId;
        private NuiSkeletonTrackingState trackingState;
        private NuiJointCollection joints;

        public NuiBoneOrientationCollection BoneOrientations { get { return this.boneOrientations; } }
        public NuiFrameEdges ClippedEdges { get { return this.clippedEdges; } set { this.clippedEdges = value; } }
        public NuiSkeletonPoint Position { get { return this.position; } set { this.position = value; } }
        public int TrackingId { get { return this.trackingId; } set { this.trackingId = value; } }
        public NuiSkeletonTrackingState TrackingState { get { return this.trackingState; } set { this.trackingState = value; } }
        public NuiJointCollection Joints { get { return this.joints; } set { this.joints = value; } }

        public NuiSkeleton(
            NuiBoneOrientationCollection nboc,
            NuiFrameEdges ce,
            NuiSkeletonPoint p,
            int tid,
            NuiSkeletonTrackingState ts,
            NuiJointCollection j)
        {
            this.boneOrientations = nboc;
            this.ClippedEdges = ce;
            this.Position = p;
            this.TrackingId = tid;
            this.TrackingState = ts;
            this.Joints = j;
        }
    }
}
