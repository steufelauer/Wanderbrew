
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMixerController : MiniGameController
{
    enum MixingState { None, Light, Darkness };
    [SerializeField] private Transform cancelledPlacement;
    [SerializeField] private GameObject backgroundPlane;
    [SerializeField] private GameObject movementPlane;
    [SerializeField] private TriggerArea finishedTrigger;
    [SerializeField] private MixingCollider lightCollider;
    [SerializeField] private MixingCollider darknessCollider;
    [SerializeField] private float timeToFin = 3f;
    [SerializeField] private MeshRenderer dbgCube;

    
    public event Action<float> OnUpdateTimer = delegate {} ;

    private LightMixGameView lightMixGameView;
    private IPickable currentPickable;

    private GameObject currentMiniGameGO;
    private Plane interactionPlane;
    private bool receivedRank = true;
    private float timer = 0f;
    private bool inFinishArea = false;

    private bool interactionEnabled => miniGameStarted;
    private MixingState currentMixingState = MixingState.None;

    private ILightMixable currentMixable;


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
    private Vector2 starColorPoint = new();
    private float firstSlope = 0f;
    private float secondSlope = 0f;



    private float colorFullDistance;

    //SpeedCalc
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private Vector2 minPos;
    private Vector2 maxPos;
    private float line1Pos;
    private float line2Pos;

    protected override void Awake()
    {
        base.Awake();
        interactionPlane = new Plane(Vector3.forward, movementPlane.transform.position);
        
        lightMixGameView = gameView as LightMixGameView;
        if(lightMixGameView == null){
            Debug.LogError($"[LightMixerController::Awake] Could not cast gameView as LightMixGameView");
        }
        lightMixGameView.Controller = this;
        lightMixGameView.OnFinishConfirmed += FinishMiniGame;
        lightCollider.MixingColliderChanged += OnLightColliderChanged;
        darknessCollider.MixingColliderChanged += OnDarknessColliderChanged;
        finishedTrigger.AreaTriggerEntered += OnFinishedTriggered;
        finishedTrigger.AreaTriggeredExited += OnFinishedTriggerExited;
    }

    ~LightMixerController()
    {
        lightMixGameView.OnFinishConfirmed -= FinishMiniGame;
        lightCollider.MixingColliderChanged -= OnLightColliderChanged;
        darknessCollider.MixingColliderChanged -= OnDarknessColliderChanged;
        finishedTrigger.AreaTriggerEntered -= OnFinishedTriggered;
        finishedTrigger.AreaTriggeredExited -= OnFinishedTriggerExited;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


        colorFullDistance = Vector2.Distance(new Vector2(minColVal, maxColVal), new Vector2(maxColVal, minColVal));

        minPos = new Vector2(minColVal, maxColVal);
        maxPos = new Vector2(maxColVal, minColVal);
        line1Pos = 1f;
        line2Pos = 0f;        
    }

    protected override void Reset()
    {
        base.Reset();
        lightCollider.gameObject.SetActive(false);
        darknessCollider.gameObject.SetActive(false);
        startColor = Color.white;
        finishedTrigger.enabled = false;
        receivedRank = false;
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

    protected override void SetUpGame()
    {
        Color.RGBToHSV(startColor, out float colorHue, out float colorSat, out float colorVal);


        startColorSaturation = colorSat;
        startColorValue = colorVal;
        starColorPoint = new Vector2(startColorSaturation, startColorValue);

        firstSlope = (startColorSaturation - minColVal) / (startColorValue - maxColVal);
        secondSlope = (maxColVal - startColorSaturation) / (minColVal - startColorValue);


        var randomVal = UnityEngine.Random.Range(minColVal, maxColVal);

        var newSat = ReceiveSaturationFromValue(randomVal);
        goalColor = Color.HSVToRGB(colorHue, newSat, randomVal);
        goalColorValue = randomVal;
        goalColorSaturation = newSat;

        lightMixGameView.SetUpDebug(new Vector2(newSat, randomVal), new Vector2(newSat, randomVal));
        lightMixGameView.SetUpView(startColor, goalColor);


        //TODO dbg
        dbgCube.material.color = new Color(goalColor.r, goalColor.g, goalColor.b);
    }

    private float ReceiveSaturationFromValue(float val)
    {
        float point;
        if (val * 100f >= startColorValue * 100f)
        {
            point = (val - maxColVal) * firstSlope + minColVal;
        }
        else
        {
            point = (val - startColorValue) * secondSlope + startColorSaturation;
        }
        return point;
    }

    protected override void EndMiniGame()
    {
        base.EndMiniGame();
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
        int rank = CalculateRank();
        receivedRank = true;
        currentMixable.Mix(rank);
        lightMixGameView.DisplayRank("ABCDEFGH"[rank]+"");
    }

    protected override int CalculateRank()
    {
       currentColorValue = currentColorValue == 0f ? 0.01f : currentColorValue;
        int rank = -1;
        var rankCalc = Math.Abs(goalColorValue - currentColorValue);
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

        return rank;
    }

    void Update()
    {

        if (timer > 0f)
        {
            if (inFinishArea)
            {
                timer -= Time.deltaTime;
                OnUpdateTimer(1f-timer/timeToFin);
                if (timer <= 0f)
                {
                    timer = 0.0f;
                    FinishMiniGame();
                }
            }
            else
            {
                timer = 0.0f;
                OnUpdateTimer(0f);
            }
        }

        if (!interactionEnabled) return;
        if (Input.GetMouseButtonUp(0))
        {
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
                        moveOnFirst = false;                     
                    }else{
                        moveOnFirst = true;
                    }
                }

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
         lightMixGameView.SetUpDebug(newPos, new Vector2(goalColorSaturation, goalColorValue));

        Color newCol = Color.HSVToRGB(newHue, newPos.x, newPos.y);
        currentColorValue = newVal;
        currentMixable.FluidColor = newCol;
    }


    private void OnLightColliderChanged(bool entered, Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        currentMixingState = entered ? MixingState.Light : MixingState.None;
    }
    private void OnDarknessColliderChanged(bool entered, Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        currentMixingState = entered ? MixingState.Darkness : MixingState.None;
    }

    private void OnFinishedTriggered(Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        lightMixGameView.DisplayFinishedTimer(true);
        inFinishArea = true;
        timer = timeToFin;
    }
        private void OnFinishedTriggerExited(Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        lightMixGameView.DisplayFinishedTimer(false);
        inFinishArea = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (miniGameStarted) return;

        var ingredient = other.gameObject.GetComponent<Ingredient>();
        if (ingredient == null) return;

        ILightMixable lightMixable = ingredient as ILightMixable;

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