using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipProvider : MonoBehaviour, IUIProvider
{
    [SerializeField] private TextTooltipHandler textTooltipHandler;

    protected IUIProviderService uIProviderService;
    protected IServiceLocator serviceLocator;



    private void Awake() {
        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        uIProviderService = serviceLocator.GetService<IUIProviderService>();
        uIProviderService.AddProvider<TooltipProvider>(this);
    }


    public void ShowTextToolTip(string text, Vector3 position) => textTooltipHandler.ShowTooltip(text, position);
    public void ShowAspectTextToolTip(string text, IngredientAspect detail, Vector3 position) => textTooltipHandler.ShowAspectTooltip(text, detail, position);
    public void UpdatePositionTextToolTip(Vector3 position) => textTooltipHandler.UpdatePosition(position);
    public void HideToolTip() => textTooltipHandler.HideToolTip();
}
