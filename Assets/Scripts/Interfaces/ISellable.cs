using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public interface ISellable
{
    List<IngredientAspect> AspectDetails { get; }
    int ValueMultiplier { get; }
}
