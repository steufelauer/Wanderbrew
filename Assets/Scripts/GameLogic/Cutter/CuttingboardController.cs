using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingboardController : MiniGameController
{
     [SerializeField] private int roundCount = 5;
    // [Tooltip("In Pixel as radius")]
    // [SerializeField] private int perfectScoreMargin = 10;
    // [SerializeField] private int rankMargin = 10;
    // [SerializeField] private int maxRanks = 3;

    private CutGameView cutGameView;
    private int finishedRoundCount = 0;
    private int perfectScoreMarginInt;
    private int rankMarginInt;

    private List<int> ranks = new();

    // protected IServiceLocator serviceLocator;
    // protected TooltipProvider tooltipProvider;
    // protected IUIProviderService uIProviderService;

    // private bool miniGameStarted = false;
    private bool waitingForInput = false;

    private ICutable currentCutable;
    // IGameStateService gameStateService;

    protected override void Awake()
    {
        base.Awake();
        perfectScoreMarginInt = (int)perfectScoreMargin;
        rankMarginInt = (int)rankMargin;
    }

    protected override void Start()
    {
        base.Start();
        cutGameView = gameView as CutGameView;
        cutGameView.Controller = this;
        cutGameView.InitiatePointCondition(perfectScoreMarginInt, rankMarginInt, maxRanks);
    }

    void Update()
    {
        if (!miniGameStarted || !waitingForInput) return;
        if (Input.GetMouseButtonDown(0))
        {
            cutGameView.SetMouseStartingPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            cutGameView.SetMouseEndPoint(Input.mousePosition);
        }
    }

    private void StartCuttingMiniGame()
    {
        Debug.Log("StartMinigame");
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

    protected override void Reset()
    {
        base.Reset();
        finishedRoundCount = 0;
        ranks.Clear();
        miniGameStarted = false;
        waitingForInput = false;
    }

    protected override void EndMiniGame(){
        base.EndMiniGame();
         Debug.Log("CuttingBoardEndMinigame");
        Reset();
    }

    public int CalculatePercentOverlap(Vector3 topGoal, Vector3 botGoal, Vector3 topReached, Vector3 botReached, float lineLength)
    {
        if (!waitingForInput) return -1;
        waitingForInput = false;
        int rank = 0;
        var perfectScoreSize = lineLength * perfectScoreMargin / 100;
        var rankScoreSize = lineLength * rankMargin / 100;

        var topYRank = Mathf.Abs(topGoal.y - topReached.y);
        var topXRank = Mathf.Abs(topGoal.x - topReached.x);

        var botYRank = Mathf.Abs(botGoal.y - botReached.y);
        var botXRank = Mathf.Abs(botGoal.x - botReached.x);

        bool perfectScoreTop = false;
        bool perfectScoreBot = false;
        if (topYRank <= perfectScoreSize && topXRank <= perfectScoreSize)
        {
            perfectScoreTop = true;
            rank = 0;
        }
        if (botYRank <= perfectScoreSize && botXRank <= perfectScoreSize)
        {
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
                if (topYRank <= rankScoreSize && topXRank <= rankScoreSize)
                {
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
                    botRank = i + 1;
                    assignedBotRank = true;
                    break;
                }
                botYRank -= rankScoreSize;
                botXRank -= rankScoreSize;
            }

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
                dbgLog += " + " + ranks[i];
                finalRank += ranks[i];
            }
            dbgLog += " ->  " + finalRank;
            finalRank /= ranks.Count;
            dbgLog += " / " + ranks.Count + " -> " + finalRank;
            finalRank = finalRank > maxRanks ? maxRanks : finalRank;

            cutGameView.DisplayFinalRank("Rank " + "ABCDEFGH"[finalRank]);
            currentCutable.Cut(finalRank);
        }
        return rank;
    }
    protected override void SetUpGame()
    {
        throw new System.NotImplementedException();
    }

    protected override int CalculateRank()
    {
        throw new System.NotImplementedException();
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

        var ingredient = other.gameObject.GetComponent<Ingredient>();
        if (ingredient == null) return;

        ICutable cutable = ingredient as ICutable;

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
