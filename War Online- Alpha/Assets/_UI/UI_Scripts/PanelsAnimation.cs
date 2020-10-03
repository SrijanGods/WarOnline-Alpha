using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        CanvasGroupDisplayFalse();
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
        foreach (PanelsAnimation pA in panelsAnimations)
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

    public void CurrentHullPanel()
    {
        string hull = GlobalValues.hull;
        foreach (PanelsAnimation pA in panelsAnimations)
        {
            if (pA.name == hull)
            {
                pA.DisplayTrue();
            }
        }
    }

    public void CurrentTurrentPanel()
    {
        string turrent = GlobalValues.turret;
        foreach (PanelsAnimation pA in panelsAnimations)
        {
            if (pA.name == turrent)
            {
                pA.DisplayTrue();
            }
        }
    }
    public void CurrentPaintPanel()
    {
        string paint = GlobalValues.colour;
        foreach (PanelsAnimation pA in panelsAnimations)
        {
            if (pA.name == paint)
            {
                pA.DisplayTrue();
            }
        }
    }
}
