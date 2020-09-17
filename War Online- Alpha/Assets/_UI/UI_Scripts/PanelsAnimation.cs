using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsAnimation : MonoBehaviour
{
    [SerializeField] bool listContent = false;
    [SerializeField] bool scrollbar = false;
    [SerializeField] PanelsAnimation[] panelsAnimations;
    public bool startScreen = false;
    public bool onScreen = false;
    CanvasGroup cG;

    private void Start()
    {
        cG = GetComponent<CanvasGroup>();
        if (startScreen) { DisplayTrue(); };
        
    }

    public void DisplayTrue()
    {
        CanvasGroupDisplayTrue();
        GetComponent<Animator>().SetBool("display", true);


    }

    private void CanvasGroupDisplayTrue()
    {
        cG.alpha = 1;
        cG.blocksRaycasts = true;
        cG.interactable = true;

    }

    private void CanvasGroupDisplayFalse()
    {
        cG.alpha = 0;
        cG.blocksRaycasts = false;
        cG.interactable = false;
        OnScreenFalse();

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
                    pA.DisplayFalse();
        }
    }

    public void DisplayTrue2()
    {
        GetComponent<Animator>().SetBool("display2", true);
    } 
    
    public void DisplayFalse2()
    {
        GetComponent<Animator>().SetBool("display2", false);
    }

    public void ScrollBarSelection()
    {
        foreach (PanelsAnimation pA in panelsAnimations)
        {
            if (scrollbar && IsOnScreen())
            {
                pA.DisplayFalse();
            }
        }
    }

}
