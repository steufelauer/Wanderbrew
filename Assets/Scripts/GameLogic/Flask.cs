using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flask : Ingredient, ILightMixable
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rigidBody;

    public bool IsMixed => isMixed;

    private bool isMixed;
    private GameObject currentActiveObject;


    public void Mix(int rank){
        Debug.Log($"[Herb::Flask] is already cut = {isMixed}");
        if(isMixed) return;
        isMixed = true;
    }

    public void ActiveRigidbody(bool activate)
    {
            rigidBody.isKinematic = !activate;        
    }

    public void Place(Vector3 position){
        Debug.Log("Placing flask to " + position);
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
