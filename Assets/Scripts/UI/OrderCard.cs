using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderCard : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text orderName;
    [SerializeField] private TMP_Text valueTxt;
    [SerializeField] private Image objectImage;
    [SerializeField] private AspectDetail aspectDetail;
    [SerializeField] private GameObject aspectDetailRoot;
    [SerializeField] private List<AspectDetail> aspectDetails;

    public void SetUp(string name, List<IngredientAspect> neededAspects, int value = 0)
    {
        orderName.text = name;
        if(value > 0){
            valueTxt.text = "Reward: " + value + " money";
        }else{
            valueTxt.text = "";
        }
        for (int i = 0; i < neededAspects.Count; i++)
        {
            var details = Instantiate(aspectDetail, aspectDetailRoot.transform);
            details.SetUp(neededAspects[i].Aspect, neededAspects[i].Points);
            details.gameObject.SetActive(true);

            aspectDetails.Add(details);
        }
    }
}
