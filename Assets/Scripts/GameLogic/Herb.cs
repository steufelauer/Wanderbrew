using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : Ingredient, ICutable
{
    // Start is called before the first frame update
    [SerializeField] private GameObject originalHerb;
    [SerializeField] private GameObject cutHerb;

    public bool IsCut => isCut;

    private bool isCut;
    private GameObject currentActiveObject;


    public void Cut(){
        Debug.Log($"[Herb::Cut] is already cut = {isCut}");
        if(isCut) return;
        originalHerb.SetActive(false);
        cutHerb.SetActive(true);
        currentActiveObject = cutHerb;
        isCut = true;
    }

    public void Place(Vector3 position){
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
