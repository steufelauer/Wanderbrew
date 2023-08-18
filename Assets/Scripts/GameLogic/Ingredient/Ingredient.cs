using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour, IPickable, IHoverable
{
    // Start is called before the first frame update
    [SerializeField] private string ingredientName;
    [SerializeField] protected IngredientAspect baseAspectDetail;
    public Rigidbody MyRigidBody;
    public IngredientAspect ReachedAspectDetail { get => reachedAspectDetail; set => reachedAspectDetail = value; }


    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uIProviderService;

    public bool IsPickedUp => isPickedUp;
    private bool isPickedUp = false;

    public virtual bool IsPrepared => isPrepared;

    public GameObject MyGameObject => myGameObject;


    private GameObject myGameObject;

    private bool isPrepared = false;
    protected IngredientAspect reachedAspectDetail = new();

    public virtual void Start() {
        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        uIProviderService = serviceLocator.GetService<IUIProviderService>();
        tooltipProvider = uIProviderService.GetProvider<TooltipProvider>();

        myGameObject = this.gameObject;
        MyRigidBody = GetComponent<Rigidbody>();
    }

    public Ingredient(){
    }

    public void PickUp()
    {
        //Debug.Log("Picking Up");
        isPickedUp = true;
    }

    public void Release()
    {
        //Debug.Log("Release");
        isPickedUp = false;
    }

    public void UpdatePosition(Vector3 pos)
    {
        //Debug.Log("UpdatePosition");
        transform.position = pos;
    }

    public void InitiateHover(Vector3 pos, string txt)
    {
        tooltipProvider.ShowTextToolTip(txt, pos);
        if (IsPrepared)
        {
            tooltipProvider.ShowAspectTextToolTip("Prepared " + txt, baseAspectDetail, reachedAspectDetail.Points, pos);
        }
        else
        {
            tooltipProvider.ShowAspectTextToolTip(txt, baseAspectDetail, 0f, pos);
        }
    }

    public void UpdateHover(Vector3 pos)
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(pos);
        tooltipProvider.UpdatePositionTextToolTip(newPos);
    }

    public void DisableHover()
    {        
        tooltipProvider.HideToolTip();
    }

    public void InitiateHover(Vector3 pos)
    {
        if(pos == Vector3.zero)
            pos = transform.position;
         
        InitiateHover(pos, ingredientName);
    }
}
