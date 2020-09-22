#if ROCKTOMATE
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Steps;
using SIDGIN.Patcher.Unity.Editors;
using System.Collections;
namespace SIDGIN.Patcher.Integration.RockTomate
{
    [StepDescription("Build and Patch", "Build and Patch for SG Patcher Update System", "SG Patcher")]
    public class SGPatcherBuildAndPatchStep : Step
    {
        [InputField]
        public string definitionName;

        bool inProgress;
        string status;
        string m_status;

        protected override IEnumerator OnExecute(JobContext context)
        {
            StartPatch();
            while (inProgress)
            {
                if (m_status != status)
                {
                    m_status = status;
                    RockLog.WriteLine(LogTier.Info, status);
                }
                yield return null;
            }
        }

        public async void StartPatch()
        {
            inProgress = true;
            var sgPatcherUnityApi = new SGPatcherUnityApi();
            sgPatcherUnityApi.onProgressChanged += OnProgressChanged;
            await sgPatcherUnityApi.BuildAndPatch(definitionName);
            IsSuccess = true;
            inProgress = false;
        }
        void OnProgressChanged(float progress, string status)
        {
            this.status = status;
        }
    }
}
#endif