using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingboardController : MonoBehaviour
{
    [SerializeField] private CutGameView cutGameView;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera miniGameCamera;
    [SerializeField] private int roundCount = 5;
    [Tooltip("In Pixel as radius")]
    [SerializeField] private int perfectScoreMargin = 10;
    [SerializeField] private int rankMargin = 10;
    [SerializeField] private int maxRanks = 3;

    private int finishedRoundCount = 0;

    private List<int> ranks = new();

    protected IServiceLocator serviceLocator;
    protected TooltipProvider tooltipProvider;
    protected IUIProviderService uIProviderService;

    private bool miniGameStarted = false;
    private bool waitingForInput = false;

    private ICutable currentCutable;
    IGameStateService gameStateService;

    private void Awake()
    {

        serviceLocator = ServiceLocatorProvider.GetServiceLocator();
        gameStateService = serviceLocator.GetService<IGameStateService>();
    }

    ~CuttingboardController()
    {

        cutGameView.EndMinigame -= EndCuttingMiniGame;
    }
    // Start is called before the first frame update
    void Start()
    {
        //TODO service?
        cutGameView.Controller = this;
        cutGameView.InitiatePointCondition(perfectScoreMargin, rankMargin, maxRanks);
        //StartCuttingMiniGame();

        cutGameView.EndMinigame += EndCuttingMiniGame;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StartCuttingMiniGame()
    {
        Debug.Log("START MINIGAAAME");
        Reset();
        miniGameStarted = true;
        mainCamera.gameObject.SetActive(false);
        miniGameCamera.gameObject.SetActive(true);
        cutGameView.Reset();
        cutGameView.EnableCanvasGroup(true);
        StartRound();
        cutGameView.EnableLines(true);
        gameStateService.ChangeState(GameState.Minigame);
    }
    private void Reset(){
        finishedRoundCount = 0;
        ranks.Clear();
        miniGameStarted = false;
        waitingForInput = false;
    }

    private void EndCuttingMiniGame()
    {
        Debug.Log("EndCuttingMiniGame");
        miniGameCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        //ranks.Clear();
        //StartRound();
        cutGameView.EnableCanvasGroup(false);
        Reset();

        gameStateService.ChangeState(GameState.Main);
    }

    private void OnMouseDown()
    {
        Debug.Log($"OnMouseDown: miniGameStarted: {miniGameStarted} && waitingForInput:{waitingForInput}");
        if (!miniGameStarted || !waitingForInput) return;
        Debug.Log("OnMouseDown CutGameView! MousePos:" + Input.mousePosition);
        cutGameView.SetMouseStartingPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        Debug.Log($"OnMouseDown: miniGameStarted: {miniGameStarted} && waitingForInput:{waitingForInput}");
        if (!miniGameStarted || !waitingForInput) return;
        Debug.Log("OnMouseUp CutGameView! MousePos:" + Input.mousePosition);
        cutGameView.SetMouseEndPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        if (!miniGameStarted && !waitingForInput) return;
        //Debug.Log("OnMouseDrag CutGameView! MousePos:" + Input.mousePosition);
    }

    public int CalculatePercentOverlap(Vector3 topGoal, Vector3 botGoal, Vector3 topReached, Vector3 botReached, float lineLength)
    {
        Debug.Log("CalculatePercentOverlap waitingForInput:"+waitingForInput);
        if(!waitingForInput) return -1;
        waitingForInput = false;
        int rank = 0;
        var perfectScoreSize = lineLength * perfectScoreMargin / 100;
        var rankScoreSize = lineLength * rankMargin / 100;

        Debug.Log("PerfectScoreDistance: " + perfectScoreSize);
        Debug.Log("rankScoreSize: " + rankScoreSize);

        //TODO 
        if (topReached.y > topGoal.y)
        {
            Debug.LogError($"started too low! (topReached:{topReached.y} < {topGoal.y})");
        }
        if (botReached.y < botGoal.y)
        {
            Debug.LogError($"ended too soon! (botReached:{botReached.y} > {botGoal.y})");
        }
        var topYRank = Mathf.Abs(topGoal.y - topReached.y);
        var topXRank = Mathf.Abs(topGoal.x - topReached.x);
        //Debug.Log($"TopYRank: {topYRank}, TopXRank: {topXRank}");

        var botYRank = Mathf.Abs(botGoal.y - botReached.y);
        var botXRank = Mathf.Abs(botGoal.x - botReached.x);
        //Debug.Log($"botYRank: {botYRank}, botXRank: {botXRank}");

        bool perfectScoreTop = false;
        bool perfectScoreBot = false;
        if (topYRank <= perfectScoreSize && topXRank <= perfectScoreSize)
        {
            Debug.Log($"Perfect score! (Y:{topYRank} X:{topXRank} <={perfectScoreSize})");
            perfectScoreTop = true;
            rank = 0;
        }
        if (botYRank <= perfectScoreSize && botXRank <= perfectScoreSize)
        {
            Debug.Log($"Perfect score! (Y:{botYRank} X:{botXRank} <={perfectScoreSize})");
            perfectScoreBot = true;
            rank = 0;
        }

        if (!perfectScoreTop || !perfectScoreBot)
        {
            botYRank -= perfectScoreSize;
            botXRank -= perfectScoreSize;
            topYRank -= perfectScoreSize;
            topXRank -= perfectScoreSize;

            bool assignedTopRank = perfectScoreTop;
            int topRank = 0;
            for (int i = 0; i < maxRanks && !perfectScoreTop; i++)
            {
                
                Debug.Log($"Testing top on {i} with topYRank={topYRank}, topXRank={topXRank} (<= {rankScoreSize})");
                if (topYRank <= rankScoreSize && topXRank <= rankScoreSize)
                {
                    Debug.Log($"Got Rank {i+1} with topYRank={topYRank}, topXRank={topXRank}");
                    //ranks.Add(i + 1);
                    assignedTopRank = true;
                    topRank = i + 1;
                    break;
                }
                topYRank -= rankScoreSize;
                topXRank -= rankScoreSize;
            }

            bool assignedBotRank = perfectScoreBot;
            int botRank = 0;
            for (int i = 0; i < maxRanks || !perfectScoreBot; i++)
            {
                if (botYRank <= rankScoreSize && botXRank <= rankScoreSize)
                {
                    Debug.Log($"Got Rank {i+1} with topYRank={topYRank}, topXRank={topXRank}, botYRank={botYRank}, botXRank={botXRank}");
                    botRank = i + 1;
                    assignedBotRank = true;
                    break;
                }
                botYRank -= rankScoreSize;
                botXRank -= rankScoreSize;
            }

            Debug.Log($"RAAAANKS: Top: {topRank}, Bot: {botRank} dividedrank: {(topRank + botRank) / 2}");
            if (assignedBotRank && assignedTopRank)
            {
                rank = (topRank + botRank) / 2;
            }
            else if (assignedBotRank)
            {
                rank = botRank + 2;
            }
            else if (assignedTopRank)
            {
                rank = topRank + 2;
            }
            else
            {
                rank = maxRanks + 2;
            }
        }
        ranks.Add(rank);

        finishedRoundCount++;
        if (finishedRoundCount < roundCount)
        {
            StartRound();
        }
        else if (finishedRoundCount >= roundCount)
        {
            waitingForInput = false;
            int finalRank = 0;
            string dbgLog = "";
            for (int i = 0; i < ranks.Count; i++)
            {
                dbgLog += " + "+ranks[i];
                finalRank += ranks[i];
            }
            dbgLog += " ->  " + finalRank;
            finalRank /= ranks.Count;
            dbgLog += " / "+ranks.Count+" -> " +finalRank;
            Debug.Log("FINALRANKCALCULATION: " + dbgLog);
            cutGameView.DisplayFinalRank("Rank " + "ABCDEFG"[finalRank]);
            currentCutable.Cut();
        }
        return rank;
    }


    private void StartRound()
    {
        cutGameView.DrawRandomLine();
        waitingForInput = true;
    }

    private void EndRound()
    {
        cutGameView.EnableLines(false);
    }



    private void OnCollisionEnter(Collision other)
    {
        if (miniGameStarted) return;
        Debug.Log($"[Cuttingboard:OnCollisionEnter] other={other.gameObject.name}");

        var ingredient = other.gameObject.GetComponent<Ingredient>();
        if (ingredient == null) return;

        ICutable cutable = ingredient as ICutable;
        Debug.Log($"[Cuttingboard:OnCollisionEnter] cutable={cutable}");

        if (cutable != null)
        {
            Debug.Log($"[Cuttingboard:OnCollisionEnter] found ICutable");
        }
        else
        {
            return;
        }

        if (!cutable.IsCut)
        {
            currentCutable = cutable;
            cutable.Place(this.transform.position + (Vector3.up));
            StartCuttingMiniGame();
        }
        else
        {
            Debug.Log($"[Cuttingboard:OnCollisionEnter] already cut");

        }
    }

}
