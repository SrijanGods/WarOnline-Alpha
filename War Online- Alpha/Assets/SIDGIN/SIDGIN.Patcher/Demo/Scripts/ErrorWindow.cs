using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ErrorWindow : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    Text errorText;
    [SerializeField]
    Button closeButton;
#pragma warning restore 0649
    void Awake()
    {
        closeButton.onClick.AddListener(OnButtonClose);
    }
    void OnButtonClose()
    {
        Application.Quit();
    }
    public void Show(string errorMessage)
    {
        errorText.text = errorMessage;
        gameObject.SetActive(true);
    }

}
