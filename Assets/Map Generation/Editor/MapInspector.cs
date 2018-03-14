using UnityEngine;
using UnityEditor;
using UnityEngine.PostProcessing;

[CustomEditor (typeof(Map))]
public class MapInspector : Editor
{

	SerializedProperty levelName;

	SerializedProperty levelWidth;
	SerializedProperty levelHeight;
	SerializedProperty wallHeight;

	SerializedProperty scale;
	SerializedProperty octaves;
	SerializedProperty lacunarity;
	SerializedProperty persistance;

	SerializedProperty heightMultiplier;

	SerializedProperty wallMaterial;

	SerializedProperty possibleTerrain;

	SerializedProperty objectChance;
	SerializedProperty possibleObjects;

	SerializedProperty wallLimit;

	SerializedProperty tileSize;

	SerializedProperty lightOptions;

	SerializedProperty postProcProfile;

	Vector2 terrainScrollPos;
	Vector2 objectScrollPos;

	void OnEnable ()
	{
		levelWidth = serializedObject.FindProperty ("levelWidth");
		levelHeight = serializedObject.FindProperty ("levelHeight");
		wallHeight = serializedObject.FindProperty ("wallHeight");

		scale = serializedObject.FindProperty ("scale");
		octaves = serializedObject.FindProperty ("octaves");
		lacunarity = serializedObject.FindProperty ("lacunarity");
		persistance = serializedObject.FindProperty ("persistance");

		heightMultiplier = serializedObject.FindProperty ("heightMultiplier");

		wallMaterial = serializedObject.FindProperty ("wallMaterial");

		possibleTerrain = serializedObject.FindProperty ("possibleTerrain");

		objectChance = serializedObject.FindProperty ("objectChance");
		possibleObjects = serializedObject.FindProperty ("possibleObjects");

		wallLimit = serializedObject.FindProperty ("wallLimit");

		tileSize = serializedObject.FindProperty ("tileSize");

		lightOptions = serializedObject.FindProperty ("lightOptions");

		postProcProfile = serializedObject.FindProperty ("postProcProfile");

	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		GUILayout.Label ("Level settings", EditorStyles.boldLabel);

		GUILayout.BeginVertical ("box");

		GUILayout.Label ("Sizing Options");
		levelWidth.intValue = EditorGUILayout.IntSlider ("Level width", levelWidth.intValue, 30, 100);
		levelHeight.intValue = EditorGUILayout.IntSlider ("Level height", levelHeight.intValue, 30, 100);
		wallHeight.floatValue = EditorGUILayout.Slider ("Wall height", wallHeight.floatValue, -5f, 10f);
		tileSize.floatValue = EditorGUILayout.Slider ("Tile size", tileSize.floatValue, 1f, 10f);

		GUILayout.Space (5);

		GUILayout.Label ("Generation Options");
		scale.floatValue = EditorGUILayout.FloatField ("Scale", scale.floatValue);
		octaves.intValue = EditorGUILayout.IntSlider ("Octaves", octaves.intValue, 1, 8);
		lacunarity.floatValue = EditorGUILayout.Slider ("Lacunarity", lacunarity.floatValue, 0f, 4f);
		persistance.floatValue = EditorGUILayout.Slider ("Persistance", persistance.floatValue, 0f, 1f);

		GUILayout.Space (5);

		EditorGUILayout.PropertyField (heightMultiplier);

//		wallLimit.floatValue = EditorGUILayout.Slider ("Wall limit", wallLimit.floatValue, 0.1f, 1f);

		GUILayout.Space (5);
		GUILayout.EndVertical ();
		GUILayout.Space (10);

		GUILayout.Label ("Levels of Terrain", EditorStyles.boldLabel);
		GUILayout.Label ("The lowest most field will be impassible (Red)");
		wallMaterial.objectReferenceValue = (Material)EditorGUILayout.ObjectField ("Wall Material", wallMaterial.objectReferenceValue, typeof(Material), false);
		GUILayout.Space (5);

		if (possibleTerrain.arraySize > 3)
			terrainScrollPos = EditorGUILayout.BeginScrollView (terrainScrollPos, GUILayout.Height (330));
		for (int i = 0; i < possibleTerrain.arraySize; i++) {
			if (i == 0)
				GUI.color = Color.red;

			GUILayout.BeginVertical ("box");
			GUI.color = Color.white;
			GUILayout.Space (5);

			SerializedProperty tileInfoStruct = possibleTerrain.GetArrayElementAtIndex (i);
			SerializedProperty terrainColour = tileInfoStruct.FindPropertyRelative ("terrainColour");
			SerializedProperty height = tileInfoStruct.FindPropertyRelative ("height");
			SerializedProperty material = tileInfoStruct.FindPropertyRelative ("terrainMaterial");

			terrainColour.colorValue = EditorGUILayout.ColorField ("Terrain colour", terrainColour.colorValue);
			material.objectReferenceValue = (Material)EditorGUILayout.ObjectField ("Terrain Material", material.objectReferenceValue, typeof(Material), false);

			float maxHeight = height.floatValue;
			if (i > 0) {
				float minHeight = possibleTerrain.GetArrayElementAtIndex (i - 1).FindPropertyRelative ("height").floatValue;
				EditorGUILayout.MinMaxSlider ("Height", ref minHeight, ref maxHeight, 0f, 1f);
				possibleTerrain.GetArrayElementAtIndex (i - 1).FindPropertyRelative ("height").floatValue = minHeight;
			} else {
				float minHeight = 0f;
				EditorGUILayout.MinMaxSlider ("Height", ref minHeight, ref maxHeight, 0f, 1f);
			}
			height.floatValue = maxHeight;

			GUILayout.Space (10);

			if (GUILayout.Button ("Remove Terrain"))
				RemoveColour (i);

			GUILayout.Space (5);
			GUILayout.EndVertical ();

		}
		if (possibleTerrain.arraySize > 3)
			EditorGUILayout.EndScrollView ();

		GUILayout.Space (5);
		if (GUILayout.Button ("Add Terrain"))
			AddColour (possibleTerrain.arraySize);

		GUILayout.Space (10);
		GUILayout.Label ("Objects for terrain", EditorStyles.boldLabel);
		objectChance.floatValue = EditorGUILayout.Slider ("Object Chance", objectChance.floatValue, 0f, 1f);
		GUILayout.Space (5);
		if (possibleObjects.arraySize > 3)
			objectScrollPos = EditorGUILayout.BeginScrollView (objectScrollPos, GUILayout.Height (330));
		for (int i = 0; i < possibleObjects.arraySize; i++) {
			GUILayout.BeginVertical ("box");
			GUILayout.Space (10);

			SerializedProperty terrInfoStruct = possibleObjects.GetArrayElementAtIndex (i);
			SerializedProperty terrain = terrInfoStruct.FindPropertyRelative ("terrain");
			SerializedProperty chance = terrInfoStruct.FindPropertyRelative ("chance");
			SerializedProperty height = terrInfoStruct.FindPropertyRelative ("height");

			terrain.objectReferenceValue = EditorGUILayout.ObjectField ("Terrain object", terrain.objectReferenceValue, typeof(GameObject), true);
			chance.intValue = EditorGUILayout.IntSlider ("Weight", chance.intValue, 0, 100);
			height.floatValue = EditorGUILayout.Slider ("Height", height.floatValue, 0f, 1f);
				
			GUILayout.Space (15);
				
			if (GUILayout.Button ("Remove object"))
				RemoveObject (i);

			GUILayout.Space (5);
			GUILayout.EndVertical ();
		}
		if (possibleObjects.arraySize > 3)
			EditorGUILayout.EndScrollView ();

		GUILayout.Space (5);
		if (GUILayout.Button ("Add object"))
			AddObject (possibleObjects.arraySize);

		GUILayout.Space (10);

		GUILayout.Label ("Light Options", EditorStyles.boldLabel);
		GUILayout.BeginVertical ("box");

		SerializedProperty angle = lightOptions.FindPropertyRelative ("angle");
		SerializedProperty intensity = lightOptions.FindPropertyRelative ("intensity");
		SerializedProperty colour = lightOptions.FindPropertyRelative ("lightColour");

		angle.vector3Value = EditorGUILayout.Vector3Field ("Light Angle", angle.vector3Value);
		intensity.floatValue = EditorGUILayout.Slider ("Light Intensity", intensity.floatValue, 0f, 1f);
		colour.colorValue = EditorGUILayout.ColorField ("Light Colour", colour.colorValue);

		postProcProfile.objectReferenceValue = EditorGUILayout.ObjectField ("Post Proccessing Profile", postProcProfile.objectReferenceValue, typeof(PostProcessingProfile), false);

		GUILayout.EndVertical ();

		GUILayout.Space (100);

		serializedObject.ApplyModifiedProperties ();

//		base.DrawDefaultInspector ();
	}

	void AddColour (int index)
	{
//		Level.TileInfo newTileInfo = new Level.TileInfo ();
		possibleTerrain.InsertArrayElementAtIndex (index);
	}

	void AddObject (int index)
	{
//		Level.TerrainInfo newTerrInfo = new Level.TerrainInfo ();
		possibleObjects.InsertArrayElementAtIndex (index);
	}

	void RemoveColour (int index)
	{
		possibleTerrain.DeleteArrayElementAtIndex (index);
	}

	void RemoveObject (int index)
	{
		possibleObjects.DeleteArrayElementAtIndex (index);
	}

	//	Texture2D MakePreview (int[,] map)
	//	{
	//		int width = map.GetLength (0);
	//		int height = map.GetLength (1);
	//		Texture2D terrain = gen.GenerateTexture ();
	//		for (int x = 0; x < width; x++) {
	//			for (int y = 0; y < height; y++) {
	//				if (map [x, y] == 1) {
	//					terrain.SetPixel (x, y, Color.black);
	//				}
	//			}
	//		}
	//		terrain.Apply ();
	//		return terrain;
	//	}
}
