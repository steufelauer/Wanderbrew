
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingFinishedTrigger : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;

    public event Action<Collider> MixingFinishedTriggered = delegate { };



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        MixingFinishedTriggered(other);
    }
    

}