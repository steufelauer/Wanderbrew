using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICutable
{
    bool IsCut {get;}
    void Cut(int rank, int maxRanks);
    void Place(Vector3 transform);
}
