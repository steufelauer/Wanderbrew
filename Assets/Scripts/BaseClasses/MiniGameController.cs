
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameController : MonoBehaviour
{
    [SerializeField] protected MiniGameView gameView;
    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected Camera miniGameCamera;
    [SerializeField] protected Transform startPlacement;
    [SerializeField] protected float perfectScoreMargin = 0.05f;
    [SerializeField] protected float rankMargin = 0.1f;
    [SerializeField] protected int maxRanks = 3;

    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uiProviderService;
    protected IGameStateService gameStateService;

    
    protected bool miniGameStarted = false;



    protected virtual void Awake()
    {

        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        gameStateService = serviceLocator.GetService<IGameStateService>();
        
        uiProviderService = serviceLocator.GetService<IUIProviderService>();
        tooltipProvider = uiProviderService.GetProvider<TooltipProvider>();

        gameView.EndMinigame += EndMiniGame;
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        Reset();
    }

    protected virtual void Reset()
    {
        miniGameStarted = false;
        gameView.Reset();
    }

    protected virtual void OnDestroy()
    {
        Debug.Log("MG Controller OnDestroy");
        gameView.EndMinigame -= EndMiniGame;
    }

    protected virtual void StartMiniGame()
    {
        Reset();
        miniGameStarted = true;
        gameView.Reset();
        gameView.EnableCanvasGroup(true);
        mainCamera.gameObject.SetActive(false);
        miniGameCamera.gameObject.SetActive(true);
        gameStateService.ChangeState(GameState.Minigame);
        tooltipProvider.HideToolTip();

        SetUpGame();
    }

    protected virtual void EndMiniGame()
    {
        mainCamera.gameObject.SetActive(true);
        miniGameCamera.gameObject.SetActive(false);
        miniGameStarted = false;

        gameView.EnableCanvasGroup(false);
        gameStateService.ChangeState(GameState.Main);
       
    }

    // private void FinishMiniGame()
    // {

    // }
    protected abstract void SetUpGame();

    protected abstract int CalculateRank();

}