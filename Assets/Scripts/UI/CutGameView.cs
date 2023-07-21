using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutGameView : MonoBehaviour
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
    [SerializeField] private Button ReturnButton;
    [SerializeField] private TMP_Text cutRankText;
    [SerializeField] private TMP_Text finalRankText;
    [SerializeField] private List<string> perfectTxts;
    [SerializeField] private List<string> rankATxts;
    [SerializeField] private List<string> rankBTxts;
    [SerializeField] private List<string> rankCTxts;
    [SerializeField] private List<string> badRankTxts;
    [SerializeField] private Image DbgLineTop;
    [SerializeField] private Image DbgLineBot;
    [SerializeField] private Image DbgPerfectRank;
    [SerializeField] private List<Image> DbgRanks = new();

    public event Action EndMinigame = delegate { };

    private Vector3 currentGoalTopPoint;
    private Vector3 currentGoalBotPoint;
    private Vector3 currentMouseStartPoint;
    private Vector3 currentMouseEndPoint;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private CuttingboardController controller;

    //DBG
    private int dbgPerfectScore;
    private int dbgRankScore;
    private int dbgMaxRanks;


    //TODO service?
    public CuttingboardController Controller { get => controller; set => controller = value; }

    void Start()
    {
        canvas = this.gameObject.GetComponentInParent<Canvas>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        Debug.Log($"Found canvas? ={canvas}");

        if (DbgRanks.Count < 1)
        {
            DbgRanks.Add(DbgPerfectRank);
        }
        SetDbgImages();
        Reset();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetDbgImages()
    {
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

    public void Reset()
    {
        cutRankText.text = "";
        finalRankText.text = "";
        ReturnButton.gameObject.SetActive(false);
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
    }

    public void SetMouseEndPoint(Vector3 pos)
    {
        mouseEndPoint.transform.position = pos;
        currentMouseEndPoint = pos;
        DrawMouseLine();
        var topInput = currentMouseStartPoint.y <= currentMouseEndPoint.y ? currentMouseStartPoint : currentMouseEndPoint;
        var botInput = currentMouseStartPoint.y <= currentMouseEndPoint.y ? currentMouseEndPoint : currentMouseStartPoint;
        // if (currentMouseStartPoint.y <= currentMouseEndPoint.y)
        // {
        //     controller.CalculatePercentOverlap(currentGoalTopPoint, currentGoalBotPoint, currentMouseEndPoint, currentMouseStartPoint);

        // var topInput = currentMouseStartPoint;
        // var botInput = currentMouseEndPoint;
        // }
        // else
        // {
        //     controller.CalculatePercentOverlap(currentGoalTopPoint, currentGoalBotPoint, currentMouseStartPoint, currentMouseEndPoint);

        // var topInput = currentMouseStartPoint;
        // var botInput = currentMouseEndPoint;
        // }

        var slope = (topInput.y - botInput.y) / (topInput.x - botInput.x);
        // Debug.Log("Slope:" + slope);
        // Debug.Log($"Slope:  ({topInput.y} - {botInput.y}) /  ({topInput.x} - {botInput.x})");
        // Debug.Log("Slope:" + (topInput.y - botInput.y)+ " / " + (topInput.x - botInput.x));
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
    public void EnableCanvasGroup(bool enable)
    {
        if (enable)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }
    }
    public void EndBtn() => EndMinigame();


    public void DrawLine(Image img, Vector3 top, Vector3 bot)
    {
        //lineImage.sprite = lineImage;
        //lineImage.color = col;
        RectTransform rect = img.GetComponent<RectTransform>();
        //rect.SetParent(transform);
        rect.localScale = Vector3.one;

        // Vector3 a = new Vector3(ax*graphScale.x, ay*graphScale.y, 0);
        // Vector3 b = new Vector3(bx*graphScale.x, by*graphScale.y, 0);
        Vector3 a = new Vector3(top.x, top.y, 0);
        Vector3 b = new Vector3(bot.x, bot.y, 0);

        Debug.Log($"A={a}, B={b}");
        rect.localPosition = a.x <= b.x ? a : b;
        //rect.localPosition = a;
        Vector3 dif = a - b;
        rect.sizeDelta = new Vector3(dif.magnitude, 5f);
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));

        // rect.anchorMin = Vector2.zero;
        // rect.anchorMax = Vector2.zero;
    }

    public void DisplayFinalRank(string content)
    {
        finalRankText.text = content;
        ReturnButton.gameObject.SetActive(true);
    }

}
