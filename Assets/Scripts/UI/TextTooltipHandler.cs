using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTooltipHandler : MonoBehaviour
{
    [SerializeField] private GameObject textTooltipRoot;
    [SerializeField] private TMPro.TMP_Text textTooltipText;
    [SerializeField] private GameObject aspectTooltipRoot;
    [SerializeField] private Image aspectTooltipImg;
    [SerializeField] private Image aspectTooltipImgBG;
    [SerializeField] private Image aspectTooltipImgReached;
    //TODO dynamic
    [SerializeField] private GameObject aspectTooltipRoot2;
    [SerializeField] private Image aspectTooltipImg2;
    [SerializeField] private Image aspectTooltipImgBG2;
    [SerializeField] private Image aspectTooltipImgReached2;


    public void ShowTooltip(string txt, Vector3 pos)
    {
        textTooltipText.text = txt;
        textTooltipRoot.gameObject.transform.position = pos + (Vector3.up * 2f);
        //UnityEngine.Debug.Log($"Showing tooltip {txt}, moved to {pos} ");
        textTooltipRoot.SetActive(true);
        aspectTooltipRoot.SetActive(false);
        aspectTooltipRoot2.SetActive(false);
    }
    public void ShowAspectTooltip(string txt, IngredientAspect detail, float reached, Vector3 pos)
    {
        ShowTooltip(txt, pos);
        aspectTooltipRoot.SetActive(true);
        //aspectTooltipText.text = detail.Points.ToString("0.##");
        aspectTooltipImg.sprite = SpriteManager.GetSpriteByAspect(detail.Aspect);
        aspectTooltipImgBG.sprite = SpriteManager.GetSpriteByAspect(detail.Aspect);
        aspectTooltipImgReached.sprite = SpriteManager.GetSpriteByAspect(detail.Aspect);

        aspectTooltipImg.fillAmount = detail.Points/10f;
        aspectTooltipImgReached.fillAmount = reached/10f;
    }
    private void EnableSecondAspect(IngredientAspect detail, float reached)
    {
        aspectTooltipRoot2.SetActive(true);
        //aspectTooltipText.text = detail.Points.ToString("0.##");
        aspectTooltipImg2.sprite = SpriteManager.GetSpriteByAspect(detail.Aspect);
        aspectTooltipImgBG2.sprite = SpriteManager.GetSpriteByAspect(detail.Aspect);
        aspectTooltipImgReached2.sprite = SpriteManager.GetSpriteByAspect(detail.Aspect);

        aspectTooltipImg2.fillAmount = detail.Points / 10f;
        aspectTooltipImgReached2.fillAmount = reached / 10f;
    }
    public void ShowAspectTooltipMultpleAspects(string txt, List<IngredientAspect> detail, Vector3 pos)
    {
        ShowAspectTooltip(txt, detail[0], detail[0].Points, pos);
        EnableSecondAspect(detail[1], detail[1].Points);
    }

    public void HideToolTip()
    {
        textTooltipRoot.SetActive(false);
        //UnityEngine.Debug.Log($"Hiding tooltip");
    }

    public void UpdatePosition(Vector3 pos)
    {
        textTooltipRoot.gameObject.transform.position = pos + (Vector3.up * 2f);
    }
}

