using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RecipesScriptableObject", order = 1)]
public class RecipesScriptableObject : ScriptableObject
{
    public string prefabName;
    public List<Recipe> myRecipes;
}
