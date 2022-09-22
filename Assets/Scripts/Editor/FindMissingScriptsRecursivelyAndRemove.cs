using UnityEditor;
using UnityEngine;
 
namespace FLGCoreEditor.Utilities
{
    public class FindMissingScriptsRecursivelyAndRemove : EditorWindow 
    {
        static int goCount;
        static int componentsCount;
        static int missingCount;
 
        static bool bHaveRun;
 
        [MenuItem("FLGCore/Editor/Utility/FindMissingScriptsRecursivelyAndRemove")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindMissingScriptsRecursivelyAndRemove));
        }
 
        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }
 
            if (!bHaveRun) return;
            
            EditorGUILayout.TextField($"{goCount} GameObjects Selected");
            if(goCount>0) EditorGUILayout.TextField($"{componentsCount} Components");
            if(goCount>0) EditorGUILayout.TextField($"{missingCount} Deleted");
        }
        
        static void FindInSelected()
        {
            var go = Selection.gameObjects;
            goCount = 0;
            componentsCount = 0;
            missingCount = 0;
            foreach (var g in go)
            {
                FindInGo(g);
            }
 
            bHaveRun = true;
            Debug.Log($"Searched {goCount} GameObjects, {componentsCount} components, found {missingCount} missing");
            
            AssetDatabase.SaveAssets();
        }
 
        static void FindInGo(GameObject g)
        {
            goCount++;
            var components = g.GetComponents<Component>();
         
            var r = 0;
            
            for (var i = 0; i < components.Length; i++)
            {
                componentsCount++;
                if (components[i] != null) continue;
                missingCount++;
                var s = g.name;
                var t = g.transform;
                while (t.parent != null) 
                {
                    s = t.parent.name +"/"+s;
                    t = t.parent;
                }
                
                Debug.Log ($"{s} has a missing script at {i}", g);
                
                var serializedObject = new SerializedObject(g);
                
                var prop = serializedObject.FindProperty("m_Component");
                
                prop.DeleteArrayElementAtIndex(i-r);
                r++;
         
                serializedObject.ApplyModifiedProperties();
            }
            
            foreach (Transform childT in g.transform)
            {
                FindInGo(childT.gameObject);
            }
        }
    }
}