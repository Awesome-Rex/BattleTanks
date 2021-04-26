using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using REXTools.EditorTools;

namespace REXTools.Tiling
{
    public class TilingRuleWindow : EditorWindow
    {
        //public static System.Type activeType = null;


        //properties and UI settings
        private bool isVertical
        {
            get
            {
                return position.width < position.height;
            }
        }
        private float maxButtonSize;

        //methods
        //[MenuItem("Window/Tiling Rule Settings")]
        public static void ShowWindow()
        {
            GetWindow<TilingRuleWindow>("Tiling Rule Settings");
        }

        private void DrawTilingRuleSettings()
        {
            if (!isVertical) // horizontal
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    float buttonSize = position.height - (EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 3f));
                    float buttonSizeClamped = Mathf.Clamp(buttonSize, 0f, maxButtonSize);

                    EditorGUILayout.BeginVertical(GUILayout.Width(buttonSizeClamped * 3f));
                    {
                        EditorGUILayout.LabelField("Self", EditorStyles.boldLabel, GUILayout.Width(buttonSizeClamped * 3f));

                        EditorGUILayout.BeginHorizontal();
                        CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("Cycle", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), null, ref TilingRulePropertyDrawer.brushType);
                        CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("Eraser", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), RuleBehaviour.DontCare, ref TilingRulePropertyDrawer.brushType);
                        CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("This", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), RuleBehaviour.This, ref TilingRulePropertyDrawer.brushType);
                        CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("NotThis", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), RuleBehaviour.NotThis, ref TilingRulePropertyDrawer.brushType);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    //vertical line
                    EditorGUILayout.BeginVertical(GUILayout.Width(1f));
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(EditorGUIUtility.singleLineHeight / 2f));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            else // vertical
            {
                float buttonSize = position.width - (EditorGUIUtility.standardVerticalSpacing * 2f);

                EditorGUILayout.LabelField("Self", EditorStyles.boldLabel, GUILayout.Width(buttonSize));

                CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("Cycle", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), null, ref TilingRulePropertyDrawer.brushType);
                CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("Eraser", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), RuleBehaviour.DontCare, ref TilingRulePropertyDrawer.brushType);
                CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("This", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), RuleBehaviour.This, ref TilingRulePropertyDrawer.brushType);
                CustomEditors.EnumButton<RuleBehaviour?>(this, () => GUILayout.Button("NotThis", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), RuleBehaviour.NotThis, ref TilingRulePropertyDrawer.brushType);
            }
        }
            
        private void OnEnable()
        {
            maxButtonSize = 60f;
            minSize = new Vector2(maxButtonSize, maxButtonSize + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            TilingRulePropertyDrawer.brushType = null;
        }

        private Vector2 scrollPos;
        public void OnGUI()
        {
            if (!isVertical) // horizontal
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            }
            else
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUIStyle.none);
            }

            DrawTilingRuleSettings();

            EditorGUILayout.EndScrollView();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}