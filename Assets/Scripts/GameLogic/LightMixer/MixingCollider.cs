
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingCollider : MonoBehaviour
{
    public event Action<bool, Collider> MixingColliderChanged = delegate { };


    private void OnTriggerEnter(Collider other)
    {
        MixingColliderChanged(true, other);
    }
    private void OnTriggerExit(Collider other)
    {
        MixingColliderChanged(false, other);
    }
    

}