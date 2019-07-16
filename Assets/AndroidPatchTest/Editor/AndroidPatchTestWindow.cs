using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class AndroidPatchTestWindow : EditorWindow
{
    protected int mNumberTestIterations = 10;
    public static readonly GUIContent testIterations = EditorGUIUtility.TrTextContent("Test iterations", "How many times each test should be reperated");
    public static readonly GUIContent fullBuild = EditorGUIUtility.TrTextContent("Full build", "Build the application completely");
    public static readonly GUIContent pachBuild = EditorGUIUtility.TrTextContent("Script patch build", "Build a patch for the code");

    protected delegate double TestFunction();

    [MenuItem("Window/AndroidPatchTest")]
    public static void InitWindow()
    {
        var window = EditorWindow.CreateInstance<AndroidPatchTestWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        mNumberTestIterations = EditorGUILayout.IntField(testIterations, mNumberTestIterations);
        if (GUILayout.Button(fullBuild, EditorStyles.toolbarButton))
        {
            PerformTest(FullBuild);
        }

        if (GUILayout.Button(pachBuild, EditorStyles.toolbarButton))
        {
            PerformTest(PatchBuild);
        }
    }

    protected void PerformTest(TestFunction testFunction)
    {
        List<double> elapsedTimes = new List<double>();
        for (int i = 0; i < mNumberTestIterations; i++)
        {
            double elapsed = testFunction.Invoke();
            elapsedTimes.Add(elapsed);
        }
        PrintStatistics(elapsedTimes);
    }

    protected void PrintStatistics(List<double> elapsedTimes)
    {
        double mean = elapsedTimes.Average();
        double sumSquaredDiff = elapsedTimes.Sum(v => (v - mean) * (v - mean));
        double stdev = System.Math.Sqrt(sumSquaredDiff / (elapsedTimes.Count()-1));
        Debug.LogFormat("Mean {0} , Stdev {1}", mean, stdev);
    }

    protected double FullBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/AndroidPatchTest/Scenes/AndroidPatchTest.unity"};
        buildPlayerOptions.locationPathName = "Builds/FullBuild.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.Development | BuildOptions.AutoRunPlayer;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");

        double startTime = EditorApplication.timeSinceStartup;
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        double elapsedTime = EditorApplication.timeSinceStartup - startTime;
        Debug.Log("elapsed: " + elapsedTime);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
        return elapsedTime;
    }

    protected double PatchBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/AndroidPatchTest/Scenes/AndroidPatchTest.unity" };
        buildPlayerOptions.locationPathName = "Builds/FullBuild.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.Development | BuildOptions.BuildScriptsOnly | BuildOptions.PatchPackage | BuildOptions.AutoRunPlayer;

        if (PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android) == "ROTATE_CUBE")
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
        }
        else
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "ROTATE_CUBE");
        }

        double startTime = EditorApplication.timeSinceStartup;
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        double elapsedTime = EditorApplication.timeSinceStartup - startTime;
        Debug.Log("elapsed: " + elapsedTime);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
        return elapsedTime;
    }
}
