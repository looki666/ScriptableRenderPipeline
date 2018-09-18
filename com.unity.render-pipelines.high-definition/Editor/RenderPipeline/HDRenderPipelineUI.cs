using UnityEngine.Events;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    using _ = CoreEditorUtils;
    using CED = CoreEditorDrawer<HDRenderPipelineUI, SerializedHDRenderPipelineAsset>;

    class HDRenderPipelineUI : BaseUI<SerializedHDRenderPipelineAsset>
    {
        public static readonly GUIContent defaultFrameSettingsContent = CoreEditorUtils.GetContent("Default Frame Settings For");
        public static readonly GUIContent renderPipelineResourcesContent = CoreEditorUtils.GetContent("Render Pipeline Resources|Set of resources that need to be loaded when creating stand alone");
        public static readonly GUIContent diffusionProfileSettingsContent = CoreEditorUtils.GetContent("Diffusion Profile Settings");
        public static readonly GUIContent enableShaderVariantStrippingContent = CoreEditorUtils.GetContent("Enable Shader Variant Stripping (experimental)");

        enum SelectedFrameSettings { Camera, CubeReflection, PlanarReflection };
        static SelectedFrameSettings selectedFrameSettings = SelectedFrameSettings.Camera;

        static HDRenderPipelineUI()
        {
            Inspector = CED.Group(
                SectionPrimarySettings,
                CED.space,
                CED.Select(
                    (s, d, o) => s.renderPipelineSettings,
                    (s, d, o) => d.renderPipelineSettings,
                    RenderPipelineSettingsUI.Inspector
                    ),
                CED.space,
                FrameSettingsSection
            );
        }

        public static readonly CED.IDrawer Inspector;

        public static readonly CED.IDrawer SectionPrimarySettings = CED.Action(Drawer_SectionPrimarySettings);
        
        public static readonly CED.IDrawer FrameSettingsSection = CED.Group(
            CED.Action(Drawer_TitleDefaultFrameSettings),
            CED.FadeGroup(
                (s, d, o, i) => s.isSectionExpandedCamera,
                FadeOption.None,
                CED.Select(
                    (s, d, o) => s.defaultFrameSettings,
                    (s, d, o) => d.defaultFrameSettings,
                    FrameSettingsUI.Inspector(withOverride: false)
                    )
                ),
            CED.FadeGroup(
                (s, d, o, i) => s.isSectionExpandedCubeReflection,
                FadeOption.None,
                //CED.Select(
                //    (s, d, o) => s.defaultCubeReflectionCaptureSettings,
                //    (s, d, o) => d.defaultCubeReflectionCaptureSettings,
                //    CaptureSettingsUI.SectionCaptureSettings(withOverride: false)
                //    ),
                CED.Select(
                    (s, d, o) => s.defaultCubeReflectionFrameSettings,
                    (s, d, o) => d.defaultCubeReflectionFrameSettings,
                    FrameSettingsUI.Inspector(withOverride: false)
                    )
                ),
            CED.FadeGroup(
                (s, d, o, i) => s.isSectionExpandedPlanarReflection,
                FadeOption.None,
                //CED.Select(
                //    (s, d, o) => s.defaultPlanarReflectionFrameSettings,
                //    (s, d, o) => d.defaultPlanarReflectionFrameSettings,
                //    CaptureSettingsUI.SectionCaptureSettings(withOverride: false)
                //    ),
                CED.Select(
                    (s, d, o) => s.defaultPlanarReflectionFrameSettings,
                    (s, d, o) => d.defaultPlanarReflectionFrameSettings,
                    FrameSettingsUI.Inspector(withOverride: false)
                    )
                )
            );

        public FrameSettingsUI defaultFrameSettings = new FrameSettingsUI();
        public FrameSettingsUI defaultCubeReflectionFrameSettings = new FrameSettingsUI();
        public FrameSettingsUI defaultPlanarReflectionFrameSettings = new FrameSettingsUI();
        //public CaptureSettingsUI defaultCubeReflectionCaptureSettings = new CaptureSettingsUI();
        //public CaptureSettingsUI defaultPlanarReflectionCaptureSettings = new CaptureSettingsUI();
        public RenderPipelineSettingsUI renderPipelineSettings = new RenderPipelineSettingsUI();
        
        public AnimBool isSectionExpandedCamera { get { return m_AnimBools[0]; } }
        public AnimBool isSectionExpandedCubeReflection { get { return m_AnimBools[1]; } }
        public AnimBool isSectionExpandedPlanarReflection { get { return m_AnimBools[2]; } }

        public HDRenderPipelineUI()
            : base(3)
        {
            isSectionExpandedCamera.value = true;
        }

        public override void Reset(SerializedHDRenderPipelineAsset data, UnityAction repaint)
        {
            renderPipelineSettings.Reset(data.renderPipelineSettings, repaint);
            defaultFrameSettings.Reset(data.defaultFrameSettings, repaint);
            defaultCubeReflectionFrameSettings.Reset(data.defaultCubeReflectionFrameSettings, repaint);
            defaultPlanarReflectionFrameSettings.Reset(data.defaultPlanarReflectionFrameSettings, repaint);
            base.Reset(data, repaint);
        }

        public override void Update()
        {
            renderPipelineSettings.Update();
            defaultFrameSettings.Update();
            defaultCubeReflectionFrameSettings.Update();
            defaultPlanarReflectionFrameSettings.Update();
            base.Update();
        }

        static void Drawer_TitleDefaultFrameSettings(HDRenderPipelineUI s, SerializedHDRenderPipelineAsset d, Editor o)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(defaultFrameSettingsContent, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            selectedFrameSettings = (SelectedFrameSettings)EditorGUILayout.EnumPopup(selectedFrameSettings);
            if(EditorGUI.EndChangeCheck())
            {
                s.isSectionExpandedCamera.value = false;
                s.isSectionExpandedCubeReflection.value = false;
                s.isSectionExpandedPlanarReflection.value = false;
                switch(selectedFrameSettings)
                {
                    case SelectedFrameSettings.Camera:
                        s.isSectionExpandedCamera.value = true;
                        break;
                    case SelectedFrameSettings.CubeReflection:
                        s.isSectionExpandedCubeReflection.value = true;
                        break;
                    case SelectedFrameSettings.PlanarReflection:
                        s.isSectionExpandedPlanarReflection.value = true;
                        break;
                }
            }
            GUILayout.EndHorizontal();
        }

        static void Drawer_SectionPrimarySettings(HDRenderPipelineUI s, SerializedHDRenderPipelineAsset d, Editor o)
        {
            EditorGUILayout.PropertyField(d.renderPipelineResources, renderPipelineResourcesContent);
            EditorGUILayout.PropertyField(d.diffusionProfileSettings, diffusionProfileSettingsContent);
            EditorGUILayout.PropertyField(d.allowShaderVariantStripping, enableShaderVariantStrippingContent);
        }
    }
}
