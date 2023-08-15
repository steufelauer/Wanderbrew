using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickingPattern : MonoBehaviour
{
    [SerializeField] List<ClickingPatternPoint> patternPoints;
    [SerializeField] bool isActive;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive) return;
    }


}
