
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    public event Action<Collider> AreaTriggerEntered = delegate { };
    public event Action<Collider> AreaTriggeredExited = delegate { };


    private void OnTriggerEnter(Collider other) => AreaTriggerEntered(other);

    private void OnTriggerExit(Collider other) => AreaTriggeredExited(other);


}