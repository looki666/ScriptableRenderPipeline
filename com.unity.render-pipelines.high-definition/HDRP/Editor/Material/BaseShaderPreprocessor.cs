using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    // returns true if the variant should be stripped.
    public delegate bool VariantStrippingFunc(HDRenderPipelineAsset hdrpAsset, Shader shader, ShaderSnippetData snippet, ShaderCompilerData inputData);

    public class BaseShaderPreprocessor
    {
        protected ShaderKeyword m_ShadowMask;
        protected ShaderKeyword m_Transparent;
        protected ShaderKeyword m_DebugDisplay;
        protected ShaderKeyword m_TileLighting;
        protected ShaderKeyword m_ClusterLighting;
        protected ShaderKeyword m_LodFadeCrossFade;
        protected ShaderKeyword m_DecalsOFF;
        protected ShaderKeyword m_Decals3RT;
        protected ShaderKeyword m_Decals4RT;
        protected ShaderKeyword m_LightLayers;
        protected ShaderKeyword m_PunctualLow;
        protected ShaderKeyword m_PunctualMedium;
        protected ShaderKeyword m_PunctualHigh;
        protected ShaderKeyword m_DirectionalLow;
        protected ShaderKeyword m_DirectionalMedium;
        protected ShaderKeyword m_DirectionalHigh;
        
        Dictionary<HDShadowQuality, ShaderKeyword> m_PunctualShadowVariants;
        Dictionary<HDShadowQuality, ShaderKeyword> m_DirectionalShadowVariants;

        public BaseShaderPreprocessor()
        {
            m_Transparent = new ShaderKeyword("_SURFACE_TYPE_TRANSPARENT");
            m_DebugDisplay = new ShaderKeyword("DEBUG_DISPLAY");
            m_TileLighting = new ShaderKeyword("USE_FPTL_LIGHTLIST");
            m_ClusterLighting = new ShaderKeyword("USE_CLUSTERED_LIGHTLIST");
            m_LodFadeCrossFade = new ShaderKeyword("LOD_FADE_CROSSFADE");
            m_DecalsOFF = new ShaderKeyword("DECALS_OFF");
            m_Decals3RT = new ShaderKeyword("DECALS_3RT");
            m_Decals4RT = new ShaderKeyword("DECALS_4RT");
            m_LightLayers = new ShaderKeyword("LIGHT_LAYERS");
            m_PunctualLow = new ShaderKeyword("PUNCTUAL_SHADOW_LOW");
            m_PunctualMedium = new ShaderKeyword("PUNCTUAL_SHADOW_MEDIUM");
            m_PunctualHigh = new ShaderKeyword("PUNCTUAL_SHADOW_HIGH");
            m_DirectionalLow = new ShaderKeyword("DIRECTIONAL_SHADOW_LOW");
            m_DirectionalMedium = new ShaderKeyword("DIRECTIONAL_SHADOW_MEDIUM");
            m_DirectionalHigh = new ShaderKeyword("DIRECTIONAL_SHADOW_HIGH");
            
            m_PunctualShadowVariants = new Dictionary<HDShadowQuality, ShaderKeyword>
            {
                {HDShadowQuality.Low, m_PunctualLow},
                {HDShadowQuality.Medium, m_PunctualMedium},
                {HDShadowQuality.High, m_PunctualHigh},
            };
            m_DirectionalShadowVariants = new Dictionary<HDShadowQuality, ShaderKeyword>
            {
                {HDShadowQuality.Low, m_DirectionalLow},
                {HDShadowQuality.Medium, m_DirectionalMedium},
                {HDShadowQuality.High, m_DirectionalHigh},
            };
        }

        public virtual void AddStripperFuncs(Dictionary<string, VariantStrippingFunc> stripperFuncs) {}

        // NOTE: All these keyword should be automatically stripped so there's no need to handle them ourselves.
        // LIGHTMAP_ON, DIRLIGHTMAP_COMBINED, DYNAMICLIGHTMAP_ON, LIGHTMAP_SHADOW_MIXING, SHADOWS_SHADOWMASK
        // FOG_LINEAR, FOG_EXP, FOG_EXP2
        // STEREO_INSTANCING_ON, STEREO_MULTIVIEW_ON, STEREO_CUBEMAP_RENDER_ON, UNITY_SINGLE_PASS_STEREO
        // INSTANCING_ON

        // Several pass are common to all shader, let's share code here
        // This remove variant (return true) for:
        // - Scene Selection
        // - Motion vectors
        // - Tile pass for Transparent (not compatible)
        // -
        protected bool CommonShaderStripper(HDRenderPipelineAsset hdrpAsset, Shader shader, ShaderSnippetData snippet, ShaderCompilerData inputData)
        {
            // Strip every useless shadow configs
            // TODO: test if it actually works
            var shadowInitParams = hdrpAsset.renderPipelineSettings.hdShadowInitParams;
            foreach (var punctualShadowVariant in m_PunctualShadowVariants)
            {
                if (punctualShadowVariant.Key != shadowInitParams.punctualShadowQuality)
                    if (inputData.shaderKeywordSet.IsEnabled(punctualShadowVariant.Value))
                        return true;
            }
            foreach (var directionalShadowVariant in m_DirectionalShadowVariants)
            {
                if (directionalShadowVariant.Key != shadowInitParams.directionalShadowQuality)
                    if (inputData.shaderKeywordSet.IsEnabled(directionalShadowVariant.Value))
                        return true;
            }

            bool isSceneSelectionPass = snippet.passName == "SceneSelectionPass";
            if (isSceneSelectionPass)
                return true;

            bool isMotionPass = snippet.passName == "Motion Vectors";
            if (!hdrpAsset.renderPipelineSettings.supportMotionVectors && isMotionPass)
                return true;

            //bool isForwardPass = (snippet.passName == "Forward") || (snippet.passName == "ForwardOnly");

            if (inputData.shaderKeywordSet.IsEnabled(m_Transparent))
            {
                // If we are transparent we use cluster lighting and not tile lighting
                if (inputData.shaderKeywordSet.IsEnabled(m_TileLighting))
                    return true;
            }
            else // Opaque
            {
                // Note: we can't assume anything regarding tile/cluster for opaque as multiple view could used different settings and it depends on MSAA
            }

            // TODO: If static lighting we can remove meta pass, but how to know that?

            // If we are in a release build, don't compile debug display variant
            // Also don't compile it if not requested by the render pipeline settings
            if ((/*!Debug.isDebugBuild || */ !hdrpAsset.renderPipelineSettings.supportRuntimeDebugDisplay) && inputData.shaderKeywordSet.IsEnabled(m_DebugDisplay))
                return true;

            if (inputData.shaderKeywordSet.IsEnabled(m_LodFadeCrossFade) && !hdrpAsset.renderPipelineSettings.supportDitheringCrossFade)
                return true;

            // Decal case

            // If decal support, remove unused variant
            if (hdrpAsset.renderPipelineSettings.supportDecals)
            {
                // Remove the no decal case
                if (inputData.shaderKeywordSet.IsEnabled(m_DecalsOFF))
                    return true;

                // If decal but with 4RT remove 3RT variant and vice versa
                if (inputData.shaderKeywordSet.IsEnabled(m_Decals3RT) && hdrpAsset.renderPipelineSettings.decalSettings.perChannelMask)
                    return true;

                if (inputData.shaderKeywordSet.IsEnabled(m_Decals4RT) && !hdrpAsset.renderPipelineSettings.decalSettings.perChannelMask)
                    return true;
            }
            else
            {
                // If no decal support, remove decal variant
                if (inputData.shaderKeywordSet.IsEnabled(m_Decals3RT) || inputData.shaderKeywordSet.IsEnabled(m_Decals4RT))
                    return true;
            }


            if (inputData.shaderKeywordSet.IsEnabled(m_LightLayers) && !hdrpAsset.renderPipelineSettings.supportLightLayers)
                return true;

            return false;
        }
    }
}
