using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LightMixGameView : MiniGameView
{
    [SerializeField] private Image goalColorImg;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private CanvasGroup rankView;

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
    [SerializeField] private Image DbgGoalPlacement;

    private Color ingredientColor;    
    private Color goalColor;    

    private bool useDBG = true;

    public event Action OnFinishConfirmed = delegate {} ;


    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Start()
    {
        base.Start();
    }
    public void SetUpView(Color ingredientColor, Color goalColor){

        this.ingredientColor = ingredientColor;
        this.goalColor = goalColor;
        goalColorImg.color = goalColor;

        //Todo move to Minigameview
        rankView.alpha = 0;
        rankView.interactable = false;

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

    public void SetUpDebug(Vector2 calc, Vector2 goal){
        DbgNewValPlacement.rectTransform.localPosition = new Vector3(calc.x*100, calc.y*100, 0);
        DbgGoalPlacement.rectTransform.localPosition = new Vector3(goal.x*100, goal.y*100, 0);

        dbgColorValueCurrent.text = calc.y + "";
    }


    //DBG
    public void Remix(){
        //DBG SetUp(ingredientColor);
    }

    public override void Reset(){
        base.Reset();
    }

    public void DisplayFinishConfirmation(bool enable){
        finishedConfirmation.alpha = enable? 1:0;
        finishedConfirmation.interactable = enable;
    }

    public void DisplayRank(string rank){
        rankText.text = "Rank: " +rank;
        rankView.alpha = 1;
        rankView.interactable = true;
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