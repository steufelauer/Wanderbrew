using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AspectAlignment
{

    None,
    Healing,
    Poison,
    Fire,
    Luck
}

public class Aspect : MonoBehaviour, IHoverable, IPickable
{
    // Start is called before the first frame update
    [SerializeField] private string aspectName;

    [SerializeField] private AspectAlignment alignment;

    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uIProviderService;

    public bool IsPickedUp => isPickedUp;
    private bool isPickedUp = false;

    void Start()
    {
        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        uIProviderService = serviceLocator.GetService<IUIProviderService>();
        tooltipProvider = uIProviderService.GetProvider<TooltipProvider>();
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
        Vector3 newPos = Camera.main.ScreenToWorldPoint(pos);
        tooltipProvider.ShowTextToolTip(txt, pos);
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

        InitiateHover(pos, aspectName);
    }

}
