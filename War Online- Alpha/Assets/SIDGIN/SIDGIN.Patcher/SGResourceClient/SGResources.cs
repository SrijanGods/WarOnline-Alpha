#if UNITY_EDITOR
public class SGResources : SIDGIN.Patcher.Unity.Editors.InternalSGResourcesEditor
{

}
#else
public class SGResources : SIDGIN.Patcher.Unity.InternalSGResources
{
  
}
#endif
