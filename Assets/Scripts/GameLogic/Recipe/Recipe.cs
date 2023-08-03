using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Recipe 
{
    // Start is called before the first frame update
    [SerializeField] private string recipeName;
    [SerializeField] private List<IngredientAspect> neededAspects;
    [SerializeField] private GameObject resultingObject;    
}
