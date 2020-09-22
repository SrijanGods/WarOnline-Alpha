using UnityEngine;
using UnityEngine.SceneManagement;
using SIDGIN.Patcher.Standalone;
using System;
using System.Linq;

public class CheckStartFromLauncher : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject messageObject;
#pragma warning restore 0649
    void Start()
    {
        if (ApplicationLocking.Check())
        {
            messageObject.SetActive(false);
            SceneManager.LoadScene(1);
        }
        else
        {
            messageObject.SetActive(true);
        }
    }
    public void CloseApplication()
    {
        Application.Quit();
    }
}
