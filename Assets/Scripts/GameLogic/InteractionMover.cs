using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionMover : MonoBehaviour
{
    private Vector3 lastMousePosition = Vector3.one;

    IPickable currentPickable;
    bool hoverEnabled = true;
    Plane plane = new Plane(Vector3.up, Vector3.up*3f);
    
    bool interactionEnabled => gameStateService.CurrentGameState == GameState.Main;
    IGameStateService gameStateService;
    protected IServiceLocator serviceLocator;

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

                //Debug.Log("GetMouseButton, pickable: " + currentPickable);
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (plane.Raycast(ray, out float ent))
                {
                    //Debug.Log("Plane Raycast hit at distance: " + ent);
                    var hitPoint = ray.GetPoint(ent);

                    currentPickable.UpdatePosition(hitPoint);

                    // var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // go.transform.position = hitPoint;
                    //Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
                }
                // else
                //     Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("GetMouseButtonDown, pickable: " + currentPickable);
            // var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // if (plane.Raycast(ray, out float ent))
            // {
            //     Debug.Log("Plane Raycast hit at distance: " + ent);
            //     var hitPoint = ray.GetPoint(ent);

            //     currentPickable.PickUp(hitPoint);

            //     var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //     go.transform.position = hitPoint;
            //     Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
            // }
            // else
            //     Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
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

}

