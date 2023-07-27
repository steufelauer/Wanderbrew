using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILightMixable
{
    bool IsMixed {get;}    
    Color FluidColor { get; set;}
    void Mix(int rank);
    void Place(Vector3 transform);
    void ActiveRigidbody(bool activate);
}
