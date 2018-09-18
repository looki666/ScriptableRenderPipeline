using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class SerializedCaptureSettings
    {
        public SerializedProperty root;

        public SerializedProperty clearColorMode;
        public SerializedProperty backgroundColorHDR;
        public SerializedProperty clearDepth;

        public SerializedProperty renderingPath;
        public SerializedProperty volumeLayerMask;
        public SerializedProperty volumeAnchorOverride;

        public SerializedProperty aperture;
        public SerializedProperty shutterSpeed;
        public SerializedProperty iso;

        public SerializedProperty shadowDistance;

        public SerializedProperty farClipPlane;
        public SerializedProperty nearClipPlane;
        public SerializedProperty fieldOfview;
        public SerializedProperty useOcclusionCulling;
        public SerializedProperty cullingMask;

        public SerializedCaptureSettings(SerializedProperty root)
        {
            this.root = root;

            clearColorMode = root.Find((CaptureSettings d) => d.clearColorMode);
            backgroundColorHDR = root.Find((CaptureSettings d) => d.backgroundColorHDR);
            clearDepth = root.Find((CaptureSettings d) => d.clearDepth);

            renderingPath = root.Find((CaptureSettings d) => d.renderingPath);
            volumeLayerMask = root.Find((CaptureSettings d) => d.volumeLayerMask);
            volumeAnchorOverride = root.Find((CaptureSettings d) => d.volumeAnchorOverride);

            aperture = root.Find((CaptureSettings d) => d.aperture);
            shutterSpeed = root.Find((CaptureSettings d) => d.shutterSpeed);
            iso = root.Find((CaptureSettings d) => d.iso);

            shadowDistance = root.Find((CaptureSettings d) => d.shadowDistance);

            farClipPlane = root.Find((CaptureSettings d) => d.farClipPlane);
            nearClipPlane = root.Find((CaptureSettings d) => d.nearClipPlane);
            fieldOfview = root.Find((CaptureSettings d) => d.fieldOfview);
            useOcclusionCulling = root.Find((CaptureSettings d) => d.useOcclusionCulling);
            cullingMask = root.Find((CaptureSettings d) => d.cullingMask);
        }
    }
}
