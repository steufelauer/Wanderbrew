
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMixerController : MonoBehaviour
{
    enum MixingState { None, Light, Darkness };
    [SerializeField] private LightMixGameView lightMixGameView;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera miniGameCamera;
    [SerializeField] private Transform startPlacement;
    [SerializeField] private Transform cancelledPlacement;
    [SerializeField] private GameObject backgroundPlane;
    [SerializeField] private GameObject movementPlane;
    [SerializeField] private MixingFinishedTrigger finishedTrigger;
    [SerializeField] private MixingCollider lightCollider;
    [SerializeField] private MixingCollider darknessCollider;
    [SerializeField] private MeshRenderer dbgCube;
    [SerializeField] private float perfectScoreMargin = 0.05f;
    [SerializeField] private float rankMargin = 0.1f;
    [SerializeField] private int maxRanks = 3;

    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uIProviderService;

    IPickable currentPickable;

    GameObject currentMiniGameGO;
    private Plane interactionPlane;
    private bool miniGameStarted = false;
    private bool waitingForInput = false;
    private bool receivedRank = true;

    private bool interactionEnabled => miniGameStarted;
    private MixingState currentMixingState = MixingState.None;

    private ILightMixable currentMixable;
    IGameStateService gameStateService;


    private Color startColor;
    private Color currentColor;
    private Color goalColor;
    private float startColorValue;
    private float startColorSaturation;
    private float goalColorValue;
    private float goalColorSaturation;
    private float currentColorValue;
    private float minColVal = 0.1f;
    private float maxColVal = 0.9f;
    private float startPercentage = 0;
    private Vector2 startPercPoint = new();
    private Vector2 starColorPoint = new();
    private float firstSlope = 0f;
    private float secondSlope = 0f;

    private float saturationStepsDown;
    private float saturationStepsUp;
    private float valueStepsDown;
    private float valueStepsUp;
    private float colorFullDistance;

    //SpeedCalc
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private Vector2 minPos;
    private Vector2 maxPos;
    private float line1Pos;
    private float line2Pos;

    private void Awake()
    {

        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        gameStateService = serviceLocator.GetService<IGameStateService>();
        interactionPlane = new Plane(Vector3.forward, movementPlane.transform.position);
        lightMixGameView.Controller = this;

        lightMixGameView.EndMinigame += EndMiniGame;
        lightMixGameView.OnFinishConfirmed += FinishMiniGame;
        lightCollider.MixingColliderChanged += OnLightColliderChanged;
        darknessCollider.MixingColliderChanged += OnDarknessColliderChanged;
        finishedTrigger.MixingFinishedTriggered += OnFinishedTriggered;
    }

    ~LightMixerController()
    {
        lightMixGameView.EndMinigame -= EndMiniGame;
        lightMixGameView.OnFinishConfirmed -= FinishMiniGame;
        lightCollider.MixingColliderChanged -= OnLightColliderChanged;
        darknessCollider.MixingColliderChanged -= OnDarknessColliderChanged;
        finishedTrigger.MixingFinishedTriggered -= OnFinishedTriggered;
    }
    // Start is called before the first frame update
    void Start()
    {

        //TODO service?
        // cutGameView.Controller = this;
        // cutGameView.InitiatePointCondition(perfectScoreMargin, rankMargin, maxRanks);
        // //StartCuttingMiniGame();

        // cutGameView.EndMinigame += EndCuttingMiniGame;
        colorFullDistance = Vector2.Distance(new Vector2(minColVal, maxColVal), new Vector2(maxColVal, minColVal));

        minPos = new Vector2(minColVal, maxColVal);
        maxPos = new Vector2(maxColVal, minColVal);
        line1Pos = 1f;
        line2Pos = 0f;

        // firstSlope = (maxColVal - startColorSaturation) / (minColVal - startColorValue);
        // secondSlope = (maxColVal- startColorSaturation) / ( minColVal - startColorValue);
        //Debug.Log("FullDistance:" + colorFullDistance);
        Reset();
    }

    public void Reset()
    {

        lightCollider.gameObject.SetActive(false);
        darknessCollider.gameObject.SetActive(false);
        miniGameStarted = false;
        startColor = Color.white;
        finishedTrigger.enabled = false;
        receivedRank = false;
        lightMixGameView.Reset();
        if (currentMiniGameGO != null)
        {
            currentMiniGameGO.layer = 0;
        }
    }

    private void StartLightMixingMiniGame()
    {
        startColor = currentMixable.FluidColor;
        backgroundPlane.SetActive(true);


        mainCamera.gameObject.SetActive(false);
        miniGameCamera.gameObject.SetActive(true);

        miniGameStarted = true;
        lightCollider.gameObject.SetActive(true);
        darknessCollider.gameObject.SetActive(true);


        lightMixGameView.EnableCanvasGroup(true);

        gameStateService.ChangeState(GameState.Minigame);
        currentMiniGameGO.layer = 6;

        finishedTrigger.enabled = true;

        SetUpGame();
    }

    private void SetUpGame()
    {
        Color.RGBToHSV(startColor, out float colorHue, out float colorSat, out float colorVal);


        startColorSaturation = colorSat;
        startColorValue = colorVal;
        Vector2 maxV = new Vector2(maxColVal, minColVal);
        Vector2 minV = new Vector2(minColVal, maxColVal);
        starColorPoint = new Vector2(startColorSaturation, startColorValue);

        firstSlope = (startColorSaturation - minColVal) / (startColorValue - maxColVal);
        secondSlope = (maxColVal - startColorSaturation) / (minColVal - startColorValue);

        Debug.Log($"Slope1: {firstSlope} = {(startColorSaturation - minColVal)} / {(startColorValue - maxColVal)};");
        Debug.Log($"Slope1: {firstSlope} = {startColorSaturation} - {minColVal} / {startColorValue} - {maxColVal};");
        Debug.Log($"Slope3: {secondSlope} = {(maxColVal - startColorSaturation)} / {(minColVal - startColorValue)};");
        Debug.Log($"Slope3: {secondSlope} = {maxColVal} - {startColorSaturation} / {minColVal} - {startColorValue};");

        //TEEEST
        var a = Vector2.Distance(minV, new Vector2(startColorSaturation, startColorValue));
        var b = Vector2.Distance(new Vector2(startColorSaturation, startColorValue), maxV);
        var c = Vector2.Distance(minV, maxV);

        //Debug.Log("a: " + a + " b: " + b + " c: " +c);

        var alpha = Mathf.Acos((-0.5f * a * a + 0.5f * b * b + 0.5f * c * c) / (b * c)) * Mathf.Rad2Deg;
        var beta = Mathf.Acos((0.5f * a * a - 0.5f * b * b + 0.5f * c * c) / (a * c)) * Mathf.Rad2Deg;
        var gamma = 180 - alpha - beta;

        var delta = 180 - 90 - alpha;
        var d = Mathf.Sin(delta) * b;


        var v2a = b * Mathf.Sin(alpha * Mathf.Deg2Rad);
        var v2b = Mathf.Sqrt(-v2a * v2a + b * b);

        var v2op = c - v2b;
        startPercentage = v2op / c;
        startPercPoint = Vector2.MoveTowards(minV, maxV, v2op);
        //Debug.Log("startPercPoint" + startPercPoint);

        // Debug.Log("Alpha: " + alpha +", beta" + beta + " gamma: " + gamma);
        // Debug.Log("v2a: " + v2a + " v2b:" + v2b + " -> v2op: "+v2op + " startPercentage:" + startPercentage );

        var randomVal = UnityEngine.Random.Range(0.1f, 0.9f);
        if (Math.Abs(colorVal - randomVal) <= 0.1)
        {
            Debug.Log($"ColorVal: {colorVal} vs randomVal: {randomVal} -> {Math.Abs(colorVal - randomVal)}");
        }

        var newSat = ReceiveSaturationFromValue(randomVal);
        goalColor = Color.HSVToRGB(colorHue, newSat, randomVal);
        goalColorValue = randomVal;
        goalColorSaturation = newSat;

        lightMixGameView.SetUpDebug(startPercPoint, new Vector2(newSat, randomVal), new Vector2(newSat, randomVal));
        lightMixGameView.SetUpView(startColor, goalColor);


        //TODO dbg
        dbgCube.material.color = new Color(goalColor.r, goalColor.g, goalColor.b);
    }

    private float ReceiveSaturationFromValue(float val)
    {
        var point = 0f;
        //Debug.Log("Val:" + val + " >= startColorSaturation " + startColorValue);
        if (val * 100f >= startColorValue * 100f)
        {
            // Debug.Log("minColVal:"+minColVal+" <= startColorSaturation.y " + startColorSaturation);
            // Debug.Log($"{val}*{firstSlope} + {maxColVal}");
            point = (val - maxColVal) * firstSlope + minColVal;
            //Debug.Log($"AAAANew Point: {point} -> ({val} - {maxColVal}) * {firstSlope} + {minColVal}");
        }
        else
        {
            //Debug.Log($"{val}*{secondSlope} + {startColorValue}");
            //point = (val - startColorValue) * secondSlope + startColorSaturation;
            point = (val - startColorValue) * secondSlope + startColorSaturation;
            //Debug.Log($"BBBBNew Point: {point} -> ({val} - {startColorValue}) * {secondSlope} + {startColorSaturation}");
        }
        return point;
    }

    private void EndMiniGame()
    {
        backgroundPlane.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        miniGameCamera.gameObject.SetActive(false);


        lightCollider.gameObject.SetActive(false);
        darknessCollider.gameObject.SetActive(false);
        miniGameStarted = false;
        currentMiniGameGO.layer = 0;

        finishedTrigger.enabled = false;

        lightMixGameView.EnableCanvasGroup(false);
        gameStateService.ChangeState(GameState.Main);
        if (!receivedRank)
        {
            currentMixable.FluidColor = startColor;
        }
        currentMixable.ActiveRigidbody(true);
        currentMixable.Place(cancelledPlacement.position);
        receivedRank = false;

        currentMixable = null;
        currentMiniGameGO = null;
        currentPickable = null;
    }

    private void FinishMiniGame()
    {
        Debug.Log("Calculation");
        currentColorValue = currentColorValue == 0f ? 0.01f : currentColorValue;
        //var rank = goalColorValue / currentColorValue;
        int rank = -1;
        var rankCalc = Math.Abs(goalColorValue - currentColorValue);
        Debug.Log($"RankCalc: { rankCalc} ({goalColorValue} - {currentColorValue} = {goalColorValue-currentColorValue})");
        if(rankCalc <= perfectScoreMargin){
            rank = 0;
        }else{
            rankCalc -= perfectScoreMargin;
            for (int i = 0; i < maxRanks; i++)
            {
                rankCalc -= rankMargin;
                if(rankCalc <= 0f){
                    rank = i+1;
                    break;
                }
            }
        }
        rank = rank == -1 ? maxRanks : rank;
        Debug.Log($"Rank: {rank} (" + "ABCDEFGH"[rank] + ")");
        receivedRank = true;
        currentMixable.Mix(rank);
        lightMixGameView.DisplayRank("ABCDEFGH"[rank]+"");
    }

    void Update()
    {
        if (!interactionEnabled) return;
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("MouseUp, pickable: " + currentPickable);
            if (currentPickable != null)
            {
                currentPickable.Release();
                currentPickable = null;
            }
        }

        if (currentPickable != null)
        {
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (interactionPlane.Raycast(ray, out float ent))
                {
                    var hitPoint = ray.GetPoint(ent);

                    currentPickable.UpdatePosition(hitPoint);
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {

            Ray forwardRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 6;
            RaycastHit hit;
            if (!Physics.Raycast(forwardRay, out hit, 100, layerMask))
            {
                return;
            }

            IPickable pickable = hit.collider.gameObject.GetComponent<IPickable>();
            if (pickable != null)
            {
                currentPickable = pickable;
                pickable.PickUp();
                currentPosition = previousPosition = currentMiniGameGO.transform.position;
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentPickable != null)
        {
            currentPosition = currentMiniGameGO.transform.position;
            if (currentPosition != previousPosition && currentMixingState != MixingState.None)
            {
                var pointer = currentPosition - previousPosition;
                float speed = Vector3.Distance(currentPosition, previousPosition) / (Time.deltaTime * 10000f);
                Debug.Log("Speed: " + speed);
                UpdateColor(speed);

            }
            previousPosition = currentPosition;
        }
    }

    private void UpdateColor(float addVal)
    {
        currentColor = new Color(currentMixable.FluidColor.r, currentMixable.FluidColor.g, currentMixable.FluidColor.b);
        Color.RGBToHSV(currentColor, out float newHue, out float newSat, out float newVal);
       bool moveOnFirst;
        Vector2 newPos = new();
        switch (currentMixingState)
        {
            case MixingState.Light:
                if (newVal >= maxColVal)
                {
                    newVal = 0.9f;
                    return;
                }
                moveOnFirst = true;


                if(line1Pos >= 1){
                    if(line2Pos > 0){
                        // line2Pos -= addVal;
                        // line2Pos = line2Pos < 0f ? 0f: line2Pos;
                        // newPos= Vector2.Lerp(startPercPoint, maxPos, line2Pos);   
                        moveOnFirst = false;                     
                    }else{
                        moveOnFirst = true;
                    }
                }
                //newVal += addVal;
                //if (newVal >= maxColVal) newVal = maxColVal;


                if(moveOnFirst){
                    line1Pos -= addVal;
                    line1Pos = line1Pos < 0f ? 0f: line1Pos;
                    newPos= Vector2.Lerp(minPos, starColorPoint, line1Pos);
                }else{
                    line2Pos -= addVal;
                    line2Pos = line2Pos < 0f ? 0f: line2Pos;
                    newPos= Vector2.Lerp(starColorPoint, maxPos, line2Pos);    
                }
                break;
            case MixingState.Darkness:

                moveOnFirst = true;
                if(line1Pos >= 1){                    
                    moveOnFirst = false; 
                }

                if(moveOnFirst){
                    line1Pos += addVal;
                    line1Pos = line1Pos > 1f ? 1f: line1Pos;
                    newPos= Vector2.Lerp(minPos, starColorPoint, line1Pos);
                }else{
                    line2Pos += addVal;
                    line2Pos = line2Pos > 1f ? 1f: line2Pos;
                    newPos= Vector2.Lerp(starColorPoint, maxPos, line2Pos);    
                }               
                break;
        }
        // var calcSat = ReceiveSaturationFromValue(newVal);
        // //TODO to debug
        // lightMixGameView.SetUpDebug(startPercPoint, new Vector2(calcSat, newVal), new Vector2(goalColorSaturation, goalColorValue));
         lightMixGameView.SetUpDebug(startPercPoint, newPos, new Vector2(goalColorSaturation, goalColorValue));

        Color newCol = Color.HSVToRGB(newHue, newPos.x, newPos.y);
        currentColorValue = newVal;
        currentMixable.FluidColor = newCol;
    }

    private void ColorCalculator()
    {

    }


    private void OnLightColliderChanged(bool entered, Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        //Debug.Log($"OnLightColliderChanged otherMixable = {otherMixable}, same as current? {otherMixable == currentMixable}");
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        currentMixingState = entered ? MixingState.Light : MixingState.None;
    }
    private void OnDarknessColliderChanged(bool entered, Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        //Debug.Log($"OnDarknessColliderChanged otherMixable = {otherMixable}, same as current? {otherMixable == currentMixable}");
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        currentMixingState = entered ? MixingState.Darkness : MixingState.None;
    }

    private void OnFinishedTriggered(Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        //Debug.Log($"OnFinishedTriggered otherMixable = {otherMixable}, same as current? {otherMixable == currentMixable}");
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        lightMixGameView.DisplayFinishConfirmation(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (miniGameStarted) return;
        //Debug.Log($"[LightMixerController:OnCollisionEnter] other={other.gameObject.name}");

        var ingredient = other.gameObject.GetComponent<Ingredient>();
        if (ingredient == null) return;

        ILightMixable lightMixable = ingredient as ILightMixable;
        //Debug.Log($"[LightMixerController:OnCollisionEnter] mixable={lightMixable}");

        if (lightMixable != null)
        {
            Debug.Log($"[LightMixerController:OnCollisionEnter] found ILightMixable");
        }
        else
        {
            return;
        }

        if (!lightMixable.IsMixed)
        {
            currentMiniGameGO = other.gameObject;
            currentMixable = lightMixable;
            currentMixable.ActiveRigidbody(false);
            lightMixable.Place(startPlacement.position);
            StartLightMixingMiniGame();
        }
        else
        {
            Debug.Log($"[LightMixerController:OnCollisionEnter] already mixed");
        }
    }
}