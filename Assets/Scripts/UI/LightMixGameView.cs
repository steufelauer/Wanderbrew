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
    
    [SerializeField] private Image DbgLineTop;
    [SerializeField] private Image DbgLineBot;
    [SerializeField] private Image DbgLineDistance;
    [SerializeField] private Image DbgPointPlacement;
    [SerializeField] private Image DbgNewValPlacement;

    private Color ingredientColor;    
    private Color goalColor;    
    private LightMixerController controller;    
    public LightMixerController Controller { get => controller; set => controller = value; }

    private bool useDBG = true;

    public event Action OnFinishConfirmed = delegate {} ;



    public void SetUpView(Color ingredientColor, Color goalColor){

        this.ingredientColor = ingredientColor;
        this.goalColor = goalColor;
        goalColorImg.color = goalColor;

        //dbg
        if(useDBG){
            Color.RGBToHSV(goalColor, out float goalColorHue, out float goalColorSat, out float goalColorVal);
            Color.RGBToHSV(ingredientColor, out float currentColorHue, out float currentColorSat, out float currentColorVal);
            dbgColorValueGoal.text = goalColorVal + "";
            dbgColorValueCurrent.text = currentColorVal + "";
            dbgStartColorImg.color = ingredientColor;

            DisplayDebugLines();
        }
    }

    public void SetUpDebug(Vector2 perc, Vector2 calc){
        Debug.Log("SetupDebug perc:" + perc +" calc: " +calc);
        DbgPointPlacement.rectTransform.localPosition = new Vector3(perc.x*100, perc.y*100, 0);
        DbgNewValPlacement.rectTransform.localPosition = new Vector3(calc.x*100, calc.y*100, 0);

        dbgColorValueCurrent.text = calc.y + "";
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

    private void DisplayDebugLines(){
        if(useDBG){
            Color.RGBToHSV(ingredientColor, out float currentColorHue, out float currentColorSat, out float currentColorVal);
            
            DrawLine(DbgLineBot, new Vector3(10,90,0), new Vector3(currentColorSat*100f,currentColorVal*100f,0));
            DrawLine(DbgLineTop, new Vector3(currentColorSat*100f,currentColorVal*100f,0), new Vector3(90,10,0));
            DrawLine(DbgLineDistance, new Vector3(10,90,0), new Vector3(90,10,0));
        }
    }

    public void DrawLine(Image img, Vector3 top, Vector3 bot)
    {
        RectTransform rect = img.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;

        Vector3 a = new Vector3(top.x, top.y, 0);
        Vector3 b = new Vector3(bot.x, bot.y, 0);

        //Debug.Log($"A={a}, B={b}");
        rect.localPosition = a.x <= b.x ? a : b;
        Vector3 dif = a - b;

        if(dif.x == 0){
            Debug.LogError($"[CutGameView::DrawLine] Division 0 prevented from dif.x calculation, changing point");
            dif = new Vector3(dif.x + 0.1f, dif.y, dif.z);
        }

        rect.sizeDelta = new Vector3(dif.magnitude, 5f);
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));

    }



}