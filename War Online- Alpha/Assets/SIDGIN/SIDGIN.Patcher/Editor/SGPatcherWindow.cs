
using SIDGIN.Patcher.Unity.Windows;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace SIDGIN.Patcher.Unity.Editors
{
    public static class SGPatcherWindow
    {
        [MenuItem("Tools/SG Patcher")]
        public static void ShowWindow()
        {
#if UNITY_STANDALONE_WIN
            PlatformDependentConsts.PlatformKey = "UNITY_EDITOR_WIN";
#elif UNITY_STANDALONE_OSX
            PlatformDependentConsts.PlatformKey = "UNITY_EDITOR_OSX";
#elif UNITY_STANDALONE_LINUX
            PlatformDependentConsts.PlatformKey = "UNITY_EDITOR_LINUX";
#endif
            MainWindow.ShowWindow();
        }
    }
}