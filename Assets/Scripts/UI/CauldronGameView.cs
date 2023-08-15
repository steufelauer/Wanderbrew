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
    [SerializeField] AspectDetail aspectDetail;
    [SerializeField] Transform aspectRoot;
    private List<AspectDetail> aspectDetails = new();
    
    [Header("UIView")]
    [SerializeField] Slider slider;

    public CauldronController Controller { get => controller; set => controller = value; }
    private CauldronController controller;

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
    public void UpdateIngredientAspectCount(List<IngredientAspect> aspects){
        for (int i = 0; i < aspects.Count; i++)
        {
            var found = aspectDetails.Where(x => x.DisplayedAspect == aspects[i].Aspect).FirstOrDefault();
            if(found == null){
                aspectDetails.Add(Instantiate(aspectDetail,aspectRoot));                
            }
            
            aspectDetails[i].SetUp(aspects[i].Aspect, aspects[i].Points);
            aspectDetails[i].gameObject.SetActive(true);
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
        for (int i = aspectDetails.Count-1; i >= 0; i--)
        {      
            Destroy(aspectDetails[i]);
        }
        aspectDetails.Clear();
    }
}
