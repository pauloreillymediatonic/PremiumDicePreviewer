using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace TriLibCore.Editor
{
    public class BuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if ((
                    report.summary.platform == BuildTarget.StandaloneWindows || report.summary.platform == BuildTarget.StandaloneWindows64 ||
                    report.summary.platform == BuildTarget.StandaloneLinux64 || report.summary.platform == BuildTarget.StandaloneOSX
                ) && PlayerSettings.GetApiCompatibilityLevel(report.summary.platformGroup) != ApiCompatibilityLevel.NET_4_6)
            {
                UnityEngine.Debug.LogWarning("Please change the API Compatibility Level to .NET 4.x under Player Settings. Otherwise, some TriLib features like GLTF loading won't work.");
            }
        }
    }
}
