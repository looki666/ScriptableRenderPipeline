using System;
using System.Collections.Generic;
//using UnityEditor;
//using UnityEditor.Experimental.Rendering;
using UnityEngine;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    internal struct OverridableSettingsArea
    {
        static readonly GUIContent overrideTooltip = CoreEditorUtils.GetContent("|Override this setting in component.");

        private struct Field
        {
            public SerializedProperty property;
            public GUIContent content;
            public Action<bool> setter;
            public Func<bool> getter;
            public Func<bool> enabler;
            public int indent;
            public bool enabled { get { return enabler == null || enabler(); } }
        }
        private List<Field> fields;

        public void Add(SerializedProperty property, GUIContent content, Action<bool> setter, Func<bool> getter, Func<bool> enabler = null, int indent = 0)
        {
            if (fields == null)
                fields = new List<Field>();
            fields.Add(new Field { property = property, content = content, setter = setter, getter = getter, enabler = enabler, indent = indent });
        }

        public void Draw(bool withOverride)
        {
            if (fields == null)
            {
                return;
            }
            if (withOverride)
            {
                OverridesHeaders();
            }
            foreach (var field in fields)
            {
                DrawField(field, withOverride);
            }
        }

        void DrawField(Field field, bool withOverride)
        {
            if (field.indent == 0)
            {
                --EditorGUI.indentLevel;    //alignment provided by the space for override checkbox
            }
            else
            {
                for (int i = field.indent - 1; i > 0; --i)
                {
                    ++EditorGUI.indentLevel;
                }
            }
            bool enabled = field.enabled;
            withOverride &= enabled;
            bool shouldBeDisabled = withOverride || !enabled;
            using (new EditorGUILayout.HorizontalScope())
            {
                var overrideRect = GUILayoutUtility.GetRect(15f, 17f, GUILayout.ExpandWidth(false)); //15 = kIndentPerLevel
                if (withOverride)
                {
                    bool originalValue = field.getter();
                    bool modifiedValue = originalValue;
                    overrideRect.yMin += 4f;
                    modifiedValue = GUI.Toggle(overrideRect, originalValue, overrideTooltip, CoreEditorStyles.smallTickbox);

                    if (originalValue ^ modifiedValue)
                    {
                        field.setter(modifiedValue);
                    }

                    shouldBeDisabled = !modifiedValue;
                }
                using (new EditorGUI.DisabledScope(shouldBeDisabled))
                {
                    EditorGUILayout.PropertyField(field.property, field.content);
                }
                if (field.indent == 0)
                {
                    ++EditorGUI.indentLevel;
                }
                else
                {
                    for (int i = field.indent - 1; i > 0; --i)
                    {
                        --EditorGUI.indentLevel;
                    }
                }
            }
        }

        void OverridesHeaders()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayoutUtility.GetRect(0f, 17f, GUILayout.ExpandWidth(false));
                if (GUILayout.Button(CoreEditorUtils.GetContent("All|Toggle all overrides on. To maximize performances you should only toggle overrides that you actually need."), CoreEditorStyles.miniLabelButton, GUILayout.Width(17f), GUILayout.ExpandWidth(false)))
                {
                    foreach (var field in fields)
                    {
                        if (field.enabled)
                            field.setter(true);
                    }
                }

                if (GUILayout.Button(CoreEditorUtils.GetContent("None|Toggle all overrides off."), CoreEditorStyles.miniLabelButton, GUILayout.Width(32f), GUILayout.ExpandWidth(false)))
                {
                    foreach (var field in fields)
                    {
                        if (field.enabled)
                            field.setter(false);
                    }
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}
