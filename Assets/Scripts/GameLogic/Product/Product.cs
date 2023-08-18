using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour, ISellable, IHoverable, IPickable
{

    [SerializeField] private string productName = "Unnamed Potion";
    [SerializeField] protected List<IngredientAspect> baseAspectDetails;
    public Rigidbody MyRigidBody;

    public bool IsPickedUp => isPickedUp;

    public GameObject MyGameObject => myGameObject;

    private bool isPickedUp = false;
    private GameObject myGameObject;
    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uIProviderService;

    // Start is called before the first frame update
    public virtual void Start()
    {
        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        uIProviderService = serviceLocator.GetService<IUIProviderService>();
        tooltipProvider = uIProviderService.GetProvider<TooltipProvider>();

        myGameObject = this.gameObject;
        MyRigidBody = GetComponent<Rigidbody>();
    }

    public void SetUp(List<IngredientAspect> baseAspectDetails){
        this.baseAspectDetails = baseAspectDetails;
    }

    public void InitiateHover(Vector3 pos)
    {
        if(pos == Vector3.zero)
            pos = transform.position;
         
        InitiateHover(pos, productName);
    }

    public void InitiateHover(Vector3 pos, string txt)
    {
        if (baseAspectDetails.Count >= 2)
        {
            tooltipProvider.ShowAspectTextToolTipMultipleAspects(txt, baseAspectDetails, pos);
        }
        else
        {
            tooltipProvider.ShowAspectTextToolTip(txt, baseAspectDetails[0], baseAspectDetails[0].Points, pos);
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

    public void PickUp()
    {
        isPickedUp = true;
    }

    public void Release()
    {
        isPickedUp = false;
    }

    public void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
