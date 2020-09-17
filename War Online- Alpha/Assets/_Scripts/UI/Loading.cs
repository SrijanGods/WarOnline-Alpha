using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviourPunCallbacks
{
    public DuloGames.UI.UIProgressBar loadingSlider;
    public Text loading;

// connect to photon in the initial scene while the login is not ready
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(LoadMainScene());
    }

    IEnumerator LoadMainScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress;
            loadingSlider.fillAmount = progress;
            loading.text = progress * 100 + "%";
            yield return null;
        }
    }
}