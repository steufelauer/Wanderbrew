using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronController : MiniGameController
{
    // Start is called before the first frame update
    [SerializeField] private Transform cancelIngredientDrop;

    [SerializeField] private GameObject cauldronWaterDisk;
    [SerializeField] private Transform ingredientRoot;
    [SerializeField] private Transform cauldronWaterDiskRoot;
    [SerializeField] private TriggerArea triggerArea;


    private List<Ingredient> ingredients = new();
    protected override void Start()
    {
        base.Start();
        triggerArea.AreaTriggerEntered += ConsumeIngredient;
        //triggerArea.AreaTriggeredExited += OnHookingExited;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        triggerArea.AreaTriggerEntered -= ConsumeIngredient;
        //triggerArea.AreaTriggeredExited -= OnHookingExited;
    }
    // Update is called once per frame


    void FixedUpdate()
    {
        cauldronWaterDisk.transform.Rotate(Vector3.up, Time.deltaTime * 10f);
    }

    // private void OnHookingTriggered(Collider other)
    // {
    //     IPickable pickable = other.gameObject.GetComponent<IPickable>();
    //     if (pickable == null)
    //     {
    //         return;
    //     }
    //     pickable.MyGameObject.transform.parent = cauldronWaterDiskRoot;
    // }
    // private void OnHookingExited(Collider other)
    // {
    //     IPickable pickable = other.gameObject.GetComponent<IPickable>();
    //     if (pickable == null)
    //     {
    //         return;
    //     }
    //     pickable.MyGameObject.transform.parent = ingredientRoot;
    // }

    protected override void SetUpGame()
    {
        Debug.Log("SetUpGame");
    }

    protected override int CalculateRank()
    {
        Debug.Log("CalculateRank");
        return 0;
    }


    private void ConsumeIngredient(Collider other)
    {
        if (miniGameStarted) return;

        var ingredient = other.gameObject.GetComponent<Ingredient>();
        if (ingredient == null) return;

        if (ingredient.IsPrepared)
        {
            ingredients.Add(ingredient);
            ingredient.gameObject.transform.position = startPlacement.position;
            ingredient.MyRigidBody.isKinematic = true;
        }
        else
        {
            Debug.Log($"Not prepared!");
            
            ingredient.gameObject.transform.position = cancelIngredientDrop.position;
        }
    }
}
