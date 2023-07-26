
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMixerController : MonoBehaviour
{
    [SerializeField] private LightMixGameView cutGameView;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera miniGameCamera;
    [SerializeField] private Transform startPlacement;
    [SerializeField] private int roundCount = 5;
    [SerializeField] private int maxRanks = 3;
    [SerializeField] private GameObject backgroundPlane;
    [SerializeField] private GameObject movementPlane;

    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uIProviderService;

    IPickable currentPickable;
    private Plane interactionPlane;
    private bool miniGameStarted = false;
    private bool waitingForInput = false;

    private bool interactionEnabled => miniGameStarted;

    private ILightMixable currentMixable;
    IGameStateService gameStateService;

    private void Awake()
    {

        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        gameStateService = serviceLocator.GetService<IGameStateService>();
        interactionPlane = new Plane(Vector3.forward, movementPlane.transform.position);
    }

    ~LightMixerController()
    {

        //cutGameView.EndMinigame -= EndCuttingMiniGame;
    }
    // Start is called before the first frame update
    void Start()
    {
        //TODO service?
        // cutGameView.Controller = this;
        // cutGameView.InitiatePointCondition(perfectScoreMargin, rankMargin, maxRanks);
        // //StartCuttingMiniGame();

        // cutGameView.EndMinigame += EndCuttingMiniGame;
    }

    private void StartLightMixingMiniGame()
    {
        backgroundPlane.SetActive(true);

        
        mainCamera.gameObject.SetActive(false);
        miniGameCamera.gameObject.SetActive(true);

        miniGameStarted = true;
        
        gameStateService.ChangeState(GameState.Minigame);
    }

    private void EndMiniGame()
    {
        backgroundPlane.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        miniGameCamera.gameObject.SetActive(false);

        miniGameStarted = false;
    }

    void Update()
    {        
        if(!interactionEnabled) return;
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
            RaycastHit hit;
            if (!Physics.Raycast(forwardRay, out hit, 100))
            {
                return;
            }

            IPickable pickable = hit.collider.gameObject.GetComponent<IPickable>();
            if (pickable != null)
            {
                currentPickable = pickable;
                pickable.PickUp();
            }
        }
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