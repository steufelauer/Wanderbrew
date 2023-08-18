using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClickingPatternPoint : MonoBehaviour
{
    [SerializeField] MeshRenderer myMeshRenderer;
    public bool Clicked { get => clicked; set => clicked = value; }
    public float CurrentTimePosition { get => currentTimePosition; set => currentTimePosition = value; }
    public bool IsAnimated { get => isAnimated; set => isAnimated = value; }
    public ClickingPattern ClickingPattern { get => clickingPattern; set => clickingPattern = value; }

    private ClickingPattern clickingPattern;
    private bool clicked;
    private bool hidden;
    private Material myMaterial;

    private bool inFadeOut = false;
    private bool inFadeIn = false;

    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    private float currentTimePosition;
    private bool isAnimated;
    private AnimationCurve currentAnimationCurve;
    private float previousAlpha = 1f;

    private float slowDown = 0.25f;


    private void Start()
    {
        myMaterial = myMeshRenderer.material;
    }

    private void Update()
    {
        if (!isAnimated) return;
        currentTimePosition += Time.deltaTime * ClickingPattern.CurrentSliderSpeed * slowDown;
        float alpha = currentAnimationCurve.Evaluate(currentTimePosition);
        if (previousAlpha != alpha)
        {
            if (alpha == 0f)
            {
                Hide(true);
            }
            else
            {
                Hide(false);
            }
        }
        currentTimePosition = currentTimePosition >= 1f ? 0f : currentTimePosition;
    }
    public void Click()
    {
        if (clicked) return;
        Debug.Log("Clicked!");
        clicked = true;
        isAnimated = false;
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
            if (fadeOutCoroutine == null)
            {
                if (inFadeIn)
                {
                    StopFadeIn();
                }
                if (!inFadeOut)
                {
                    fadeOutCoroutine = StartCoroutine(FadeOutAlpha());
                    inFadeOut = true;
                }
            }
        }
        else
        {
            if (fadeInCoroutine == null)
            {
                if (inFadeOut)
                {
                    StopFadeOut();
                }
                if (!inFadeIn)
                {
                    fadeInCoroutine = StartCoroutine(FadeInAlpha());
                    inFadeIn = true;
                }
            }
        }
    }

    public void StartAnimation(float timePos, AnimationCurve animationCurve)
    {
        currentTimePosition = timePos;
        this.currentAnimationCurve = animationCurve;
        isAnimated = true;
    }

    public void Reset()
    {
        clicked = false;
    }

    private IEnumerator FadeOutAlpha()
    {
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
            previousAlpha = 0.0f;
            StopFadeOut();
        }
    }

    private IEnumerator FadeInAlpha()
    {
        Color c = myMaterial.color;
        float alpha = c.a;
        while (c.a < 1f)
        {
            alpha += 0.01f;
            if (alpha > 1f)
                alpha = 1.0f;
            c.a = alpha;
            myMaterial.color = c;
            yield return null;
        }
        if (c.a >= 1.0f)
        {
            previousAlpha = 1.0f;
            StopFadeIn();
        }
    }

    public void StopFadeIn()
    {
        inFadeIn = false;
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }
    }

    public void StopFadeOut()
    {
        inFadeOut = false;
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }
    }
}
