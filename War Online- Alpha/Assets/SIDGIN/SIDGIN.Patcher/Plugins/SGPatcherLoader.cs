using SIDGIN.Patcher.Client;
using SIDGIN.Patcher.Common;
using SIDGIN.Patcher.Configurator;
using SIDGIN.Patcher.Storages;
using SIDGIN.Patcher.Unity.SceneManagment;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
namespace SIDGIN.Patcher.Unity
{
    public class ErrorMessage
    {
        public string message;
        public System.Exception exception;
    }
    [System.Serializable]
    public class SGPatcherProgressEvent : UnityEvent<PatcherProgress> { }
    [System.Serializable]
    public class SGPatcherErrorEvent : UnityEvent<ErrorMessage> { }
    public class SGPatcherLoader : MonoBehaviour
    {

        public SGPatcherProgressEvent onProgressChanged;
        public SGPatcherErrorEvent onError;
        public SGPatcherErrorEvent onInternalError;
        public bool startUpdateOnAwake = true;
        public string languageKey;

        CancellationTokenSource tokenSource;
        void Start()
        {
            PrepareLocalizationSheets();
            if (startUpdateOnAwake)
            {
                UpdateGame();
            }
        }
        void PrepareLocalizationSheets()
        {
            var localizationFolder = Path.Combine(Application.persistentDataPath, "loc");
            var languageResources = Resources.LoadAll<TextAsset>("loc/");
            if (languageResources != null && languageResources.Any())
            {
                if (!Directory.Exists(localizationFolder))
                    Directory.CreateDirectory(localizationFolder);
                foreach (var languageData in languageResources)
                {
                    var languageFile = Path.Combine(localizationFolder, $"{languageData.name}.xml");
                    File.WriteAllText(languageFile, languageData.text);
                }
            }
        }
        private static string GetSystemCulture()
        {
            var systemLanguage = Application.systemLanguage;
            var cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures).
                FirstOrDefault(x => x.EnglishName == Enum.GetName(systemLanguage.GetType(), systemLanguage));
            if (cultureInfo != null)
            {
                return cultureInfo.EnglishName;
            }
            return "";
        }
        PatcherClient PrepareClient()
        {
            var targetPath = Application.persistentDataPath;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            var patcherClinetSettings = Resources.Load<PatcherClientSettings>("PatcherClientSettings");
            if (patcherClinetSettings == null)
                throw new System.ApplicationException("PatcherClientSettings asset not found!");

            InternalErrorHandler.onErrorHandled += InternalErrorHandler_onErrorHandled;
            var configBuilder = new ConfigurationBuilder();
            var clientSettings = new ClientSettings();
            clientSettings.appId = patcherClinetSettings.appId;
            clientSettings.targetDirectory = targetPath;
            clientSettings.integrityCheck = patcherClinetSettings.integrityCheck;
            if (patcherClinetSettings.syncMainBuild)
            {
                clientSettings.mainBuildVersion = Application.version;
            }
            var localizationFolder = Path.Combine(Application.persistentDataPath, "loc");
            if (System.IO.Directory.Exists(localizationFolder))
            {
                clientSettings.localizeDirectory = localizationFolder;
                if (!string.IsNullOrEmpty(languageKey))
                {
                    clientSettings.languageKey = languageKey;
                }
                else
                {
                    clientSettings.languageKey = GetSystemCulture();
                }
            }
            var config = configBuilder.Add(clientSettings).Add(new StorageSettings
            {
                storageName = patcherClinetSettings.storageName
            }).Compile();

            var versionControl = new VersionsControl(config);
            var client = new PatcherClient(config);
            client.onProgressChanged += OnProgressChanged;
            return client;
        }

