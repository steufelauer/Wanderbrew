using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CauldronGameView : MiniGameView
{
    // Start is called before the first frame update
    [Header("CauldronWorldView")]
    [SerializeField] TMP_Text ingredientCount;
    [SerializeField] AspectDetail aspectDetailWV;
    [SerializeField] Transform aspectRootWV;
    
    [Header("UIView")]
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text clickedText;
    [SerializeField] AspectDetail aspectDetailGV;
    [SerializeField] Transform aspectRootGV;

    public CauldronController Controller { get => controller; set => controller = value; }
    private CauldronController controller;
    private List<AspectDetail> aspectDetailsWV = new();
    private List<AspectDetail> aspectDetailsGV = new();

    public Action<float> OnSliderChanged = delegate {};
    protected override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateIngredientCount(int current, int max){
        ingredientCount.text = current + " / " + max;
    }
    public void UpdateIngredientAspectCountWorldView(List<IngredientAspect> aspects){
        for (int i = 0; i < aspects.Count; i++)
        {
            var found = aspectDetailsWV.Where(x => x.DisplayedAspect == aspects[i].Aspect).FirstOrDefault();
            if(found == null){
                aspectDetailsWV.Add(Instantiate(aspectDetailWV,aspectRootWV));                
            }
            
            aspectDetailsWV[i].SetUp(aspects[i].Aspect, aspects[i].Points);
            aspectDetailsWV[i].gameObject.SetActive(true);
            //ingredientCount.text = current + " / " + max;
        }
    }

    public void UpdateClickedPoints(int clickedPoints, int maxPoints){
        Debug.Log("UpdateClickedPoitns: " + clickedPoints + " / " +maxPoints);
        float f = clickedPoints * 1f / maxPoints;
        Debug.Log("MyF: " + f);
        clickedText.text = clickedPoints + " / " + maxPoints;
        for (int i = 0; i < aspectDetailsGV.Count; i++)
        {
            aspectDetailsGV[i].UpdateColoredValueOnly(f);
        }
    }

    public void UpdateIngredientAspectCountMGView(List<IngredientAspect> aspects){
        for (int i = 0; i < aspects.Count; i++)
        {
            var found = aspectDetailsGV.Where(x => x.DisplayedAspect == aspects[i].Aspect).FirstOrDefault();
            if(found == null){
                aspectDetailsGV.Add(Instantiate(aspectDetailGV,aspectRootGV));                
            }
            
            aspectDetailsGV[i].SetUpWithBlackWhiteImg(aspects[i].Aspect, 0f, aspects[i].Points);
            aspectDetailsGV[i].gameObject.SetActive(true);
            //ingredientCount.text = current + " / " + max;
        }
    }

    public void SliderChanged(){
        OnSliderChanged(slider.value);
    }

    public override void Reset()
    {
        //TODO POOLING
        base.Reset();
        for (int i = aspectDetailsWV.Count-1; i >= 0; i--)
        {      
            Destroy(aspectDetailsWV[i]);
        }
        aspectDetailsWV.Clear();
    }

}
