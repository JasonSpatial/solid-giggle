using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SelectGameObjectsWithMissingScripts : Editor {

    [MenuItem("Tools/Select GameObjects With Missing Scripts")]
    private static void SelectGameObjects()
    {
        //Get current scene and all top-level GameObjects in scene hierarchy
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        List<Object> objectsWithDeadLinks = new List<Object>();

        foreach (GameObject g in rootObjects)
        {
            //Get all components on the GameObject then loop through them
            Component[] components = g.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                Component currentC = components[i];

                //if the component is null, that means it's a missing script.
                if(currentC == null)
                {
                    //Add the sinner to the naughty-list.
                    objectsWithDeadLinks.Add(g);
                    Selection.activeGameObject = g;
                    Debug.Log(string.Format("{0} has a missing script.", g));
                }
            }
        }

        //if objectsWithDeadLinks has elements in it, set selection in the editor
        if (objectsWithDeadLinks.Count > 0)
            Selection.objects = objectsWithDeadLinks.ToArray();
        else Debug.Log(string.Format("No GameObjects in '{0}' have missing scripts.", currentScene.name));
    }
}