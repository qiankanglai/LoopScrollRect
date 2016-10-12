using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(LoopScrollRect), true)]
public class LoopScrollRectInspector : Editor
{
	public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        LoopScrollRect scroll = (LoopScrollRect)target;
        scroll.initInStart = EditorGUILayout.Toggle("Init in Start", scroll.initInStart);
		scroll.prefabPool = (MarchingBytes.EasyObjectPool)EditorGUILayout.ObjectField("Prefab Pool", scroll.prefabPool, typeof(MarchingBytes.EasyObjectPool), true);
		scroll.prefabPoolName = EditorGUILayout.TextField("Prefab Pool Name", scroll.prefabPoolName);
        scroll.totalCount = EditorGUILayout.IntField("Total Count", scroll.totalCount);
        scroll.threshold = Mathf.Max(10, EditorGUILayout.FloatField("Threshold", scroll.threshold));
        scroll.reverseDirection = EditorGUILayout.Toggle("Reverse Direction", scroll.reverseDirection);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Clear Cells"))
        {
            scroll.ClearCells();
        }
        if(GUILayout.Button("Refill Cells"))
        {
            scroll.RefillCells();
        }
        EditorGUILayout.EndHorizontal();
	}
}