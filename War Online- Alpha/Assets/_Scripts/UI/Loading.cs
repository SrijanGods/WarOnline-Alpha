using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviourPunCallbacks
{
    public Slider loadingSlider;
    public Text loading;

// connect to photon in the initial scene while the login is not ready
    private void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    IEnumerator LoadMainScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress;
            loadingSlider.value = progress;
            loading.text = progress * 100 + "%";
            yield return null;
        }
    }
}