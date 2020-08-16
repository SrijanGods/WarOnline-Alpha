using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LevelLoad()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);
    }
}
