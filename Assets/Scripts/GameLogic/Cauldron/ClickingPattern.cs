using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ClickingPattern : MonoBehaviour
{
    [Serializable]
    class Ranking{
        [SerializeField] public int MissclickAmount;
        [SerializeField] public int ResultingRank;
    }
    [SerializeField] private List<ClickingPatternPoint> patternPoints;
    [SerializeField] private List<Ranking> rankSettings;
    [SerializeField] private bool isActive;
    [SerializeField] private AnimationCurve patternRevealAnimationCurve;
    private float currentSliderSpeed = 0.5f;

    public int UnClickedPointCount => patternPoints.Where(x => x.Clicked == false).Count();
    public int ClickedPointCount => patternPoints.Where(x => x.Clicked == true).Count();
    public int AllPointCount => patternPoints.Count();

    public float CurrentSliderSpeed { get => currentSliderSpeed; set => currentSliderSpeed = value; }

    void Awake(){
        rankSettings = rankSettings.OrderBy(x => x.MissclickAmount).ToList();
    }
    void Start()
    {   
        foreach (var point in patternPoints)
        {
            point.ClickingPattern = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive) return;
    }

    public void StartPattern(float sliderSpeed){
        ResetPattern();
        isActive = true;
        currentSliderSpeed = sliderSpeed;
        float random = 0f;
        for (int i = 0; i < patternPoints.Count; i++)
        {
            random = UnityEngine.Random.Range(0f, 1f);
            patternPoints[i].StartAnimation(random, patternRevealAnimationCurve);
        }
    }

    public void ResetPattern(){
         for (int i = 0; i < patternPoints.Count; i++)
        {
           patternPoints[i].Clicked = false;
        }
    }

    public void UpdateSliderSpeed(float sliderSpeed) => currentSliderSpeed = sliderSpeed;

    public int GetRankFromMissclicks(int clickCOunt){
        for (int i = 0; i < rankSettings.Count; i++)
        {
            if(clickCOunt <= rankSettings[i].MissclickAmount){
                return rankSettings[i].ResultingRank;
            }
        }
            return -1;        
    }

}
