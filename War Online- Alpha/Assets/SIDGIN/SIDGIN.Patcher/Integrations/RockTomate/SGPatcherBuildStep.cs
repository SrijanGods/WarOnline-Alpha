#if ROCKTOMATE
using HardCodeLab.RockTomate.Core.Steps;
using SIDGIN.Patcher.Unity.Editors;
using HardCodeLab.RockTomate.Core.Attributes;
namespace SIDGIN.Patcher.Integration.RockTomate
{
    [StepDescription("Build", "Build for SG Patcher Update System", "SG Patcher")]
    public class SGPatcherBuildStep : SimpleStep
    {
        [InputField]
        public string definitionName;
        protected override bool OnStepStart()
        {
            var sgPatcherUnityApi = new SGPatcherUnityApi();
            sgPatcherUnityApi.Build(definitionName);
            return true;
        }
    }
}
#endif