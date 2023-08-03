using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Recipe 
{
    // Start is called before the first frame update
    [SerializeField] public string RecipeName;
    [SerializeField] public List<IngredientAspect> NeededAspects;
    [SerializeField] public GameObject ResultingObject;    
}
