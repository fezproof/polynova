using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CustomEditor(typeof(NetworkPoolerManager))]
public class PoolManagerInspector : Editor {

	SerializedProperty objectsToPool;
	SerializedProperty amountsToPool;
	SerializedProperty willGrows;
	SerializedProperty growAmounts;

	void OnEnable()
	{
		objectsToPool = serializedObject.FindProperty ("objectsToPool");
		amountsToPool = serializedObject.FindProperty ("amountsToPool");
		willGrows = serializedObject.FindProperty ("willGrows");
		growAmounts = serializedObject.FindProperty ("growAmounts");

	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();

		for (int i = 0; i < objectsToPool.arraySize; i++)
		{
			GUILayout.BeginVertical ("box");
			GUILayout.Space (5);
			
			objectsToPool.GetArrayElementAtIndex(i).objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Object to pool:", objectsToPool.GetArrayElementAtIndex(i).objectReferenceValue, typeof(GameObject), true);
			amountsToPool.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField ("Amount to pool:", amountsToPool.GetArrayElementAtIndex(i).intValue);
			willGrows.GetArrayElementAtIndex(i).boolValue = GUILayout.Toggle (willGrows.GetArrayElementAtIndex(i).boolValue, "Will grow?");
			growAmounts.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField ("Grow amount:", growAmounts.GetArrayElementAtIndex(i).intValue);

			if (GUILayout.Button ("Remove pool"))
				RemovePool (i);

			GUILayout.Space (5);
			GUILayout.EndVertical ();
		}
			
		GUILayout.Space (20);
		if (GUILayout.Button ("Add pool"))
			AddPool (objectsToPool.arraySize);

		serializedObject.ApplyModifiedProperties ();
//		GUILayout.Space (40);
//		base.DrawDefaultInspector ();
	}

	void AddPool(int index)
	{
		objectsToPool.InsertArrayElementAtIndex (index);
		amountsToPool.InsertArrayElementAtIndex (index);
		willGrows.InsertArrayElementAtIndex (index);
		growAmounts.InsertArrayElementAtIndex (index);
	}

	void RemovePool(int index)
	{
		objectsToPool.DeleteArrayElementAtIndex (index);
		amountsToPool.DeleteArrayElementAtIndex (index);
		willGrows.DeleteArrayElementAtIndex (index);
		growAmounts.DeleteArrayElementAtIndex (index);
	}
}
