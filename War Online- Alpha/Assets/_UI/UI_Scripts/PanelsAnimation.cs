using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsAnimation : MonoBehaviour
{
    [SerializeField] bool listContent = false;
    [SerializeField] bool listSelector = false;
    [SerializeField] PanelsAnimation[] panelsAnimations;
    public bool startScreen = false;
    bool onScreen = false;

    private void Start()
    {
        if (startScreen) { DisplayTrue(); };
        if (listSelector)
        {
            panelsAnimations = FindObjectsOfType<PanelsAnimation>();
        }
    }

    public void DisplayTrue()
    {
        GetComponent<Animator>().SetBool("display", true);

    }
    public void DisplayFalse()
    {
        GetComponent<Animator>().SetBool("display", false);
    }

    public bool IsListContent()
    {
        return listContent;
    }

    public bool IsOnScreen()
    {
        return onScreen;
    }

    public void OnScreenTrue()
    {
        onScreen = true;
    }

    public void OnScreenFalse()
    {
        onScreen = false;
    }

    public void ListSelection()
    {
        foreach (PanelsAnimation pA in panelsAnimations)
        {
            if (pA.IsListContent())
            {
                for (int i = 0; i < 1; i++)
                {
                    if (pA.IsOnScreen())
                    {
                        pA.DisplayFalse();
                    }
                }

            }

        }
    }
    
    public void BackButton()
    {
        foreach(PanelsAnimation pA in panelsAnimations)
        {
                if (pA.IsOnScreen())
                {
                    pA.DisplayFalse();
                }
        }
    }

}
