using UnityEngine;
using UnityEditor;

//This file goes in an Editor folder in your project.
public class SavePrefab : EditorWindow {

	//Edit this with where you want prefabs to go to by default.
	private const string saveLocation = "Assets/Prefabs/";

	//Define a hotkey here at the end of the string. %&v = Ctrl+Alt+V -- # = Shift, % = Ctrl, & = Alt.
	[MenuItem("Tools/Create Prefab From Selected %&v")]
	private static void CreatePrefab() {
		GameObject[] objs = Selection.gameObjects;

		foreach (GameObject go in objs) {
			string path = saveLocation + go.name + ".prefab";
			if (AssetDatabase.LoadAssetAtPath(path, typeof(GameObject))) {
				if (EditorUtility.DisplayDialog("Are you sure?",
					"The prefab already exists. Do you want to overwrite it?",
					"Yes",
					"No")) {
					CreateNew(go, path);
				}
			}
			else {
				CreateNew(go, path);
			}
		}
	}

	private static void CreateNew(GameObject obj, string path) {
		Object prefab = PrefabUtility.CreateEmptyPrefab(path);
		PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
	}
	//If you change the above hotkey/name, change it here too.
	[MenuItem("Tools/Create Prefab From Selected %&v", true)]
	private static bool ValidateCreatePrefab() {
		return Selection.activeGameObject != null;
	}
}