
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderController : MonoBehaviour
{

    [SerializeField] private OrderView orderView;    
    [SerializeField] private RecipesScriptableObject activeOrders;    
    private IServiceLocator serviceLocator;
    private TooltipProvider tooltipProvider;
    private IUIProviderService uIProviderService;
    private IGameStateService gameStateService;

    private Recipe currentOrder;

    private void Awake() {
        
    }
    private void Start() {
        ActivateOrder(0);
    }

    private void ActivateOrder(int recipeNr = -1){
        orderView.DisplayOrder(activeOrders.myRecipes[recipeNr]);
    }
}