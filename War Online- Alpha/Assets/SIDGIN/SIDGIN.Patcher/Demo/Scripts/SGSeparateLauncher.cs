using System;
using System.Threading;
using SIDGIN.Patcher.Client;
using SIDGIN.Patcher.Configurator;
using SIDGIN.Patcher.Storages;
using UnityEngine;
using SIDGIN.Patcher.Unity;
using SIDGIN.Patcher.Standalone;
using SIDGIN.Patcher.Common;

public class SGSeparateLauncher : MonoBehaviour
{
    static string CONFIG_PATH
    {
        get
        {
            return System.IO.Path.Combine(Application.dataPath, "patcher.config");
        }
    }
    static string MAIN_CLIENT_FOLDER
    {
        get
        {
            return Application.persistentDataPath;
        }
    }
    static string LOC_FOLDER
    {
        get
        {
            return System.IO.Path.Combine(MAIN_CLIENT_FOLDER,"loc");
        }
    }
    public static SIDGIN.Patcher.Common.Version Version
    {
        get
        {
            var versionLocal = VersionsControl.GetVersionLocal(MAIN_CLIENT_FOLDER);
            if (versionLocal != null)
            {
                return versionLocal.Version.ToVersion();
            }
            return SIDGIN.Patcher.Common.Version.Empty;
        }
    }

    SGPatcherLoaderView loaderView;
    void Start()
    {
        gameObject.AddComponent<SGDispatcher>();
        loaderView = GetComponent<SGPatcherLoaderView>();
        StartUpdate();
    }

    ConfigurationBuilder GetConfigurationBuilder()
    {
        string data;
        var pathToConfig = CONFIG_PATH;
        if (System.IO.File.Exists(pathToConfig))
        {
            data = System.IO.File.ReadAllText(pathToConfig);
        }
        else
        {
            throw new InvalidConfigurationException(209);
        }

        return new ConfigurationBuilder(data);
    }
    void MainProgress(PatcherProgress patcherProgress)
    {
        
        SGDispatcher.Register(() =>
        {
            float progress = patcherProgress.progress;
            loaderView.OnProgressChanged(new PatcherProgress { progress = progress, status = patcherProgress.status, downloadProgress = patcherProgress.downloadProgress });
        });
    }
    async void StartUpdate()
    {

        try
        {
            var configBuilder = GetConfigurationBuilder();
            var clientSettings = configBuilder.GetData<ClientSettings>();
            if (clientSettings == null)
            {
                clientSettings = new ClientSettings();
            }
            clientSettings.targetDirectory = MAIN_CLIENT_FOLDER;
            clientSettings.localizeDirectory = LOC_FOLDER;

            var config = configBuilder.Add(clientSettings).Compile();
            try
            {
                var sgPatcher = new PatcherClient(config);
                sgPatcher.onProgressChanged += MainProgress;

                await sgPatcher.Update(new CancellationToken());
                StartGame(config);
            }
            catch (Exception ex)
            {
                if (clientSettings.offlineMode)
                {
                    if (ex is UnableConnectToServer || ex is UnableLoadResource)
                    {
                        StartGame(config);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }
        catch (Exception ex)
        {
            loaderView.OnError(new ErrorMessage { message = ex.Message, exception = ex });
            return;
        }
    }
    void StartGame(ConfigurationData data)
    {
#if UNITY_STANDALONE_OSX
        var clientSettings = data.GetData<ClientSettings>();
        MacAccessRightsHelper.GrantAccessRights(MAIN_CLIENT_FOLDER, clientSettings.executeFileName);
#elif UNITY_STANDALONE_LINUX
        var clientSettings = data.GetData<ClientSettings>();
        LinuxAccessRightsHelper.GrantAccessRights(MAIN_CLIENT_FOLDER, clientSettings.executeFileName);
#endif

        var applicationLauncher = new ApplicationLauncher(data);
        applicationLauncher.Launch();
        Application.Quit();
    }

}
