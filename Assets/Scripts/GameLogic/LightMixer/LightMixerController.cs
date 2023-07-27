
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
    [SerializeField] private int roundCount = 5;
    [SerializeField] private int maxRanks = 3;
    [SerializeField] private GameObject backgroundPlane;
    [SerializeField] private GameObject movementPlane;
    [SerializeField] private MixingFinishedTrigger finishedTrigger;
    [SerializeField] private MixingCollider lightCollider;
    [SerializeField] private MixingCollider darknessCollider;

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
    private float goalColorValue;
    private float currentColorValue;

    //SpeedCalc
    private float speed;
    private Vector3 previousPosition;
    private Vector3 currentPosition;

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

        var randomVal = UnityEngine.Random.Range(0.05f, 0.95f);
        Color.RGBToHSV(startColor, out float colorHue, out float colorSat, out float colorVal);
        if (Math.Abs(colorVal - randomVal) <= 0.1)
        {
            Debug.Log($"ColorVal: {colorVal} vs randomVal: {randomVal} -> {Math.Abs(colorVal - randomVal)}");
        }
        goalColor = Color.HSVToRGB(colorHue, colorSat, randomVal);


        lightMixGameView.SetUpView(startColor, goalColor);
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
        receivedRank = false;
    }

    private void FinishMiniGame()
    {
        Debug.Log("Calculation");
        currentColorValue = currentColorValue == 0f ? 0.01f : currentColorValue;
        var rank = goalColorValue / currentColorValue;
        Debug.Log($"Rank: {rank} (" + "ABCDEFGH"[(int)rank] + ")");
        receivedRank = true;
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
        switch (currentMixingState)
        {
            case MixingState.Light:
                newVal += addVal;
                break;
            case MixingState.Darkness:
                newVal -= addVal;
                break;
        }
        Color newCol = Color.HSVToRGB(newHue, newSat, newVal);
        currentMixable.FluidColor = newCol;
    }


    private void OnLightColliderChanged(bool entered, Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        Debug.Log($"OnLightColliderChanged otherMixable = {otherMixable}, same as current? {otherMixable == currentMixable}");
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        currentMixingState = entered ? MixingState.Light : MixingState.None;
    }
    private void OnDarknessColliderChanged(bool entered, Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        Debug.Log($"OnDarknessColliderChanged otherMixable = {otherMixable}, same as current? {otherMixable == currentMixable}");
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        currentMixingState = entered ? MixingState.Darkness : MixingState.None;
    }

    private void OnFinishedTriggered(Collider other)
    {
        ILightMixable otherMixable = other.gameObject.GetComponent<ILightMixable>();
        Debug.Log($"OnFinishedTriggered otherMixable = {otherMixable}, same as current? {otherMixable == currentMixable}");
        if (otherMixable == null || otherMixable != currentMixable)
        {
            return;
        }
        lightMixGameView.DisplayFinishConfirmation(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (miniGameStarted) return;
        Debug.Log($"[LightMixerController:OnCollisionEnter] other={other.gameObject.name}");

        var ingredient = other.gameObject.GetComponent<Ingredient>();
        if (ingredient == null) return;

        ILightMixable lightMixable = ingredient as ILightMixable;
        Debug.Log($"[LightMixerController:OnCollisionEnter] mixable={lightMixable}");

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