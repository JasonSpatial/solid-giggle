using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ProjectBuilderEditor : EditorWindow {
    private static string[] folderNames;
    private TextAsset textFile;
    private string dataPath;
    private Thread thread;

    private string textPath {
        get { return EditorPrefs.GetString("Folder Names Text Path"); }
        set { EditorPrefs.SetString("Folder Names Text Path", value); }
    }

    [MenuItem("Tools/Build Project Hierarchy")]
    private static void Init() {
        ProjectBuilderEditor window = (ProjectBuilderEditor)GetWindow(typeof(ProjectBuilderEditor));
        window.titleContent = new GUIContent("Project Builder");
        window.Show();
    }

    private void OnGUI() {
        if (dataPath != Application.dataPath)
            dataPath = Application.dataPath;
        if (textFile == null && string.IsNullOrEmpty(textPath) == false) {
            textFile = AssetDatabase.LoadAssetAtPath<TextAsset>(textPath);
        }

        GUILayout.Label("You must tab out and back in again for Unity to refresh and show the folders.", EditorStyles.boldLabel);

        textFile = (TextAsset)EditorGUILayout.ObjectField(new GUIContent("Folder Names Text Asset: "), textFile, typeof(TextAsset), false);
        if (textFile != null) {
            var path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(textFile.name)[0]);
            if (path != textPath) {
                textPath = path;
            }
            folderNames = textFile.text.Split('\n');
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Create Folders"), GUILayout.Width(100))) {
            thread = new Thread(new ThreadStart(Folders));
            thread.Start();
        }
        GUILayout.Space(15);
        if (GUILayout.Button(new GUIContent("Reset"), GUILayout.Width(100))) {
            textPath = "";
            textFile = null;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        if (textFile != null && folderNames != null && folderNames.Length > 0) {
            foreach (var value in folderNames) {
                if (value.StartsWith("#"))
                    EditorGUILayout.LabelField(value.Replace('\t', '\u2192'), EditorStyles.boldLabel);
                else
                    EditorGUILayout.LabelField(value.Replace('\t', '\u2192'));
            }
        }
    }

    private void Folders() {
        BuildFolders(dataPath);
    }

    private void BuildFolders(string appPath) {
        for (int i = 0; i < folderNames.Length; i++) {
            if (folderNames[i].StartsWith("#")) {
                continue;
            }
            var path = "";
            if (folderNames[i].Contains('\t')) {
                var tabCount = folderNames[i].Count(f => f == '\t');
                bool match = false;
                int count = 0;
                List<string> newPath = new List<string>();
                newPath.Add(folderNames[i].Replace("\t", string.Empty));
                while (match == false) {
                    int newTabCount = folderNames[i - count].Count(f => f == '\t');
                    if (newTabCount < tabCount) {
                        newPath.Add(folderNames[i - count].Replace("\t", string.Empty));
                        tabCount = newTabCount;
                    }

                    if (newTabCount == 0) {
                        match = true;
                        newPath.Reverse();
                        path = String.Join("/", newPath.ToArray());
                    }
                    count++;
                }
            }
            else {
                path = folderNames[i];
            }
            path = new string(path.Where(c => !char.IsControl(c)).ToArray());
            var fullPath = (appPath + "/" + path).Replace("/", "\\");
            try {
                if (Directory.Exists(fullPath) == false) {
                    Directory.CreateDirectory(fullPath);
                }
            }
            catch (ArgumentException e) {
            }
            //AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        thread.Abort();
    }
}