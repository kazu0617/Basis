// ---------------------------------------------------------------------------
// AURBuildScript.cs
// Basis Social VR Framework – AUR ビルド用 Unity Editor スクリプト
//
// このファイルは PKGBUILD の build() ステップで
// Assets/Editor/ に自動的に配置されます。
// プロジェクトのリポジトリには含まれていません。
// ---------------------------------------------------------------------------
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AURBuildScript
{
    // 環境変数 BUILD_OUTPUT_PATH でビルド先を指定できます
    private static string GetBuildOutputPath()
    {
        return Environment.GetEnvironmentVariable("BUILD_OUTPUT_PATH")
            ?? "build/StandaloneLinux64/Basis Unity";
    }

    /// <summary>
    /// Linux x86_64 向けスタンドアローンビルドを実行します。
    /// Unity CLI から -executeMethod AURBuildScript.BuildLinux64 で呼び出されます。
    /// </summary>
    public static void BuildLinux64()
    {
        string outputPath = GetBuildOutputPath();

        // 出力ディレクトリを確保
        string outputDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDir))
            Directory.CreateDirectory(outputDir);

        // Build Settings で有効化されているシーンを収集
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("[AURBuild] Build Settings に有効なシーンが見つかりません。");
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log($"[AURBuild] {scenes.Length} シーンを {outputPath} にビルドします。");
        foreach (string scene in scenes)
            Debug.Log($"[AURBuild]   + {scene}");

        BuildPlayerOptions opts = new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = outputPath,
            target           = BuildTarget.StandaloneLinux64,
            subtarget        = (int)StandaloneSubtarget.Player,
            options          = BuildOptions.None,
        };

        BuildReport  report  = BuildPipeline.BuildPlayer(opts);
        BuildSummary summary = report.summary;

        switch (summary.result)
        {
            case BuildResult.Succeeded:
                Debug.Log($"[AURBuild] ビルド成功 ({summary.totalSize / 1024.0 / 1024.0:F1} MB)");
                EditorApplication.Exit(0);
                break;

            case BuildResult.Failed:
                Debug.LogError("[AURBuild] ビルド失敗");
                foreach (BuildStep step in report.steps)
                    foreach (BuildStepMessage msg in step.messages)
                        if (msg.type >= LogType.Warning)
                            Debug.LogError($"[AURBuild] [{step.name}] {msg.content}");
                EditorApplication.Exit(1);
                break;

            default:
                Debug.LogWarning($"[AURBuild] ビルド結果: {summary.result}");
                EditorApplication.Exit(1);
                break;
        }
    }
}
