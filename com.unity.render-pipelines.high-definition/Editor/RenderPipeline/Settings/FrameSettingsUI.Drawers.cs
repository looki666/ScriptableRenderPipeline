using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    using CED = CoreEditorDrawer<FrameSettingsUI, SerializedFrameSettings>;

    partial class FrameSettingsUI
    {
        internal static CED.IDrawer Inspector(bool withOverride = true, bool withXR = true)
        {
            return CED.Group(
                    SectionRenderingPasses(withOverride),
                    SectionRenderingSettings(withOverride),
                    CED.FadeGroup(
                        (s, d, o, i) => new AnimBool(withXR),
                        FadeOption.None,
                        SectionXRSettings(withOverride)),
                    SectionLightingSettings(withOverride),
                    CED.Select(
                        (s, d, o) => s.lightLoopSettings,
                        (s, d, o) => d.lightLoopSettings,
                        LightLoopSettingsUI.SectionLightLoopSettings(withOverride)
                        )
                    );
        }

        public static CED.IDrawer SectionRenderingPasses(bool withOverride)
        {
            return CED.FoldoutGroup(
                renderingPassesHeaderContent,
                (s, p, o) => s.isSectionExpandedRenderingPasses,
                FoldoutOption.Indent,
                CED.LabelWidth(200, CED.Action((s, p, o) => Drawer_SectionRenderingPasses(s, p, o, withOverride)))
                );
        }


        public static CED.IDrawer SectionRenderingSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                renderingSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedRenderingSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(300,
                    CED.Action((s, p, o) => Drawer_FieldForwardRenderingOnly(s, p, o, withOverride)),
                    CED.FadeGroup(
                        (s, d, o, i) => s.isSectionExpandedUseForwardOnly,
                        FadeOption.None,
                        CED.Action((s, p, o) => Drawer_FieldUseDepthPrepassWithDefferedRendering(s, p, o, withOverride))
                        ),
                    CED.Action((s, p, o) => Drawer_SectionOtherRenderingSettings(s, p, o, withOverride))
                    )
                );
        }

        public static CED.IDrawer SectionXRSettings(bool withOverride)
        {
            return CED.FadeGroup(
                (s, d, o, i) => s.isSectionExpandedXRSupported,
                FadeOption.None,
                CED.FoldoutGroup(
                    xrSettingsHeaderContent,
                    (s, p, o) => s.isSectionExpandedXRSettings,
                    FoldoutOption.Indent,
                    CED.LabelWidth(200, CED.Action((s, p, o) => Drawer_FieldStereoEnabled(s, p, o, withOverride)))));
        }

        public static CED.IDrawer SectionLightingSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                lightSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedLightingSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(250, CED.Action((s, p, o) => Drawer_SectionLightingSettings(s, p, o, withOverride))));
        }

        static void Drawer_SectionRenderingPasses(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            DrawProperty(p.enableTransparentPrepass, transparentPrepassContent, withOverride, a => p.overridesTransparentPrepass = a, () => p.overridesTransparentPrepass, null);
            DrawProperty(p.enableTransparentPostpass, transparentPostpassContent, withOverride, a => p.overridesTransparentPostpass = a, () => p.overridesTransparentPostpass, null);
            DrawProperty(p.enableMotionVectors, motionVectorContent, withOverride, a => p.overridesMotionVectors = a, () => p.overridesMotionVectors, () => hdrpSettings.supportMotionVectors);
            DrawProperty(p.enableObjectMotionVectors, objectMotionVectorsContent, withOverride, a => p.overridesObjectMotionVectors = a, () => p.overridesObjectMotionVectors, () => hdrpSettings.supportMotionVectors);
            DrawProperty(p.enableDecals, decalsContent, withOverride, a => p.overridesDecals = a, () => p.overridesDecals, () => hdrpSettings.supportDecals);
            DrawProperty(p.enableRoughRefraction, roughRefractionContent, withOverride, a => p.overridesRoughRefraction = a, () => p.overridesRoughRefraction, null);
            DrawProperty(p.enableDistortion, distortionContent, withOverride, a => p.overridesDistortion = a, () => p.overridesDistortion, null);
            DrawProperty(p.enablePostprocess, postprocessContent, withOverride, a => p.overridesPostprocess = a, () => p.overridesPostprocess, null);
        }

        static void Drawer_FieldForwardRenderingOnly(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            DrawProperty(p.enableForwardRenderingOnly, forwardRenderingOnlyContent, withOverride, a => p.overridesForwardRenderingOnly = a, () => p.overridesForwardRenderingOnly, () => hdrpSettings.supportOnlyForward);
        }

        static void Drawer_FieldUseDepthPrepassWithDefferedRendering(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            DrawProperty(p.enableDepthPrepassWithDeferredRendering, depthPrepassWithDeferredRenderingContent, withOverride, a => p.overridesDepthPrepassWithDeferredRendering = a, () => p.overridesDepthPrepassWithDeferredRendering, null);
        }

        static void Drawer_SectionOtherRenderingSettings(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            DrawProperty(p.enableAsyncCompute, asyncComputeContent, withOverride, a => p.overridesAsyncCompute = a, () => p.overridesAsyncCompute, null);

            DrawProperty(p.enableOpaqueObjects, opaqueObjectsContent, withOverride, a => p.overridesOpaqueObjects = a, () => p.overridesOpaqueObjects, null);
            DrawProperty(p.enableTransparentObjects, transparentObjectsContent, withOverride, a => p.overridesTransparentObjects = a, () => p.overridesTransparentObjects, null);

            DrawProperty(p.enableMSAA, msaaContent, withOverride, a => p.overridesMSAA = a, () => p.overridesMSAA, () => hdrpSettings.supportMSAA);
        }

        static void Drawer_FieldStereoEnabled(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            DrawProperty(p.enableStereo, stereoContent, withOverride, a => p.overridesStereo = a, () => p.overridesStereo, null);
        }

        static void Drawer_SectionLightingSettings(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            DrawProperty(p.enableShadow, shadowContent, withOverride, a => p.overridesShadow = a, () => p.overridesShadow, null);
            DrawProperty(p.enableContactShadow, contactShadowContent, withOverride, a => p.overridesContactShadow = a, () => p.overridesContactShadow, null);
            DrawProperty(p.enableShadowMask, shadowMaskContent, withOverride, a => p.overridesShadowMask = a, () => p.overridesShadowMask, () => hdrpSettings.supportShadowMask);
            DrawProperty(p.enableSSR, ssrContent, withOverride, a => p.overridesSSR = a, () => p.overridesSSR, () => hdrpSettings.supportSSR);
            DrawProperty(p.enableSSAO, ssaoContent, withOverride, a => p.overridesSSAO = a, () => p.overridesSSAO, () => hdrpSettings.supportSSAO);
            DrawProperty(p.enableSubsurfaceScattering, subsurfaceScatteringContent, withOverride, a => p.overridesSubsurfaceScattering = a, () => p.overridesSubsurfaceScattering, () => hdrpSettings.supportSubsurfaceScattering);
            DrawProperty(p.enableTransmission, transmissionContent, withOverride, a => p.overridesTransmission = a, () => p.overridesTransmission, null);
            DrawProperty(p.enableAtmosphericScattering, atmosphericScatteringContent, withOverride, a => p.overridesAtmosphericScaterring = a, () => p.overridesAtmosphericScaterring, null);
            DrawProperty(p.enableVolumetric, volumetricContent, withOverride, a => p.overridesVolumetrics = a, () => p.overridesVolumetrics, () => hdrpSettings.supportVolumetrics);
            DrawProperty(p.enableLightLayers, lightLayerContent, withOverride, a => p.overridesLightLayers = a, () => p.overridesLightLayers, () => hdrpSettings.supportLightLayers);
        }

        /// <summary>Internal only drawer to support overridable checkbox on left if property could be overrided</summary>
        /// <param name="p">The property to draw</param>
        /// <param name="c">Label and ToolTip</param>
        /// <param name="withOverride">Override possible?</param>
        /// <param name="setter">what to do when value change</param>
        /// <param name="getter">where to get value</param>
        /// <param name="enabled">if not null and return false, the value will be display as disabled without the overrideable checkbox</param>
        internal static void DrawProperty(SerializedProperty p, GUIContent c, bool withOverride, Action<bool> setter, Func<bool> getter, Func<bool> enabled)
        {
            --EditorGUI.indentLevel;    //alignment provided by the space for override checkbox
            withOverride &= enabled == null || enabled();
            bool shouldBeDisabled = withOverride || (enabled != null && !enabled());
            using (new EditorGUILayout.HorizontalScope())
            {
                var overrideRect = GUILayoutUtility.GetRect(15f, 17f, GUILayout.ExpandWidth(false)); //15 = kIndentPerLevel
                if (withOverride)
                {
                    bool originalValue = getter();
                    bool modifiedValue = originalValue;
                    overrideRect.yMin += 4f;
                    modifiedValue = GUI.Toggle(overrideRect, originalValue, overrideTooltip, CoreEditorStyles.smallTickbox);

                    if (originalValue ^ modifiedValue)
                    {
                        setter(modifiedValue);
                    }

                    shouldBeDisabled = !modifiedValue;
                }
                using (new EditorGUI.DisabledScope(shouldBeDisabled))
                {
                    EditorGUILayout.PropertyField(p, c);
                }
                ++EditorGUI.indentLevel;
            }
        }
    }
}
