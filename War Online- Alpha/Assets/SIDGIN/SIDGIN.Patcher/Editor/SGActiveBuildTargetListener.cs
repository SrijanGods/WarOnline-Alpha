using UnityEditor;
using UnityEditor.Build;
namespace SIDGIN.Patcher.Unity.Editors
{
    public class SGActiveBuildTargetListener : IActiveBuildTargetChanged
    {
        public int callbackOrder { get { return 0; } }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
#if UNITY_STANDALONE_WIN
            PlatformDependentConsts.PlatformKey = "UNITY_EDITOR_WIN";
#elif UNITY_STANDALONE_OSX
            PlatformDependentConsts.PlatformKey = "UNITY_EDITOR_OSX";
#elif UNITY_STANDALONE_LINUX
            PlatformDependentConsts.PlatformKey = "UNITY_EDITOR_LINUX";
#endif
        }
    }
}