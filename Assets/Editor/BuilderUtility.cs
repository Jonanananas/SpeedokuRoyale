using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;

public class BuilderUtility {
    static string[] SCENES = FindEnabledEditorScenes();

    static string APP_NAME = "SpeedokuRoyale";
    static string TARGET_DIR = "Builds";

    [MenuItem("Custom/CI/Build WebGL")]
    public static void BuildWebGL() {
        string targetDir = APP_NAME + "WebGL";
        GenericBuild(SCENES, TARGET_DIR + "/" + targetDir, BuildTargetGroup.WebGL, BuildTarget.WebGL, BuildOptions.None);
    }

    private static string[] FindEnabledEditorScenes() {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string dir, BuildTargetGroup targetGroup, BuildTarget target, BuildOptions options) {
        // EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
        BuildPipeline.BuildPlayer(scenes, dir, target, options);
        // string res = BuildPipeline.BuildPlayer(scenes,target_dir,build_target,build_options);
        // if (res.Length > 0) {
        //         throw new Exception("BuildPlayer failure: " + res);
        // }
    }
}