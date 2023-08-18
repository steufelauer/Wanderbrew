using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class MiniGameView : MonoBehaviour
{
    //TODO take into account that the start line point of the user can be on bottom as well, not only top
    [SerializeField] protected TMP_Text rankText;
    [SerializeField] protected CanvasGroup rankView;
    [SerializeField] protected Canvas rankViewCanvas;
    
    public event Action EndMinigame = delegate { };

    private Canvas canvas;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {

        canvas = this.gameObject.GetComponent<Canvas>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        rankViewCanvas = rankView.GetComponent<Canvas>();
    }
    protected virtual void Start()
    {
        EnableCanvasGroup(false);
    }

    public void EndBtn() => EndMinigame();

    public void EnableCanvasGroup(bool enable)
    {
        if (enable)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvas.enabled = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvas.enabled = false;
        }
    }

    public virtual void Reset()
    {        
        rankView.alpha = 0;
        rankViewCanvas.enabled = false;
        rankView.interactable = false;
    }

    public virtual void DisplayFinalRank(string rank)
    {
        rankText.text = "Rank: " + rank;
        rankView.alpha = 1;
        rankView.interactable = true;
        rankViewCanvas.enabled = true;
    }

}