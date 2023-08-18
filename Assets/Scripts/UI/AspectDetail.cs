using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AspectDetail : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image aspectImgColor;
    [SerializeField] private Image aspectImgBG;
    [SerializeField] private Image aspectImgBW;

    public AspectAlignment DisplayedAspect { get => displayedAspect; set => displayedAspect = value; }
    private AspectAlignment displayedAspect;

    private float maxReach = 1f;


    internal void SetUp(AspectAlignment aspect, float points)
    {
        Debug.Log("Setup with aspect " + aspect);
        displayedAspect = aspect;
        aspectImgColor.sprite = SpriteManager.GetSpriteByAspect(aspect);
        aspectImgBG.sprite = SpriteManager.GetSpriteByAspect(aspect);
        aspectImgBW.sprite = SpriteManager.GetSpriteByAspect(aspect);

        aspectImgColor.fillAmount = points/10f;
        aspectImgBW.fillAmount = 0f;

        maxReach = points;
    }

    internal void UpdateColoredValueOnly(float value){
        Debug.Log("UpdateColoredPoints: " + value + " / 10f * " + maxReach + " = " + (value/10f*maxReach));
        aspectImgColor.fillAmount = value/10f * maxReach;
    }

    internal void SetUpWithBlackWhiteImg(AspectAlignment aspect, float colPoints, float maxReach){
        displayedAspect = aspect;
        aspectImgColor.sprite = SpriteManager.GetSpriteByAspect(aspect);
        aspectImgBG.sprite = SpriteManager.GetSpriteByAspect(aspect);
        aspectImgBW.sprite = SpriteManager.GetSpriteByAspect(aspect);

        aspectImgColor.fillAmount = colPoints/10f;
        aspectImgBW.fillAmount = maxReach/10f;

        this.maxReach = maxReach;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
