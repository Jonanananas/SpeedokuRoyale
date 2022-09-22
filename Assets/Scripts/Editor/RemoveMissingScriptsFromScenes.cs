using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RemoveMissingScriptsFromScenes : ScriptableWizard {
    public Object[] scenes;

    [MenuItem("Tools/Remove Missing Scripts From Scenes")]
    static void CreateWizard() {
        ScriptableWizard.DisplayWizard("Edit Scenes", typeof(RemoveMissingScriptsFromScenes), "Run");
    }

    void OnWizardCreate() {
        foreach (Object s in scenes) {
            string path = AssetDatabase.GetAssetPath(s);

            Scene openScene = EditorSceneManager.OpenScene(path);

            Object[] sceneObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            foreach (object obj in sceneObjects) {
                GameObject go = (GameObject)obj;
                Debug.Log(go);
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            }
            EditorSceneManager.SaveScene(openScene);
        }
    }
}