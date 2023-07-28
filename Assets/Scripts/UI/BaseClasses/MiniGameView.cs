using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameView : MonoBehaviour
{
    //TODO take into account that the start line point of the user can be on bottom as well, not only top
    public event Action EndMinigame = delegate { };

    private Canvas canvas;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {

        canvas = this.gameObject.GetComponentInParent<Canvas>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }
    protected virtual void Start()
    {
    }

    public void EndBtn() => EndMinigame();

    public void EnableCanvasGroup(bool enable)
    {
        if (enable)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }
    }

    public virtual void Reset()
    {
    }

}