        IEnumerator LoadGameAsync()
        {
#if !UNITY_EDITOR
            try
            {
            SGResources.Load();
            }
            catch (System.Exception ex)
            {
                onError.Invoke(new ErrorMessage
                        {
                            exception = ex,
                            message = ex + "\n" + ex.InnerException?.Message + "\n" + ex.StackTrace
                        });
                throw ex;
            }
#endif
            var asyncOperation = SGSceneManager.LoadSceneAsync(0);
            while (!asyncOperation.isDone)
            {
                onProgressChanged.Invoke(new PatcherProgress { progress = asyncOperation.progress, status = "Loading..." });
                if (asyncOperation.progress > 0.9f)
                {
                    asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
        public void LoadGame()
        {
            if (SGPatcher.Version == Common.Version.Empty)
            {
                UpdateGame();
            }
            else
            {
                StartCoroutine(LoadGameAsync());
            }
        }
        public async Task<UpdateMetaData> GetUpdateMetaData()
        {
            try
            {
                var patcherClient = PrepareClient();
                var metaData = patcherClient.GetUpdateMetaData();
                return await metaData;
            }
            catch (System.Exception ex)
            {
                var patcherClinetSettings = Resources.Load<PatcherClientSettings>("PatcherClientSettings");
                if (patcherClinetSettings == null)
                    throw new System.ApplicationException("PatcherClientSettings asset not found!");
                if (patcherClinetSettings.offlineMode)
                {
                    if (ex is UnableConnectToServer || ex is UnableLoadResource)
                    {
                        var versionData = VersionsControl.GetVersionLocal(Consts.TARGET_PATH);
                        if (versionData != null)
                        {
                            LoadGame();
                        }
                        InternalErrorHandler_onErrorHandled(ex);
                    }
                    else
                    {
                        onError.Invoke(new ErrorMessage
                        {
                            exception = ex,
                            message = ex + "\n" + ex.InnerException?.Message + "\n" + ex.StackTrace
                        });
                      
                    }
                }
                else
                {
                    onError.Invoke(new ErrorMessage
                    {
                        exception = ex,
                        message = ex + "\n" + ex.InnerException?.Message + "\n" + ex.StackTrace
                    });
                }
                return null;
            } 
        }
        public async void UpdateGame()
        {
            var patcherClinetSettings = Resources.Load<PatcherClientSettings>("PatcherClientSettings");
            if (patcherClinetSettings == null)
                throw new System.ApplicationException("PatcherClientSettings asset not found!");

            try
            {
                var client = PrepareClient();
                tokenSource = new CancellationTokenSource();
                await client.Update(tokenSource.Token);
                StartCoroutine(LoadGameAsync());

            }
            catch (Exception ex)
            {
                if (patcherClinetSettings.offlineMode)
                {
                    if (ex is UnableConnectToServer || ex is UnableLoadResource)
                    {
                        var versionData = VersionsControl.GetVersionLocal(Consts.TARGET_PATH);
                        if (versionData != null)
                        {
                            StartCoroutine(LoadGameAsync());
                        }
                        InternalErrorHandler_onErrorHandled(ex);
                    }
                    else
                    {
                        onError.Invoke(new ErrorMessage
                        {
                            exception = ex,
                            message = ex + "\n" + ex.InnerException?.Message + "\n" + ex.StackTrace
                        });
                    }
                }
                else
                {
                    onError.Invoke(new ErrorMessage
                    {
                        exception = ex,
                        message = ex + "\n" + ex.InnerException?.Message + "\n" + ex.StackTrace
                    });
                }
            }
            finally
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
        }
        private void OnApplicationQuit()
        {
            if (tokenSource != null)
                tokenSource.Cancel();
        }
        private void InternalErrorHandler_onErrorHandled(Exception ex)
        {
            SGDispatcher.Register(() =>
            {
                onInternalError.Invoke(new ErrorMessage
                {
                    exception = ex,
                    message = ex + "\n" + ex.InnerException?.Message + "\n" + ex.StackTrace
                });
            });
        }

        private void OnProgressChanged(PatcherProgress patcherProgress)
        {
            SGDispatcher.Register(() =>
            {
                var progress = patcherProgress;
                onProgressChanged.Invoke(progress);
            });
        }


    }
}
