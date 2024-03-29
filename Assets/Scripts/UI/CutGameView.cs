using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutGameView : MiniGameView
{
    //TODO take into account that the start line point of the user can be on bottom as well, not only top
    [Serializable]
    class LineSkeleton
    {
        [SerializeField] public RectTransform TopLeft;
        [SerializeField] public RectTransform TopRight;
        [SerializeField] public RectTransform BottomLeft;
        [SerializeField] public RectTransform BottomRight;
    }
    // Start is called before the first frame update

    [SerializeField] private bool useDBG = true;
    [SerializeField] private LineSkeleton linePoints;
    [SerializeField] private Image lineImage;
    [SerializeField] private Image mouseStartPoint;
    [SerializeField] private Image mouseEndPoint;
    [SerializeField] private Image mouseLineImage;
    [SerializeField] private TMP_Text cutRankText;
    [SerializeField] private List<string> perfectTxts;
    [SerializeField] private List<string> rankATxts;
    [SerializeField] private List<string> rankBTxts;
    [SerializeField] private List<string> rankCTxts;
    [SerializeField] private List<string> badRankTxts;
    [SerializeField] private Image DbgLineTop;
    [SerializeField] private Image DbgLineBot;
    [SerializeField] private Image DbgPerfectRank;
    [SerializeField] private List<Image> DbgRanks = new();

    // public event Action EndMinigame = delegate { };
    public CuttingboardController Controller { get => controller; set => controller = value; }
    private CuttingboardController controller;

    private Vector3 currentGoalTopPoint;
    private Vector3 currentGoalBotPoint;
    private Vector3 currentMouseStartPoint;
    private Vector3 currentMouseEndPoint;


    //DBG
    private int dbgPerfectScore;
    private int dbgRankScore;
    private int dbgMaxRanks;


    //TODO service?
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        if (DbgRanks.Count < 1)
        {
            DbgRanks.Add(DbgPerfectRank);
        }

        EnableDBG(false);
        Reset();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public override void Reset()
    {
        base.Reset();
        cutRankText.text = "";
        rankText.text = "";
        EnableCanvasGroup(false);
        //EnableLines(false);
        mouseLineImage.transform.localScale = Vector3.zero;
    }


    public void InitiatePointCondition(int perfectScorePerc, int rankPerc, int maxRanks)
    {
        this.dbgPerfectScore = perfectScorePerc;
        this.dbgRankScore = rankPerc;
        this.dbgMaxRanks = maxRanks;
    }


    public void SetMouseStartingPoint(Vector3 pos)
    {
        mouseStartPoint.transform.position = pos;
        currentMouseStartPoint = pos;
        
        //TODO kick
        mouseStartPoint.GetComponent<Image>().enabled = true;
        mouseStartPoint.transform.position = pos;
    }

    public void SetMouseEndPoint(Vector3 pos)
    {
        //TODO kick
        if (!useDBG)
        {
            mouseStartPoint.GetComponent<Image>().enabled = false;
        }
    
        mouseEndPoint.transform.position = pos;
        currentMouseEndPoint = pos;
        if(currentMouseStartPoint.x == currentMouseEndPoint.x && currentMouseStartPoint.y == currentMouseEndPoint.y)
        {            
            Debug.LogError($"[CutGameView::SetMouseEndPoint]Click detected, ignoring");
            return;
        }
        DrawMouseLine();
        var topInput = currentMouseStartPoint.y <= currentMouseEndPoint.y ? currentMouseStartPoint : currentMouseEndPoint;
        var botInput = currentMouseStartPoint.y <= currentMouseEndPoint.y ? currentMouseEndPoint : currentMouseStartPoint;
        if(topInput.x == botInput.x){
            Debug.LogError($"[CutGameView::SetMouseEndPoint] Division 0 prevented from x calculation, changing point");
            botInput = new Vector3(botInput.x + 1f, botInput.y, 0);
        }
        var slope = (topInput.y - botInput.y) / (topInput.x - botInput.x);

        if (slope == 0f)
        {
            Debug.LogError($"[CutGameView::SetMouseEndPoint] Division 0 prevented from slope, changing slope");
            slope = 0.01f;
        }
        var topX = (currentGoalTopPoint.y - topInput.y) / slope + topInput.x;
        var botX = (currentGoalBotPoint.y - botInput.y) / slope + botInput.x;

        Vector3 topReached = new Vector3(topX, currentGoalTopPoint.y, 0);
        Vector3 botReached = new Vector3(botX, currentGoalBotPoint.y, 0);


        var lineLength = currentGoalTopPoint.y - currentGoalBotPoint.y;
        var perfectScoreSize = lineLength * dbgPerfectScore / 100 * 2;//scoresize is going in all directions, not the general size, thus times 2
        var rankScoreSize = lineLength * dbgRankScore / 100 * 2;

#region DbgDisplay
        if (useDBG)
        {
            DbgLineTop.transform.position = topReached;
            DbgLineBot.transform.position = botReached;


            Debug.Log("LineLength:" + lineLength);
            Debug.Log("PerfectRankDistance: " + perfectScoreSize);
            DbgPerfectRank.transform.position = currentGoalTopPoint;
            //DbgRanks.transform.localScale = new Vector3(scoreSize, scoreSize, 0);
            DbgPerfectRank.rectTransform.sizeDelta = new Vector2(perfectScoreSize, perfectScoreSize);
            if (DbgRanks.Count < 2)
            {
                var botPerfectScore = Instantiate(DbgPerfectRank);
                botPerfectScore.transform.parent = DbgPerfectRank.transform.parent;
                DbgRanks.Add(botPerfectScore);
            }
            DbgRanks[1].transform.position = currentGoalBotPoint;
            DbgRanks[1].rectTransform.sizeDelta = new Vector2(perfectScoreSize, perfectScoreSize);
            for (int i = 1; i < dbgMaxRanks + 1; i++)
            {
                if (DbgRanks.Count < (i * 2 + 1))
                {
                    var newRankImg = Instantiate(DbgPerfectRank);
                    newRankImg.transform.parent = DbgPerfectRank.transform.parent;
                    DbgRanks.Add(newRankImg);
                    newRankImg = Instantiate(DbgPerfectRank);
                    newRankImg.transform.parent = DbgPerfectRank.transform.parent;
                    DbgRanks.Add(newRankImg);
                }
                DbgRanks[i * 2].transform.position = currentGoalTopPoint;
                DbgRanks[i * 2].rectTransform.sizeDelta = new Vector2(rankScoreSize * (i) + perfectScoreSize, rankScoreSize * (i) + perfectScoreSize);
                DbgRanks[i * 2 + 1].transform.position = currentGoalBotPoint;
                DbgRanks[i * 2 + 1].rectTransform.sizeDelta = new Vector2(rankScoreSize * (i) + perfectScoreSize, rankScoreSize * (i) + perfectScoreSize);
            }
        }
#endregion

        var rank = controller.CalculatePercentOverlap(currentGoalTopPoint, currentGoalBotPoint, topReached, botReached, lineLength);

        switch (rank)
        {
            case 0:
            var random = UnityEngine.Random.Range(0, perfectTxts.Count);
            cutRankText.text = perfectTxts[random];
            break;
            case 1:
            random = UnityEngine.Random.Range(0, rankATxts.Count);
            cutRankText.text = rankATxts[random];
            break;
            case 2:
            random = UnityEngine.Random.Range(0, rankBTxts.Count);
            cutRankText.text = rankBTxts[random];
            break;
            case 3:
            random = UnityEngine.Random.Range(0, rankCTxts.Count);
            cutRankText.text = rankCTxts[random];
            break;
            case -1:
            cutRankText.text = "";
            break;
            default:
            random = UnityEngine.Random.Range(0, badRankTxts.Count);
            cutRankText.text = badRankTxts[random];
            break;
        }
    }

    public void ToggleDBG()
    {
        Debug.Log("ToggleDBG");
        EnableDBG(!useDBG);
    }

    public void EnableDBG(bool enable){
        useDBG = enable;

        linePoints.TopLeft.GetComponent<Image>().enabled = useDBG;
        linePoints.TopRight.GetComponent<Image>().enabled = useDBG;
        linePoints.BottomLeft.GetComponent<Image>().enabled = useDBG;
        linePoints.BottomRight.GetComponent<Image>().enabled = useDBG;
        DbgLineTop.GetComponent<Image>().enabled = useDBG;
        DbgLineBot.GetComponent<Image>().enabled = useDBG;
        DbgPerfectRank.GetComponent<Image>().enabled = useDBG;
        mouseStartPoint.GetComponent<Image>().enabled = useDBG;
        mouseEndPoint.GetComponent<Image>().enabled = useDBG;
        
        foreach (var image in DbgRanks)
        {
            image.GetComponent<Image>().enabled = useDBG;
        }
    }


    public void DrawRandomLine()
    {

        float randomTop = UnityEngine.Random.Range(linePoints.TopLeft.position.x, linePoints.TopRight.position.x);
        float randomBot = UnityEngine.Random.Range(linePoints.BottomLeft.position.x, linePoints.BottomRight.position.x);

        currentGoalTopPoint = new Vector3(randomTop, linePoints.TopLeft.position.y, 0);
        currentGoalBotPoint = new Vector3(randomBot, linePoints.BottomLeft.position.y, 0);

        DrawLine(lineImage, currentGoalTopPoint, currentGoalBotPoint);
    }

    public void DrawMouseLine()
    {
        if (mouseStartPoint.transform.position.y <= mouseEndPoint.transform.position.y)
        {
            DrawLine(mouseLineImage, mouseStartPoint.transform.position, mouseEndPoint.transform.position);
        }
        else
        {
            DrawLine(mouseLineImage, mouseEndPoint.transform.position, mouseStartPoint.transform.position);
        }
    }

    public void EnableLines(bool enable)
    {
        lineImage.gameObject.SetActive(enable);
        mouseLineImage.gameObject.SetActive(enable);
    }


    public void DrawLine(Image img, Vector3 top, Vector3 bot)
    {
        RectTransform rect = img.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;

        Vector3 a = new Vector3(top.x, top.y, 0);
        Vector3 b = new Vector3(bot.x, bot.y, 0);

        rect.localPosition = a.x <= b.x ? a : b;
        Vector3 dif = a - b;

        if(dif.x == 0){
            Debug.LogError($"[CutGameView::DrawLine] Division 0 prevented from dif.x calculation, changing point");
            dif = new Vector3(dif.x + 0.1f, dif.y, dif.z);
        }

        rect.sizeDelta = new Vector3(dif.magnitude, 5f);
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));

    }

    // public void DisplayFinalRank(string content)
    // {
    //     rankText.text = content;
    //     ReturnButton.gameObject.SetActive(true);
    // }

}
