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


    public void ShowTooltip(string txt, Vector3 pos)
    {
        textTooltipText.text = txt;
        textTooltipRoot.gameObject.transform.position = pos + (Vector3.up * 2f);
        //UnityEngine.Debug.Log($"Showing tooltip {txt}, moved to {pos} ");
        textTooltipRoot.SetActive(true);
        aspectTooltipRoot.SetActive(false);
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

