using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionHighlighter : MonoBehaviour
{
    [SerializeField] private Texture2D highlightCursor;
    private Vector3 lastMousePosition = Vector3.one;

    IHoverable currentHoverable;
    bool interactionEnabled => gameStateService.CurrentGameState == GameState.Main;
    bool hoverableEnabled = true;

    IGameStateService gameStateService;
    protected IServiceLocator serviceLocator;
    
    Plane plane = new Plane(Vector3.up, Vector3.up*3f);

    private void Awake() {
        
        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        gameStateService = serviceLocator.GetService<IGameStateService>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isFocused || !interactionEnabled)
            return;

        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition == lastMousePosition)
            return;
        lastMousePosition = mousePosition;

        Ray forwardRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(forwardRay, out hit, 100))
        {
            return;

        }

        UpdateHoverable(hit);
    }

    public void UpdateHoverable(RaycastHit hit){
        if(!hoverableEnabled)
            return;
        IHoverable hoverable = hit.collider.gameObject.GetComponent<IHoverable>();

        if (hoverable == currentHoverable)
            return;
        if (hoverable == null && currentHoverable != null)
        {
            currentHoverable.DisableHover();
            currentHoverable = null;
            return;
        }
        // if (currentHoverable != null && currentHoverable == hoverable)
        // {
        //     currentHoverable.UpdateHover(mousePosition);
        //     return;
        // }

        hoverable.InitiateHover(Vector3.zero);
        currentHoverable = hoverable;
    }
}

