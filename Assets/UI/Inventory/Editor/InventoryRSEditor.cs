using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(InventoryRS))]
public class InventoryRSEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.DrawDefaultInspector ();
		EditorGUILayout.Space ();
		InventoryRS RS = (InventoryRS)target;
		if (GUILayout.Button ("Clear")) {
			RS.ClearInv ();
		}
	}
}
