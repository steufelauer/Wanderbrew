using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellController : MonoBehaviour
{
    [SerializeField] private List<ISellable> sellables;
    [SerializeField] private OrderController orderController;
    [SerializeField] private OrderCard currentOrder;
    [SerializeField] private TriggerArea triggerArea;

    private Recipe activeRecipe;
    // Start is called before the first frame update
    void Start()
    {
        RequestOrder();

        // triggerArea.AreaTriggerEntered += OnSellAreaEntered;
        // triggerArea.AreaTriggeredExited += OnSellAreaExited;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void RequestOrder()
    {
        activeRecipe = orderController.RequestRecipe();
        currentOrder.SetUp(activeRecipe.RecipeName, activeRecipe.NeededAspects, activeRecipe.Value);
    }

    private void OnSellAreaEntered(Collider other)
    {
        ISellable sellable = other.gameObject.GetComponent<ISellable>();
        if (sellable == null)
        {
            return;
        }
        if (!sellables.Contains(sellable))
        {
            sellables.Add(sellable);
        }
    }
    private void OnSellAreaExited(Collider other)
    {
        ISellable sellable = other.gameObject.GetComponent<ISellable>();
        if (sellable == null)
        {
            return;
        }
        if (sellables.Contains(sellable))
        {
            sellables.Remove(sellable);
        }
    }

    public void Sell(){
        for (int i = 0; i < sellables.Count; i++)
        {
            //if(sellables[i].As)
        }
    }
}
