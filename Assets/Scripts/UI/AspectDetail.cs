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
    [SerializeField] private TMP_Text aspectCount;

    internal void SetUp(AspectAlignment aspect, float points)
    {
        //TODO image <-> aspect
        aspectCount.text = points.ToString();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
