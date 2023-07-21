using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTooltipHandler : MonoBehaviour
{
    [SerializeField] private GameObject textTooltipRoot;
    [SerializeField] private TMPro.TMP_Text textTooltipText;


    public void ShowTooltip(string txt, Vector3 pos)
    {
        textTooltipText.text = txt;
        textTooltipRoot.gameObject.transform.position = pos + (Vector3.up * 2f);
        //UnityEngine.Debug.Log($"Showing tooltip {txt}, moved to {pos} ");
        textTooltipRoot.SetActive(true);
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

