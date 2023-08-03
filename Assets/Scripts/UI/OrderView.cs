using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderView : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject orderRoot;
    [SerializeField] private OrderCard orderCard;
    private List<OrderCard> orderCards = new();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DisplayOrder(Recipe order){
        var card = Instantiate(orderCard, orderRoot.transform);
        card.SetUp(order.RecipeName, order.NeededAspects);
        card.gameObject.SetActive(true);
        orderCards.Add(card);
    }   

    public void DeleteOrder(){

    }
}
