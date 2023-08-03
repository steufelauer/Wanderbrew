using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : Ingredient, ICutable
{
    // Start is called before the first frame update
    [SerializeField] private GameObject originalHerb;
    [SerializeField] private List<GameObject> cutHerbs;

    protected override bool IsPrepared => isCut;
    public bool IsCut => isCut;
    private bool isCut;
    private GameObject currentActiveObject;


    public void Cut(int rank, int maxRanks){
        Debug.Log($"[Herb::Cut] is already cut = {isCut}");
        if(isCut) return;
        originalHerb.SetActive(false);
        if (rank >= cutHerbs.Count)
        {
            Debug.LogError($"[Herb::Cut]CutHerbs mesh missing for herb {gameObject.name} on rank={rank}");
            rank = cutHerbs.Count - 1;
        }
        cutHerbs[rank].SetActive(true);
        currentActiveObject = cutHerbs[rank];
        isCut = true;
        
        reachedAspectDetail.Points = baseAspectDetail.Points - (baseAspectDetail.Points / maxRanks * rank);
    }

    public void Place(Vector3 position){
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
