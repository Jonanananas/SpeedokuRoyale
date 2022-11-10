using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;

class Builder {
    static string[] SCENES = FindEnabledEditorScenes();

    static string APP_NAME = "SpeedokuRoyale";
    static string TARGET_DIR = "Builds";

    [MenuItem("Custom/CI/Build Mac OS X")]
    static void PerformMacOSXBuild() {
        string target_dir = APP_NAME + ".app";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneOSXIntel, BuildOptions.None);
    }
    static void PerformWindowsBuild() {
        string target_dir = APP_NAME + ".app";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    private static string[] FindEnabledEditorScenes() {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options) {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        // string res = BuildPipeline.BuildPlayer(scenes,target_dir,build_target,build_options);
        // if (res.Length > 0) {
        //         throw new Exception("BuildPlayer failure: " + res);
        // }
    }
}