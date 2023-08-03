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

    internal void SetUp(AspectAlignment aspect, float points)
    {
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
