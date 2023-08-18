using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellController : MonoBehaviour
{
    [SerializeField] private List<ISellable> sellables = new();
    [SerializeField] private OrderController orderController;
    [SerializeField] private OrderCard currentOrder;
    [SerializeField] private TriggerArea triggerArea;
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private Transform moneyTransform;

    private Recipe activeRecipe;
    // Start is called before the first frame update
    void Start()
    {
        RequestOrder();

        triggerArea.AreaTriggerEntered += OnSellAreaEntered;
        triggerArea.AreaTriggeredExited += OnSellAreaExited;
    }

    ~SellController()
    {
        triggerArea.AreaTriggerEntered -= OnSellAreaEntered;
        triggerArea.AreaTriggeredExited -= OnSellAreaExited;
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
        Debug.Log("OnSellAreaEntered");
        ISellable sellable = other.gameObject.GetComponent<ISellable>();
        if (sellable == null)
        {
            return;
        }
        if (!sellables.Contains(sellable))
        {            
            Debug.Log("OnSellAreaEntered added");
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
            Debug.Log("OnSellAreaEntered removed");
            sellables.Remove(sellable);
        }
    }

    public void Sell()
    {
        Debug.Log("Sell");
        int moneySpawn = 0;
        ISellable sellable;
        for (int i = 0; i < sellables.Count; i++)
        {
            for (int j = 0; j < sellables[i].AspectDetails.Count; j++)
            {
                Debug.Log(i+": " + sellables[i].AspectDetails[j].Aspect + " == " + activeRecipe.NeededAspects[0].Aspect);
                if (sellables[i].AspectDetails[j].Aspect == activeRecipe.NeededAspects[0].Aspect)
                {
                    Debug.Log(i+"-"+j+": " + sellables[i].AspectDetails[j].Points + " == " + activeRecipe.NeededAspects[0].Points);
                    if (sellables[i].AspectDetails[j].Points >= activeRecipe.NeededAspects[0].Points)
                    {
                        sellable = sellables[i];

                        moneySpawn = activeRecipe.Value;
                        Destroy(sellable.GO);
                        break;
                    }
                }
            }
        }


        for (int i = 0; i < moneySpawn; i++)
        {
            Instantiate(moneyPrefab, moneyTransform);
        }

    }
}
