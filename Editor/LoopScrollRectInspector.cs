using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(LoopScrollRectBase), true)]
    public class LoopScrollRectInspector : Editor
    {
        SerializedProperty m_Content;
        SerializedProperty m_Horizontal;
        SerializedProperty m_Vertical;
        SerializedProperty m_MovementType;
        SerializedProperty m_Elasticity;
        SerializedProperty m_Inertia;
        SerializedProperty m_DecelerationRate;
        SerializedProperty m_ScrollSensitivity;
        SerializedProperty m_Viewport;
        SerializedProperty m_HorizontalScrollbar;
        SerializedProperty m_VerticalScrollbar;
        SerializedProperty m_HorizontalScrollbarVisibility;
        SerializedProperty m_VerticalScrollbarVisibility;
        SerializedProperty m_HorizontalScrollbarSpacing;
        SerializedProperty m_VerticalScrollbarSpacing;
        SerializedProperty m_OnValueChanged;
        AnimBool m_ShowElasticity;
        AnimBool m_ShowDecelerationRate;
        bool m_ViewportIsNotChild, m_HScrollbarIsNotChild, m_VScrollbarIsNotChild;
        static string s_HError = "For this visibility mode, the Viewport property and the Horizontal Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
        static string s_VError = "For this visibility mode, the Viewport property and the Vertical Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";

        //==========LoopScrollRect==========
        SerializedProperty totalCount;
        SerializedProperty reverseDirection;
        
        int firstItem = 0, lastItem = 0, scrollIndex = 0;
        float firstOffset = 0.0f, lastOffset = 0.0f, scrollOffset = 0;
        LoopScrollRectBase.ScrollMode scrollMode = LoopScrollRectBase.ScrollMode.ToStart;
        float scrollSpeed = 1000, scrollTime = 1;

        protected virtual void OnEnable()
        {
            m_Content = serializedObject.FindProperty("m_Content");
            m_Horizontal = serializedObject.FindProperty("m_Horizontal");
            m_Vertical = serializedObject.FindProperty("m_Vertical");
            m_MovementType = serializedObject.FindProperty("m_MovementType");
            m_Elasticity = serializedObject.FindProperty("m_Elasticity");
            m_Inertia = serializedObject.FindProperty("m_Inertia");
            m_DecelerationRate = serializedObject.FindProperty("m_DecelerationRate");
            m_ScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            m_Viewport = serializedObject.FindProperty("m_Viewport");
            m_HorizontalScrollbar = serializedObject.FindProperty("m_HorizontalScrollbar");
            m_VerticalScrollbar = serializedObject.FindProperty("m_VerticalScrollbar");
            m_HorizontalScrollbarVisibility = serializedObject.FindProperty("m_HorizontalScrollbarVisibility");
            m_VerticalScrollbarVisibility = serializedObject.FindProperty("m_VerticalScrollbarVisibility");
            m_HorizontalScrollbarSpacing = serializedObject.FindProperty("m_HorizontalScrollbarSpacing");
            m_VerticalScrollbarSpacing = serializedObject.FindProperty("m_VerticalScrollbarSpacing");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");

            m_ShowElasticity = new AnimBool(Repaint);
            m_ShowDecelerationRate = new AnimBool(Repaint);
            SetAnimBools(true);

            //==========LoopScrollRect==========
            totalCount = serializedObject.FindProperty("totalCount");
            reverseDirection = serializedObject.FindProperty("reverseDirection");
        }

        protected virtual void OnDisable()
        {
            m_ShowElasticity.valueChanged.RemoveListener(Repaint);
            m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
        }

        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
            SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
        }

        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        void CalculateCachedValues()
        {
            m_ViewportIsNotChild = false;
            m_HScrollbarIsNotChild = false;
            m_VScrollbarIsNotChild = false;
            if (targets.Length == 1)
            {
                Transform transform = ((LoopScrollRectBase)target).transform;
                if (m_Viewport.objectReferenceValue == null || ((RectTransform)m_Viewport.objectReferenceValue).transform.parent != transform)
                    m_ViewportIsNotChild = true;
                if (m_HorizontalScrollbar.objectReferenceValue == null || ((Scrollbar)m_HorizontalScrollbar.objectReferenceValue).transform.parent != transform)
                    m_HScrollbarIsNotChild = true;
                if (m_VerticalScrollbar.objectReferenceValue == null || ((Scrollbar)m_VerticalScrollbar.objectReferenceValue).transform.parent != transform)
                    m_VScrollbarIsNotChild = true;
            }
        }

        public override void OnInspectorGUI()
        {
            SetAnimBools(false);

            serializedObject.Update();
            // Once we have a reliable way to know if the object changed, only re-cache in that case.
            CalculateCachedValues();

            EditorGUILayout.LabelField("Scroll Rect", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Content);

            EditorGUILayout.PropertyField(m_Horizontal);
            EditorGUILayout.PropertyField(m_Vertical);

            EditorGUILayout.PropertyField(m_MovementType);
            if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_Elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Inertia);
            if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_DecelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_ScrollSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Viewport);

            EditorGUILayout.PropertyField(m_HorizontalScrollbar);
            if (m_HorizontalScrollbar.objectReferenceValue && !m_HorizontalScrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_HorizontalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                if ((ScrollRect.ScrollbarVisibility)m_HorizontalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                    && !m_HorizontalScrollbarVisibility.hasMultipleDifferentValues)
                {
                    if (m_ViewportIsNotChild || m_HScrollbarIsNotChild)
                        EditorGUILayout.HelpBox(s_HError, MessageType.Error);
                    EditorGUILayout.PropertyField(m_HorizontalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(m_VerticalScrollbar);
            if (m_VerticalScrollbar.objectReferenceValue && !m_VerticalScrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_VerticalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                if ((ScrollRect.ScrollbarVisibility)m_VerticalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                    && !m_VerticalScrollbarVisibility.hasMultipleDifferentValues)
                {
                    if (m_ViewportIsNotChild || m_VScrollbarIsNotChild)
                        EditorGUILayout.HelpBox(s_VError, MessageType.Error);
                    EditorGUILayout.PropertyField(m_VerticalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
                }

                EditorGUI.indentLevel--;
            }
        
            EditorGUILayout.PropertyField(m_OnValueChanged);

            //==========LoopScrollRect==========
            EditorGUILayout.LabelField("Loop Scroll Rect", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(totalCount);
            EditorGUILayout.PropertyField(reverseDirection);
        
            serializedObject.ApplyModifiedProperties();
            
            LoopScrollRectBase scroll = (LoopScrollRectBase)target;
            GUI.enabled = Application.isPlaying;
            const float buttonWidth = 100f;

            #region Basic Usage
            EditorGUILayout.LabelField("Basic Usage", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Clear existing items");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear", GUILayout.Width(buttonWidth)))
            {
                scroll.ClearCells();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Refresh existing items(only update data)");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh", GUILayout.Width(buttonWidth)))
            {
                scroll.RefreshCells();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Refill(0, 0.0f)");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Refill", GUILayout.Width(buttonWidth)))
            {
                scroll.RefillCells();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("RefillFromEnd(0, 0.0f)");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("RefillFromEnd", GUILayout.Width(buttonWidth)))
            {
                scroll.RefillCellsFromEnd();
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Refill Test
            EditorGUILayout.LabelField("Refill Test", EditorStyles.boldLabel);
            
            firstItem = EditorGUILayout.IntField("FirstItem", firstItem);
            firstOffset = EditorGUILayout.FloatField("FirstOffset", firstOffset);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Get first item and offset");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("GetFirstItem", GUILayout.Width(buttonWidth)))
            {
                firstItem = scroll.GetFirstItem(out firstOffset);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Refill with first item and offset");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Refill", GUILayout.Width(buttonWidth)))
            {
                scroll.RefillCells(scroll.reverseDirection ? (scroll.totalCount - firstItem) : firstItem, firstOffset);
            }
            EditorGUILayout.EndHorizontal();
            #endregion
            
            #region Refill Test
            EditorGUILayout.LabelField("RefillFromEnd Test", EditorStyles.boldLabel);
            
            lastItem = EditorGUILayout.IntField("LastItem", lastItem);
            lastOffset = EditorGUILayout.FloatField("LastOffset", lastOffset);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Get last item and offset");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("GetLastItem", GUILayout.Width(buttonWidth)))
            {
                lastItem = scroll.GetLastItem(out lastOffset);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("RefillFromEnd with last item and offset");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("RefillFromEnd", GUILayout.Width(buttonWidth)))
            {
                scroll.RefillCellsFromEnd(scroll.reverseDirection ? lastItem : (scroll.totalCount - lastItem), lastOffset);
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Scroll Test
            EditorGUILayout.LabelField("Scroll Test", EditorStyles.boldLabel);
            scrollIndex = EditorGUILayout.IntField("ScrollIndex", scrollIndex);
            scrollOffset = EditorGUILayout.FloatField("ScrollOffset", scrollOffset);
            scrollMode = (LoopScrollRectBase.ScrollMode)EditorGUILayout.EnumPopup("Mode", scrollMode);
            
            scrollSpeed = EditorGUILayout.FloatField("Speed", scrollSpeed);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scroll to index and offset with speed");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("ScrollToCell", GUILayout.Width(buttonWidth)))
            {
                scroll.ScrollToCell(scrollIndex, scrollSpeed, scrollOffset, scrollMode);
            }
            EditorGUILayout.EndHorizontal();
        
            scrollTime = EditorGUILayout.FloatField("Time", scrollTime);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scroll to index and offset whtin time");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("ScrollToCellWithinTime", GUILayout.Width(buttonWidth)))
            {
                scroll.ScrollToCellWithinTime(scrollIndex, scrollTime, scrollOffset, scrollMode);
            }
            EditorGUILayout.EndHorizontal();
            #endregion
        }
    }
}