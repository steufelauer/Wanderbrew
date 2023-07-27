using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LightMixGameView : MiniGameView
{
    [SerializeField] private Image goalColorImg;

    [Header("Debug")]
    [SerializeField] private TMP_Text dbgColorValueGoal;
    [SerializeField] private TMP_Text dbgColorValueCurrent;
    [SerializeField] private Image dbgStartColorImg;
    [SerializeField] private CanvasGroup finishedConfirmation;

    private Color ingredientColor;    
    private Color goalColor;    
    private LightMixerController controller;    
    public LightMixerController Controller { get => controller; set => controller = value; }

    private bool useDBG = true;

    public event Action OnFinishConfirmed = delegate {} ;



    public void SetUpView(Color ingredientColor, Color goalColor){


        goalColorImg.color = goalColor;

        //dbg
        if(useDBG){
            Color.RGBToHSV(goalColor, out float goalColorHue, out float goalColorSat, out float goalColorVal);
            Color.RGBToHSV(ingredientColor, out float currentColorHue, out float currentColorSat, out float currentColorVal);
            dbgColorValueGoal.text = goalColorVal + "";
            dbgColorValueCurrent.text = currentColorVal + "";
            dbgStartColorImg.color = ingredientColor;
        }
    }


    //DBG
    public void Remix(){
        //SetUp(ingredientColor);
    }

    public void DisplayFinishConfirmation(bool enable){
        finishedConfirmation.alpha = enable? 1:0;
        finishedConfirmation.interactable = enable;
    }

    public void FinishConfirmed()
    {
        DisplayFinishConfirmation(false);
        OnFinishConfirmed();
    }
    public void FinishedDeclined()
    {
        DisplayFinishConfirmation(false);
    }



}