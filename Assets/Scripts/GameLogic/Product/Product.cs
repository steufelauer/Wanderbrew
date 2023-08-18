using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour, ISellable, IHoverable, IPickable
{

    [SerializeField] private string productName = "Unnamed Potion";
    private List<IngredientAspect> aspectDetails;
    public Rigidbody MyRigidBody;

    public bool IsPickedUp => isPickedUp;

    public GameObject MyGameObject => myGameObject;

    public List<IngredientAspect> AspectDetails => aspectDetails;

    public int ValueMultiplier => valueMultiplier;

    private bool isPickedUp = false;
    private GameObject myGameObject;
    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uIProviderService;
    private int valueMultiplier;

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
        this.aspectDetails = baseAspectDetails;
    }

    public void InitiateHover(Vector3 pos)
    {
        if(pos == Vector3.zero)
            pos = transform.position;
         
        InitiateHover(pos, productName);
    }

    public void InitiateHover(Vector3 pos, string txt)
    {
        if (aspectDetails.Count >= 2)
        {
            tooltipProvider.ShowAspectTextToolTipMultipleAspects(txt, aspectDetails, pos);
        }
        else
        {
            tooltipProvider.ShowAspectTextToolTip(txt, aspectDetails[0], aspectDetails[0].Points, pos);
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
