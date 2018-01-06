using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneSwitcherInspector : EditorWindow {

    public string newScene;
    public int index;

    private static List<SceneAsset> availableScenes = new List<SceneAsset>();

    private Vector2 scrollPos;
    private GUIStyle style;

    [MenuItem("Window/SSI %&s")]
    private static void Init()
    {
        availableScenes.Clear();
        EditorWindow window = GetWindowWithRect(typeof(SceneSwitcherInspector), new Rect(0, 0, 160, 300));

        var scenePaths = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);

        //finds path of all scenes and adds to list of sceneassets
        foreach (var path in scenePaths)
        {
            var unityPath = FileUtil.GetProjectRelativePath(path);
            var unityScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(unityPath);
            availableScenes.Add(unityScene);
        }

        window.Show();
    }

    void Update()
    {
        if(!EditorApplication.isPlaying)
        {
            this.Repaint();
        }
    }
    
    void OnEnable()
    {
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.onHover.textColor = Color.blue;
        style.normal.textColor = Color.blue;
        style.alignment = TextAnchor.LowerCenter; 
        style = new GUIStyle(style);
    }

    void OnGUI()
    {
        this.Repaint();

        //create label showing current loaded scene
        EditorGUILayout.LabelField("Current Scene: " + EditorSceneManager.GetActiveScene().name);
        //create label showing total scene count
        EditorGUILayout.LabelField("Total Scenes: " + availableScenes.Count);

        EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(200), GUILayout.Height(300));


        //add button for each scene that exists and attach openscene for each button
        for (int i = 0; i < availableScenes.Count; i++)
        { 
            var unityScene = availableScenes[i];

            //create button for each scene that exists in assets
            if (GUILayout.Button(unityScene.name, GUILayout.Width(150)))
            {
                var pathToUnityScene = AssetDatabase.GetAssetOrScenePath(unityScene);

                //save current scene before loading
                SaveScene();

                //open new scene
                EditorSceneManager.OpenScene(pathToUnityScene, OpenSceneMode.Single);
            }

            
        }

        //if (GUILayout.Button("Create New Scene", GUILayout.Width(150)))
        //{
        //    SaveScene();

        //    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        //}


        GUILayout.Label("Made by ");

        if (GUILayout.Button("Krassenstein", style))
        {
            Application.OpenURL("http://www.twitch.tv/krassenstein");
        }

        EditorGUILayout.EndScrollView();

        this.Repaint();
    }

    void SaveScene()
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            Debug.Log("Scene Saved");
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveOpenScenes();
        }

    }
}
