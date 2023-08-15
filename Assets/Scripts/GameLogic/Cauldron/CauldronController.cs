using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronController : MiniGameController
{
    // Start is called before the first frame update
    [SerializeField] private Transform cancelIngredientDrop;

    [SerializeField] private GameObject cauldronWaterDisk;
    [SerializeField] private Transform ingredientRoot;
    [SerializeField] private Transform cauldronWaterDiskRoot;
    [SerializeField] private TriggerArea triggerArea;
    [SerializeField] private int maxIngredients = 5;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private ClickingPattern clickingPattern;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float clickCount = 0f;



    public event Action<float> OnUpdateTimer = delegate {} ;

    
    private float sliderSpeed = 0f;


    private List<Ingredient> ingredients = new();
    private CauldronGameView cauldronGameView;
    private float currentPosAnimationCurve = 0f;


    private bool interactionEnabled => miniGameStarted;

    protected override void Awake(){
        base.Awake();        
        cauldronGameView = gameView as CauldronGameView;
    }
    protected override void Start()
    {
        base.Start();
        triggerArea.AreaTriggerEntered += ConsumeIngredient;

        cauldronGameView.OnSliderChanged += SliderValueChanged;


        cauldronGameView.UpdateIngredientCount(ingredients.Count, maxIngredients);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        triggerArea.AreaTriggerEntered -= ConsumeIngredient;
        cauldronGameView.OnSliderChanged -= SliderValueChanged;
    }
    // Update is called once per frame


    void FixedUpdate()
    {
        // float mult = animationCurve.Evaluate(currentPosAnimationCurve * 0.1f);
        // currentPosAnimationCurve += Time.deltaTime;
        // currentPosAnimationCurve = currentPosAnimationCurve > 1f ? 0f: currentPosAnimationCurve;
        // cauldronWaterDisk.transform.Rotate(Vector3.up, Time.deltaTime * 10f * mult);
        cauldronWaterDisk.transform.Rotate(Vector3.up, Time.deltaTime * speed * sliderSpeed);
    }

    void Update()
    {
        if (!miniGameStarted) return;
        if (Input.GetMouseButtonUp(0))
        {
            clickCount++;
            Ray forwardRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 8;
            RaycastHit hit;
            if (!Physics.Raycast(forwardRay, out hit, 100, layerMask))
            {
                return;
            }
            ClickingPatternPoint point = hit.collider.gameObject.GetComponent<ClickingPatternPoint>();
            if (point != null)
            {
                point.Click();
            }
        }
    }

    public void SliderValueChanged(float speed){
        sliderSpeed = speed;
    }
    protected override void EndMiniGame()
    {
        base.EndMiniGame();
        clickingPattern.gameObject.SetActive(false);
    }

    protected override void SetUpGame()
    {
        Debug.Log("SetUpGame");        
        clickingPattern.gameObject.SetActive(true);
    }

    protected override int CalculateRank()
    {
        Debug.Log("CalculateRank");
        return 0;
    }

    protected override void Reset(){
        base.Reset();
        clickCount = 0;
        clickingPattern.gameObject.SetActive(false);

        // if (!miniGameStarted)
        // {
        //     for (int i = ingredients.Count - 1; i >= 0; i--)
        //     {
        //         Destroy(ingredients[i]);
        //     }

        //     ingredients.Clear();
        //     CalculateAspectPoints();
        // }
    }


    private void ConsumeIngredient(Collider other)
    {
        if (miniGameStarted) return;

        var ingredient = other.gameObject.GetComponent<Ingredient>();
        if (ingredient == null) return;

        if (ingredient.IsPrepared && ingredients.Count < maxIngredients && !miniGameStarted)
        {
            ingredients.Add(ingredient);
            ingredient.gameObject.transform.position = startPlacement.position;
            ingredient.MyRigidBody.isKinematic = true;

            cauldronGameView.UpdateIngredientCount(ingredients.Count, maxIngredients);
            CalculateAspectPoints();

            if (ingredients.Count >= maxIngredients)
            {
                StartMiniGame();
            }
        }
        else
        {
            Debug.Log($"Not prepared!");

            ingredient.gameObject.transform.position = cancelIngredientDrop.position;
        }
    }

    private void CalculateAspectPoints(){
        List<IngredientAspect> ingredientAspects = new();

        for (int i = 0; i < ingredients.Count; i++)
        {
            var aspect = ingredientAspects.Where(x => x.Aspect == ingredients[i].ReachedAspectDetail.Aspect).FirstOrDefault();
            if(aspect != null){
                aspect.Points += ingredients[i].ReachedAspectDetail.Points;
            }else{
                ingredientAspects.Add(new IngredientAspect { Aspect = ingredients[i].ReachedAspectDetail.Aspect, Points = ingredients[i].ReachedAspectDetail.Points});
            }
        }
        cauldronGameView.UpdateIngredientAspectCount(ingredientAspects);
    }
}
