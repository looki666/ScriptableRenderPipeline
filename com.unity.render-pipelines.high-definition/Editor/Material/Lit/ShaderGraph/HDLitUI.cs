using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.ShaderGraph
{
    public class HDLitGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            materialEditor.PropertiesDefaultGUI(props);
            if (materialEditor.EmissionEnabledProperty())
            {
                // Use the overload version of this function once the following PR is merged: Pull request #74105
                materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
                //materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true, true);
            }

            var material = (Material)materialEditor.target;
            if (material != null)
            {
                bool enabled = material.GetShaderPassEnabled(HDShaderPassNames.s_MotionVectorsStr);
                EditorGUI.BeginChangeCheck();
                enabled = EditorGUILayout.Toggle("Enable Motion Vector For Vertex Animation", enabled);
                if (EditorGUI.EndChangeCheck())
                {
                    material.SetShaderPassEnabled(HDShaderPassNames.s_MotionVectorsStr, enabled);
                }
            }
        }
    }
}
