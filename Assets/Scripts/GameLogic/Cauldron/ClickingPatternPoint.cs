using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClickingPatternPoint : MonoBehaviour
{
    [SerializeField] MeshRenderer myMeshRenderer;
    public bool Clicked { get => clicked; set => clicked = value; }

    private bool clicked;
    private bool hidden;
    private Material myMaterial;

    private bool inFadeOut = false;
    private bool inFadeIn = false;

    private Coroutine currentCoroutine;


    private void Start()
    {
        myMaterial = myMeshRenderer.material;
    }
    public void Click()
    {
        if (clicked) return;
        Debug.Log("Clicked!");
        clicked = true;
        Hide(true);
    }

    public void Hide(bool hide)
    {
        if (hide == hidden) return;
        // Color bufCol = myMaterial.color;
        // bufCol.a = hide ? 0f: 1f;
        // myMaterial.color = bufCol;

        hidden = hide;

        if (hide)
        {
            inFadeOut = true;
            currentCoroutine = StartCoroutine(FadeOutAlpha());
        }
    }

    public IEnumerator FadeOutAlpha()
    {
        Debug.Log("FadeOutAlpha");
        Color c = myMaterial.color;
        float alpha = c.a;
        while (c.a > 0f)
        {
            alpha -= 0.01f;
            if (alpha < 0f)
                alpha = 0.0f;
            c.a = alpha;
            myMaterial.color = c;
            yield return null;
        }
        if (c.a <= 0.0f)
        {
            Debug.Log("FadeOutStopped");
            inFadeOut = false;
            StopCoroutine(currentCoroutine);
        }
    }
}
