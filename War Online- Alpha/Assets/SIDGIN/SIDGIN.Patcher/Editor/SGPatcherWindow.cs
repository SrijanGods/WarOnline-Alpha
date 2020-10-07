
using SIDGIN.Patcher.Editors;
using UnityEditor;

namespace SIDGIN.Patcher.Unity.Editors
{
    public static class SGPatcherWindow
    {
        [MenuItem("Tools/SG Patcher")]
        public static void ShowWindow()
        {
            MainWindow.ShowWindow();
        }
    }
}