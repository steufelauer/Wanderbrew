
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderController : MonoBehaviour
{

    [SerializeField] private OrderView orderView;
    [SerializeField] private RecipesScriptableObject activeOrders;
    [SerializeField] private bool enableAll = true;
    private IServiceLocator serviceLocator;
    private TooltipProvider tooltipProvider;
    private IUIProviderService uIProviderService;
    private IGameStateService gameStateService;

    private Recipe currentOrder;

    private void Awake()
    {

    }
    private void Start()
    {
        ActivateOrder(0);
        if (enableAll)
        {
            for (int i = 1; i < activeOrders.myRecipes.Count; i++)
            {
                ActivateOrder(i);
            }
        }
    }

    private void ActivateOrder(int recipeNr = -1)
    {
        orderView.DisplayOrder(activeOrders.myRecipes[recipeNr]);
    }
}