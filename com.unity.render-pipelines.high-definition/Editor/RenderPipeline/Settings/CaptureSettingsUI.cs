using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    using CED = CoreEditorDrawer<CaptureSettingsUI, SerializedCaptureSettings>;

    partial class CaptureSettingsUI : BaseUI<SerializedFrameSettings>
    {
        const string captureSettingsHeaderContent = "Capture Settings";
        static readonly GUIContent clearColorModeContent = CoreEditorUtils.GetContent("Clear Mode");
        static readonly GUIContent backgroundColorHDRContent = CoreEditorUtils.GetContent("Background Color");
        static readonly GUIContent clearDepthContent = CoreEditorUtils.GetContent("Clear Depth");
        static readonly GUIContent renderingPathContent = CoreEditorUtils.GetContent("Rendering Path");
        static readonly GUIContent volumeLayerMaskContent = CoreEditorUtils.GetContent("Volume Layer Mask");
        static readonly GUIContent volumeAnchorOverrideContent = CoreEditorUtils.GetContent("Volume Anchor Override");
        static readonly GUIContent apertureContent = CoreEditorUtils.GetContent("Aperture");
        static readonly GUIContent shutterSpeedContent = CoreEditorUtils.GetContent("Shutter Speed");
        static readonly GUIContent isoContent = CoreEditorUtils.GetContent("Iso");
        static readonly GUIContent shadowDistanceContent = CoreEditorUtils.GetContent("Shadow Distance");
        static readonly GUIContent farClipPlaneContent = CoreEditorUtils.GetContent("Far Clip Plane");
        static readonly GUIContent nearClipPlaneContent = CoreEditorUtils.GetContent("Near Clip Plane");
        static readonly GUIContent fieldOfviewContent = CoreEditorUtils.GetContent("Field Of View");
        static readonly GUIContent useOcclusionCullingContent = CoreEditorUtils.GetContent("Occlusion Culling");
        static readonly GUIContent cullingMaskContent = CoreEditorUtils.GetContent("Culling Mask");
        
        public static CED.IDrawer SectionCaptureSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                captureSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedCaptureSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(250, CED.Action((s, p, o) => Drawer_SectionCaptureSettings(s, p, o, withOverride))));
        }

        public AnimBool isSectionExpandedCaptureSettings { get { return m_AnimBools[0]; } }

        public CaptureSettingsUI()
            : base(1)
        {
        }

        static void Drawer_SectionCaptureSettings(CaptureSettingsUI s, SerializedCaptureSettings p, Editor owner, bool withOverride)
        {
            OverridableSettingsArea area = new OverridableSettingsArea();
            area.Add(p.clearColorMode, clearColorModeContent, a => p.overridesClearColorMode = a, () => p.overridesClearColorMode);
            area.Add(p.backgroundColorHDR, backgroundColorHDRContent, a => p.overridesBackgroundColorHDR = a, () => p.overridesBackgroundColorHDR);
            area.Add(p.clearDepth, clearDepthContent, a => p.overridesClearDepth = a, () => p.overridesClearDepth);
            area.Add(p.renderingPath, renderingPathContent, a => p.overridesRenderingPath = a, () => p.overridesRenderingPath);
            area.Add(p.volumeLayerMask, volumeLayerMaskContent, a => p.overridesVolumeLayerMask = a, () => p.overridesVolumeLayerMask);
            area.Add(p.volumeAnchorOverride, volumeAnchorOverrideContent, a => p.overridesVolumeAnchorOverride = a, () => p.overridesVolumeAnchorOverride);
            area.Add(p.aperture, apertureContent, a => p.overridesAperture = a, () => p.overridesAperture);
            area.Add(p.shutterSpeed, shutterSpeedContent, a => p.overridesShutterSpeed = a, () => p.overridesShutterSpeed);
            area.Add(p.iso, isoContent, a => p.overridesIso = a, () => p.overridesIso);
            area.Add(p.shadowDistance, shadowDistanceContent, a => p.overridesShadowDistance = a, () => p.overridesShadowDistance);
            area.Add(p.farClipPlane, farClipPlaneContent, a => p.overridesFarClip = a, () => p.overridesFarClip);
            area.Add(p.nearClipPlane, nearClipPlaneContent, a => p.overridesNearClip = a, () => p.overridesNearClip);
            area.Add(p.fieldOfview, fieldOfviewContent, a => p.overridesFieldOfview = a, () => p.overridesFieldOfview);
            area.Add(p.useOcclusionCulling, useOcclusionCullingContent, a => p.overridesUseOcclusionCulling = a, () => p.overridesUseOcclusionCulling);
            area.Add(p.cullingMask, cullingMaskContent, a => p.overridesCullingMask = a, () => p.overridesCullingMask);
            area.Draw(withOverride);
        }
    }
}
