using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject cauldronWaterDisk;
    [SerializeField] private Transform ingredientRoot;
    [SerializeField] private Transform cauldronWaterDiskRoot;
    [SerializeField] private TriggerArea triggerArea;
    void Start()
    {
        triggerArea.AreaTriggerEntered += OnHookingTriggered;
        triggerArea.AreaTriggeredExited += OnHookingExited;
    }

    private void OnDestroy()
    {
        triggerArea.AreaTriggerEntered -= OnHookingTriggered;
        triggerArea.AreaTriggeredExited -= OnHookingExited;
    }
    // Update is called once per frame


    void FixedUpdate()
    {
        cauldronWaterDisk.transform.Rotate(Vector3.up, Time.deltaTime * 10f);
    }

    private void OnHookingTriggered(Collider other)
    {
        IPickable pickable = other.gameObject.GetComponent<IPickable>();
        if (pickable == null)
        {
            return;
        }
        pickable.MyGameObject.transform.parent = cauldronWaterDiskRoot;
    }
    private void OnHookingExited(Collider other)
    {
        IPickable pickable = other.gameObject.GetComponent<IPickable>();
        if (pickable == null)
        {
            return;
        }
        pickable.MyGameObject.transform.parent = ingredientRoot;
    }

}
