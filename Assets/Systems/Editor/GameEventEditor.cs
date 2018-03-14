using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(GameEvent))]
public class GameEventEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.DrawDefaultInspector ();
		EditorGUILayout.Space ();
		GameEvent GE = (GameEvent)target;
		if (GUILayout.Button ("Raise")) {
			GE.Raise ();
		}
	}
}
