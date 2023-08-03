using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flask : Ingredient, ILightMixable
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private MeshRenderer fluidMesh;
    [SerializeField, Tooltip("Set in runtime")] private Color fluidColor;

    public Color FluidColor
    {
        get => fluidColor;
        set
        {
            fluidColor = value;
            fluidMesh.material.color = fluidColor;
        }
    }
    

    public bool IsMixed => isMixed;
    protected override bool IsPrepared => IsMixed;

    private bool isMixed;
    private GameObject currentActiveObject;

    public override void Start()
    {
        base.Start();
        Material fluidMaterial = fluidMesh.material;
        fluidColor = fluidMaterial.color;
    }


    public void Mix(int rank, int maxRanks)
    {
        //Debug.Log($"[Herb::Flask] is already cut = {isMixed}");
        if (isMixed) return;
        isMixed = true;
        //rank 1, 2, 4, 5

        reachedAspectDetail.Aspect = baseAspectDetail.Aspect;
        reachedAspectDetail.Points = baseAspectDetail.Points - (baseAspectDetail.Points / maxRanks * rank);
        //10
        // rank 1 -> 10
        //rank 5 -> 2
        //10/5 -> 2
        //
    
    }

    public void ActiveRigidbody(bool activate)
    {
        rigidBody.isKinematic = !activate;
    }

    public void Place(Vector3 position)
    {
        //Debug.Log("Placing flask to " + position);
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
