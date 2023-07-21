using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICutable
{
    bool IsCut {get;}
    void Cut();
    void Place(Vector3 transform);
}
