using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AspectDetail : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image aspectImg;
    [SerializeField] private Image aspectImgBG;

    public AspectAlignment DisplayedAspect { get => displayedAspect; set => displayedAspect = value; }
    private AspectAlignment displayedAspect;


    internal void SetUp(AspectAlignment aspect, float points)
    {
        Debug.Log("Setup with aspect " + aspect);
        displayedAspect = aspect;
        aspectImg.sprite = SpriteManager.GetSpriteByAspect(aspect);
        aspectImgBG.sprite = SpriteManager.GetSpriteByAspect(aspect);

        aspectImg.fillAmount = points/10f;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
