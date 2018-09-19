using UnityEditor.AnimatedValues;
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
                CED.LabelWidth(200, CED.Action((s, p, o) => Drawer_SectionRenderingPasses(s, p, o, withOverride))),
                CED.space
                );
        }


        public static CED.IDrawer SectionRenderingSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                renderingSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedRenderingSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(300, CED.Action((s, p, o) => Drawer_SectionRenderingSettings(s, p, o, withOverride))),
                CED.space
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
                    CED.LabelWidth(200, CED.Action((s, p, o) => Drawer_FieldStereoEnabled(s, p, o, withOverride))),
                    CED.space));
        }

        public static CED.IDrawer SectionLightingSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                lightSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedLightingSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(250, CED.Action((s, p, o) => Drawer_SectionLightingSettings(s, p, o, withOverride))),
                CED.space);
        }

        static void Drawer_SectionRenderingPasses(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            OverridableSettingsArea area = new OverridableSettingsArea();
            area.Add(p.enableTransparentPrepass, transparentPrepassContent, a => p.overridesTransparentPrepass = a, () => p.overridesTransparentPrepass);
            area.Add(p.enableTransparentPostpass, transparentPostpassContent, a => p.overridesTransparentPostpass = a, () => p.overridesTransparentPostpass);
            area.Add(p.enableMotionVectors, motionVectorContent, a => p.overridesMotionVectors = a, () => p.overridesMotionVectors, () => hdrpSettings.supportMotionVectors);
            area.Add(p.enableObjectMotionVectors, objectMotionVectorsContent, a => p.overridesObjectMotionVectors = a, () => p.overridesObjectMotionVectors, () => hdrpSettings.supportMotionVectors);
            area.Add(p.enableDecals, decalsContent, a => p.overridesDecals = a, () => p.overridesDecals, () => hdrpSettings.supportDecals);
            area.Add(p.enableRoughRefraction, roughRefractionContent, a => p.overridesRoughRefraction = a, () => p.overridesRoughRefraction);
            area.Add(p.enableDistortion, distortionContent, a => p.overridesDistortion = a, () => p.overridesDistortion);
            area.Add(p.enablePostprocess, postprocessContent, a => p.overridesPostprocess = a, () => p.overridesPostprocess);
            area.Draw(withOverride);
        }

        static void Drawer_SectionRenderingSettings(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            OverridableSettingsArea area = new OverridableSettingsArea();
            area.Add(p.enableForwardRenderingOnly, forwardRenderingOnlyContent, a => p.overridesForwardRenderingOnly = a, () => p.overridesForwardRenderingOnly, () => hdrpSettings.supportOnlyForward);
            if (s.isSectionExpandedUseForwardOnly.target)
            {
                area.Add(p.enableDepthPrepassWithDeferredRendering, depthPrepassWithDeferredRenderingContent, a => p.overridesDepthPrepassWithDeferredRendering = a, () => p.overridesDepthPrepassWithDeferredRendering);
            }
            area.Add(p.enableAsyncCompute, asyncComputeContent, a => p.overridesAsyncCompute = a, () => p.overridesAsyncCompute);
            area.Add(p.enableOpaqueObjects, opaqueObjectsContent, a => p.overridesOpaqueObjects = a, () => p.overridesOpaqueObjects);
            area.Add(p.enableTransparentObjects, transparentObjectsContent, a => p.overridesTransparentObjects = a, () => p.overridesTransparentObjects);
            area.Add(p.enableMSAA, msaaContent, a => p.overridesMSAA = a, () => p.overridesMSAA, () => hdrpSettings.supportMSAA);
            area.Draw(withOverride);
        }
        
        static void Drawer_SectionLightingSettings(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            OverridableSettingsArea area = new OverridableSettingsArea();
            area.Add(p.enableShadow, shadowContent, a => p.overridesShadow = a, () => p.overridesShadow);
            area.Add(p.enableContactShadow, contactShadowContent, a => p.overridesContactShadow = a, () => p.overridesContactShadow);
            area.Add(p.enableShadowMask, shadowMaskContent, a => p.overridesShadowMask = a, () => p.overridesShadowMask, () => hdrpSettings.supportShadowMask);
            area.Add(p.enableSSR, ssrContent, a => p.overridesSSR = a, () => p.overridesSSR, () => hdrpSettings.supportSSR);
            area.Add(p.enableSSAO, ssaoContent, a => p.overridesSSAO = a, () => p.overridesSSAO, () => hdrpSettings.supportSSAO);
            area.Add(p.enableSubsurfaceScattering, subsurfaceScatteringContent, a => p.overridesSubsurfaceScattering = a, () => p.overridesSubsurfaceScattering, () => hdrpSettings.supportSubsurfaceScattering);
            area.Add(p.enableTransmission, transmissionContent, a => p.overridesTransmission = a, () => p.overridesTransmission);
            area.Add(p.enableAtmosphericScattering, atmosphericScatteringContent, a => p.overridesAtmosphericScaterring = a, () => p.overridesAtmosphericScaterring);
            area.Add(p.enableVolumetric, volumetricContent, a => p.overridesVolumetrics = a, () => p.overridesVolumetrics, () => hdrpSettings.supportVolumetrics);
            area.Add(p.enableLightLayers, lightLayerContent, a => p.overridesLightLayers = a, () => p.overridesLightLayers, () => hdrpSettings.supportLightLayers);
            area.Draw(withOverride);
        }

        static void Drawer_FieldStereoEnabled(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            OverridableSettingsArea area = new OverridableSettingsArea();
            area.Add(p.enableStereo, stereoContent, a => p.overridesStereo = a, () => p.overridesStereo, null);
            area.Draw(withOverride);
        }
    }
}
