namespace SIDGIN.Patcher.Unity.SceneManagment
{
    public class SGSceneManager :
#if UNITY_EDITOR
        SIDGIN.Patcher.Unity.Editors.SGSceneManagerEditor
#else
        SIDGIN.Patcher.Unity.SceneManagment.InternalSGSceneManager
#endif
    {
    }
}
